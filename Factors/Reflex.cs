using System;
using System.Collections.Generic;
using System.Diagnostics;
using Core;
using Core.Factors;
using Core.States;
using Factors.Cores;

namespace Factors
{
    //- Meant to be a very lightweight version of a Reactor, for situations where
    //  you have something you want to happen every time a given factor is triggered,
    //  don't want to make the triggering Factor necessary, and there is no issue with 
    //  the Reflex staying alive for the lifetime of the triggering Factor (no weak
    //  references are used, so the Reflex will just sit in the triggering Factor's
    //  subscriber pool indefinitely). 
    
    //- As a note, since these object go off every time their parent triggers them
    //  if the Reflex' response interacts with the parent in a way that causes it
    //  to react (for example trying to get the value of a Reactive<T> will cause it to
    //  react) then that parent and everything it depends on will essentially be
    //  reflexive.
    public abstract class Reflex : IUpdateable, IPrioritizedUpdate, IFactorSubscriber
    {
        #region Constants

        protected const uint Reacting  = 0b00001;
        protected const uint Queued    = 0b00010;
        protected const uint Unstable  = 0b00100;
        protected const uint Triggered = 0b01000;
        protected const uint Automatic = 0b10000;

        #endregion
       
        
        #region Instance Fields

        private uint state;

        #endregion

        
        #region Static Properties

        public static UpdateList UpdateList => Influence.UpdateList;

        #endregion
        

        #region Instance Properties

        public bool IsUnstable
        {
                      get => (state & Unstable) is Unstable;
            protected set
            {
                if (value is true)
                {
                    state |= Unstable;
                }
                else
                {
                    state &= ~Unstable;
                }
            }
        }

        public bool IsTriggered 
        {
                      get => (state & Triggered) is Triggered;
            protected set
            {
                if (value is true)
                {
                    state |= Triggered;
                }
                else
                {
                    state &= ~Triggered;
                }
            }
        }

        public bool IsReacting
        {
            get => (state & Reacting) is Reacting;
            protected set
            {
                if (value is true)
                {
                    state |= Reacting;
                }
                else
                {
                    state &= ~Reacting;
                }
            }
        }

        protected bool IsQueued
        {
            get => (state & Queued) is Queued;
            set
            {
                if (value is true)
                {
                    state |= Queued;
                }
                else
                {
                    state &= ~Queued;
                }
            }
        }

        protected abstract IEnumerable<IFactor> Triggers       { get; }
        public    abstract int                  UpdatePriority { get; }
        
        public bool AutomaticallyReacts
        {
            get => (state & Automatic) is Automatic;
            set
            {
                if (AutomaticallyReacts)
                {
                    if (value is false)
                    {
                        state &= ~Automatic;
                    }
                }
                else if (value is true)
                {
                    state |= Automatic;

                    if (IsTriggered)
                    {
                        UpdateOutcome();
                    }
                }
            }
        }

        #endregion

        
        #region Instance Methods

        protected bool React()
        {
            if (IsReacting) { Debug.Fail($"Update loop in {nameof(ReactorCore)} => {this}."); }
            
            bool outcomeChanged;

            IsReacting  = true;
            IsUnstable  = false;
            IsTriggered = false;
            
            try
            {
                outcomeChanged = CreateOutcome();
            }
            catch (Exception e)
            {
                //- TODO : Consider storing exceptions as an accessible field,
                //         similar to some of the reactives available in other libraries.
                
                // InvalidateOutcome(null);
                throw;
            }
            finally
            {
                IsReacting = false;
            }

            return outcomeChanged;
        }

        protected abstract bool CreateOutcome();
        
        public bool AttemptReaction()
        {
            if (IsTriggered)
            {
                return React();
            }
            else if (IsUnstable)
            {
                if (TryStabilizeOutcome())
                {
                    Debug.Assert(IsTriggered is false);

                    return false;
                }
                else
                {
                    return React();
                }
            }
            else return false;
        }

        protected bool TryStabilizeOutcome()
        {
            bool successfullyReconciled = true;
            
            foreach (var determinant in Triggers)
            {
                if (determinant.Reconcile() is false)
                {
                    successfullyReconciled = false;
                    Debug.Assert(IsTriggered);
                    //^ If the reconciliation failed then the trigger should have reacted and then
                    //  triggered us since we're one of its subscribers.

                    break;
                    //- No point in stabilizing the rest.  We try to stabilize to avoid updating, and if one
                    //  of our triggers fails to stabilize then we have no choice but to update.  In addition
                    //  we don't even know if we'll use the same triggers when we update. If we do end
                    //  up accessing the same triggers, they'll try to stabilize themselves anyways when we access
                    //  them.
                }
            }
            
            IsUnstable = false;
            //- Should we be changing this?  Should it be set by GenerateOutcome?
            //  Should it only happen if we successfully reconcile?
            //  What if we're destabilized while generating our outcome?  This may mark us as stable, even if we aren't.

            return successfullyReconciled;
            
            //- What if this is called during the update process of a Reactor we depend on?  
            //- Could that cause the Reactor to update twice if the timing was right?
        }
        
        public bool ForceReaction() => React();

        public bool Trigger() => Trigger(null, TriggerFlags.Default, out _);

        public bool Trigger(IFactor triggeringFactor, long triggerFlags, out bool removeSubscription)
        {
            removeSubscription = false;  
            
            if (IsReacting)
            {
                Settings.Logging.Notify_ReactorTriggeredWhileUpdating(this, triggeringFactor);

                //- If this Outcome is in the update list we should know it's a loop, if it's not then it should be
                //  another thread accessing it.
                //  Well actually, the parent won't add us to the list until this returns...
                //  Don't we add ourselves now?
            }
            
            if (IsTriggered is false)
            {
                IsTriggered = true;
                Debug.Assert(IsQueued is false);

                if (AutomaticallyReacts)
                {
                    UpdateOutcome();
                }

                return true;
            }
    
            return false;
        }

        //- Does not imply the caller will queue this subscriber to be updated.  Only that the this subscriber
        //  should mark itself and its dependents as Unstable, return whether it is Necessary or not.
        public bool Destabilize()
        {
            IsUnstable = true;
            return false;
        }

        protected bool AddTrigger(IFactor trigger, bool necessary) => trigger.Subscribe(this, necessary);
        protected void RemoveTrigger(IFactor trigger)              => trigger.Unsubscribe(this);
        
        protected virtual void UpdateOutcome()
        {
            if (IsQueued is false)
            {
                IsQueued = true;
                UpdateList.Update(this);
                //- Make sure there isn't a situation where a method calls this 
                //  but should have reacted immediately, instead of queuing.
            }
        }

        public void Dispose()
        {
            foreach (var trigger in Triggers)
            {
                RemoveTrigger(trigger);
            }
        }
        
        #endregion


        #region Constructors

        protected Reflex()
        {
            state |= Automatic;
        }

        #endregion

        
        #region Explicit Implementations

        void IUpdateable.Update()
        {
            IsQueued = false;
            AttemptReaction();
        }

        #endregion
    }
}