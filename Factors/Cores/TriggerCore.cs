using System;
using System.Collections.Generic;
using System.Diagnostics;
using Core;
using Core.Factors;
using Core.States;

namespace Factors.Cores
{
    public abstract class TriggerCore : FactorCore, IFactorSubscriber
    {
        #region Instance Fields

        protected readonly IReactorSubscriber subscriber;

        #endregion

        
        #region Static Properties

        public static UpdateList UpdateList => Influence.UpdateList;

        #endregion
        

        #region Instance Properties

        public             ITriggerCoreCallback Callback         { get; protected set; }
        public             bool                 IsStabilizing    { get; protected set; }
        protected abstract IEnumerable<IFactor> Triggers         { get; }
        public    abstract bool                 HasTriggers      { get; }
        public    abstract int                  NumberOfTriggers { get; }
        
        
        public bool IsUnstable {           get => subscriber.IsUnstable;
                                 protected set => subscriber.IsUnstable = value; }
        public bool IsTriggered {           get => subscriber.IsTriggered;
                                  protected set => subscriber.IsTriggered = value; }
        
        public bool IsNecessary {          get => subscriber.IsNecessary;
                                 protected set => subscriber.IsNecessary = value; }


        
        #endregion

        
        #region Instance Methods
        
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
            IsStabilizing = false;

            return successfullyReconciled;
            
            //- What if this is called during the update process of a Reactor we depend on?  
            //- Could that cause the Reactor to update twice if the timing was right?
        }
        

        public bool Trigger(IFactor triggeringFactor, long triggerFlags, out bool removeSubscription)
        {
            removeSubscription = false; //- Remove this

            if (IsTriggered is false)
            {
                IsTriggered = true;
                Callback.CoreTriggered(this);
                return true;
            }
    
            return false;
        }
        
        public bool Trigger() => Trigger(null, TriggerFlags.Default, out _);

        //- Does not imply the caller will queue this subscriber to be updated.  Only that the this subscriber
        //  should mark itself and its dependents as Unstable, return whether it is Necessary or not.
        public bool Destabilize()
        {
            if (IsNecessary)
            {
                return true;
                
                //- We shouldn't need to mark ourselves as Unstable. If we're Necessary then either
                //  we're going to be triggered when our parent updates, or the parent won't
                //  change, in which case we aren't Unstable.
            }
            else if (IsUnstable is false)
            {
                if (Callback != null && 
                    Callback.CoreDestabilized(this))
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

        public void Reset()
        {
            IsUnstable  = false;
            IsTriggered = false;
        }
        
        public void OnNecessary()
        {
            subscriber.IsNecessary = true;
        }
        
        public void OnNotNecessary()
        {
            if (subscriber.IsNecessary)
            {
                subscriber.IsNecessary = false;

                foreach (var trigger in Triggers)
                {
                    trigger.NotifyNotNecessary(this);
                }
            }
        }

        protected bool AddTrigger(IFactor trigger, bool necessary) => trigger.Subscribe(subscriber, necessary);
        protected void RemoveTrigger(IFactor trigger)              => trigger.Unsubscribe(subscriber);

        // public override bool Reconcile()
        // {
        //     if (IsTriggered)
        //     {
        //         Reset();
        //         return false;
        //     }
        //     else if (IsUnstable)
        //     {
        //         if (TryStabilizeOutcome())
        //         {
        //             IsUnstable = false;
        //             return true;
        //         }
        //         else
        //         {
        //             Callback.CoreUpdated(this);
        //         } 
        //     }
        //     else return true;
        // }

        public void SetCallback(ITriggerCoreCallback newCallback)
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

        protected void EvaluateUpdatePriority()
        {
            //- Add this to AddTrigger()?
        }

        #endregion


        #region Constructors

        protected TriggerCore(IReactorSubscriber subscriberToUse = null)
        {
            IsTriggered = false;
            subscriber  = subscriberToUse ?? new WeakSubscriber(this);
        }

        #endregion

        //- We're going to have to add something to the ReactorCores that causes them to
        //  periodically check their UpdatePriority.  Maybe we can add an
        //  UpdatePriorityChanged() method to the IFactorSubscriber interface.
    }
    

    public interface ITriggerCoreCallback
    {
        bool CoreUpdated(TriggerCore triggerCore);
        bool CoreDestabilized(TriggerCore triggerCore);
        void CoreTriggered(TriggerCore triggerCore);
    }
}