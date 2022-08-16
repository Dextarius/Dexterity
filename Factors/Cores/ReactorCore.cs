using System;
using System.Collections.Generic;
using System.Diagnostics;
using Core;
using Core.Factors;
using Core.States;
using static Core.Tools.Types;
using static Core.Settings;

namespace Factors.Cores
{
    public abstract class ReactorCore : FactorCore, IReactorCore, IUpdateable
    {
        #region Instance Fields

        protected readonly WeakSubscriber weakSubscriber;
        private            bool           isReflexive;
        private            bool           automaticallyReacts;

        #endregion

        
        #region Static Properties

        public static UpdateList UpdateList => Influence.UpdateList;

        #endregion
        

        #region Instance Properties

        public             IReactorCoreCallback Callback         { get; protected set; }
        public             bool                 IsReacting       { get; protected set; }
        public             bool                 IsStabilizing    { get; protected set; }
        public             bool                 HasReacted       { get; protected set; }
        protected          bool                 IsQueued         { get;           set; }
        protected abstract IEnumerable<IFactor> Triggers         { get; }
        public    abstract bool                 HasTriggers      { get; }
        public    abstract int                  NumberOfTriggers { get; }
        
        
        public bool IsUnstable       {           get => weakSubscriber.IsUnstable;
                                       protected set => weakSubscriber.IsUnstable = value; }
        public bool IsTriggered {           get => weakSubscriber.IsTriggered;
                                  protected set => weakSubscriber.IsTriggered = value; }

        public bool AutomaticallyReacts
        {
            get => automaticallyReacts;
            set
            {
                if (automaticallyReacts != value)
                {
                    automaticallyReacts = value;

                    if (value is true)
                    {
                        UpdateOutcome();
                        //- Should we wait if we haven't reacted yet?
                    }
                }
            }
        }

        public bool IsReflexive
        {
            get => isReflexive;
            set
            {
                if (value is true)
                {
                    if (isReflexive is false)
                    {
                        isReflexive = true;
                        OnNecessary();
                    }
                }
                else if (isReflexive is true)
                {
                    isReflexive = false;
                    OnNotNecessary();
                }
            }
        }
        
        #endregion

        
        #region Instance Methods

        protected bool React()
        {
            if (IsReacting)           { Debug.Fail($"Update loop in {NameOf<ReactorCore>()} => {this}."); }
            if (IsTriggered is false) { InvalidateOutcome(null); }
            
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
            
            if (HasReacted is false)
            {
                HasReacted = true;
            }
            
            if (outcomeChanged)
            {
                Callback?.CoreUpdated(this);
                return true;
            }
            else return false;
        }

        protected abstract void InvalidateOutcome(IFactor changedFactor);
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

        public bool TryStabilizeOutcome()
        {
            bool successfullyReconciled = true;
            
            IsStabilizing = true;

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
            IsStabilizing = false;

            return successfullyReconciled;
            
            //- What if this is called during the update process of a Reactor we depend on?  
            //- Could that cause the Reactor to update twice if the timing was right?
        }
        
        public bool ForceReaction() => React();

        public bool Trigger() => Trigger(null, out _);

        public bool Trigger(IFactor triggeringFactor, out bool removeSubscription)
        {
            removeSubscription = false;  //- Remove this
            
            if (IsReacting)
            {
                Logging.Notify_ReactorTriggeredWhileUpdating(this, triggeringFactor);

                //- If this Outcome is in the update list we should know it's a loop, if it's not then it should be
                //  another thread accessing it.
                //  Well actually, the parent won't add us to the list until this returns...
                //  Don't we add ourselves now?
            }
            
            if (IsTriggered is false)
            {
                IsTriggered = true;
                InvalidateOutcome(triggeringFactor);
                Debug.Assert(IsQueued is false);

                if (IsReflexive || 
                    AutomaticallyReacts ||
                    Callback != null && Callback.ReactorTriggered(this))
                {
                    UpdateOutcome();
                }

                return true;
            }
    
            return false;
        }

        //- Does not imply the caller will queue this subscriber to be updated.  Only that the this subscriber
        //  should mark itself and its dependents as Unstable, return whether it is Necessary or not.
        public bool Destabilize(IFactor factor)
        {
            if (IsReflexive)
            {
                return true;
                
                //- We shouldn't need to mark ourselves as Unstable. If we're Necessary then either
                //  we're going to be triggered when our parent updates, or the parent won't
                //  change, in which case we aren't Unstable.
            }
            else if (IsUnstable is false)
            {
                if (Callback != null && 
                    Callback.ReactorDestabilized(this))
                {
                    return true;
                }
                else
                {
                    IsUnstable = true;
                }
            }
    
            return false;
            
            //- Note : This method used to check IsStabilizing and IsTriggered, but I removed that because it
            //         it no longer seemed relevant.  When we have time, go through all the possible ways this might
            //         called and make sure I'm not missing something.
        }
        
        protected void OnNecessary()
        {
            weakSubscriber.IsNecessary = true;

            //- We don't propagate the Necessary status to our triggers, to avoid walking up the tree.
            //  All Reactors walk down the tree when they trigger/destabilize their dependents, so we
            //  can let them know then if need be.
            
            if (IsTriggered || IsUnstable)
            {
                UpdateOutcome();
                //^ We use UpdateOutcome() instead of calling AttemptReaction() here, since this might
                //  be called as a response to us destabilizing a necessary subscriber, and I'm not sure
                //  if we want to start a reaction in the middle of that process.  UpdateOutcome() will at
                //  least queue the reaction. Either way might lead to this reactor updating multiple times,
                //  so I'll need to think about it more later.
                //- TODO : This seems like it could lead to some weird behavior when combined with the fact that Reactives
                //         already update before returning a value. Think about it for a bit when you get a chance.
                //- TODO : We could probably skip checking IsTriggered and IsUnstable, and just call AttemptReaction().
                //         AttemptReaction checks them anyways, and won't do anything if they're both false. 
            }
            
            //- TODO : Should we check if we're already reacting, or if we're queued?
        }
        
        protected void OnNotNecessary()
        {
            weakSubscriber.IsNecessary = false;

            if (HasReacted)
            {
                foreach (var trigger in Triggers)
                {
                    trigger.NotifyNotNecessary(this);
                }
            }
            
            //- Note : This may not work as intended if Observed cores continue to pass themselves as triggers,
            //         because the core won't be able to tell its owner when its dependents notify it that it's
            //         not necessary.
        }

        protected bool AddTrigger(IFactor trigger, bool necessary) => trigger.Subscribe(weakSubscriber, necessary);
        protected void RemoveTrigger(IFactor trigger)              => trigger.Unsubscribe(weakSubscriber);

        public override bool Reconcile()
        {
            return AttemptReaction() is false;

            //- This method is intended to be used by factors that have been made unstable.
            //  If the caller is unstable, then some Factor (us or another Reactor) was either triggered or made unstable,
            // and in turn made them unstable. If we are no longer triggered or unstable then we reacted
            // before this call, and we would have triggered our subscribers if the reaction 'changed something'.
            // Since they're trying to reconcile, then they haven't been triggered, so we accept the reconciliation.
        }
        
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

        public void SetCallback(IReactorCoreCallback newCallback)
        {
            Callback = newCallback ?? throw new ArgumentNullException(nameof(newCallback));
        }

        public override void Dispose()
        {
            Callback = null;

            foreach (var trigger in Triggers)
            {
                RemoveTrigger(trigger);
            }
        }

        #endregion


        #region Constructors

        protected ReactorCore()
        {
            weakSubscriber = new WeakSubscriber(this);
            IsTriggered    = true;
        }

        #endregion

        
        #region Explicit Implementations

        void IUpdateable.Update()
        {
            IsQueued = false;
            AttemptReaction();
        }

        #endregion
        
        //- We're going to have to add something to the ReactorCores that causes them to
        //  periodically check their UpdatePriority.  Maybe we can add an
        //  UpdatePriorityChanged() method to the IFactorSubscriber interface.
    }
}