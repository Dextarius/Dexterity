using System;
using System.Collections.Generic;
using Core.Factors;
using Core.States;

namespace Factors.Cores
{
    public class WeakSubscriber : IReactorSubscriber
    {
        #region Instance Fields

        private readonly IFactorSubscriber                subscriber;
        private readonly WeakReference<IFactorSubscriber> weakReferenceToSubscriber;
        private          int                              interactionCount;

        #endregion

        
        #region Properties

        public bool IsNecessary { get; set; }
        public bool IsUnstable  { get; set; }
        public bool IsTriggered { get; set; } = true;
        
      //  public bool UsesTriggerFlags.Default { get; set; } = true;

        #endregion


        #region Instance Methods

        public bool Trigger() => Trigger(null, TriggerFlags.Default, out _);

        public bool Trigger(IFactor triggeringFactor, long triggerFlags, out bool removeSubscription)
        {
            if (IsTriggered)
            {
                if (subscriber == null)
                {
                    EnsureReferenceIsStillValid(out removeSubscription);
                }
                else
                {
                    removeSubscription = false;
                }

                return false;
            }
            else if (subscriber != null)
            {
                return subscriber.Trigger(triggeringFactor, triggerFlags, out removeSubscription);
            }
            else if (weakReferenceToSubscriber.TryGetTarget(out var weakSubscriber))
            {
                return weakSubscriber.Trigger(triggeringFactor, triggerFlags, out removeSubscription);
            }
            else
            {
                removeSubscription = true;
                return false;
            }
        }

        private bool EnsureReferenceIsStillValid(out bool removeSubscription)
        {
            if (subscriber != null)
            {
                removeSubscription = false;
                return true;
            }
            else if (interactionCount >= 10)
            {
                if (weakReferenceToSubscriber.TryGetTarget(out var target))
                {
                    interactionCount = 0;
                    removeSubscription = false;
                    return true;
                }
                else
                {
                    removeSubscription = true;
                    return false;
                }
            }
            else
            {
                removeSubscription = false;
                interactionCount++;
                return true;
            }
        }
        
        public bool Destabilize()
        {
            if (IsNecessary)
            {
                return EnsureReferenceIsStillValid(out bool _);
            }
            else if (IsUnstable)
            {
                EnsureReferenceIsStillValid(out bool _);
                return false;
            }
            else
            {
                return ForwardDestabilizeToOwner();
            }
            
            //- Since we can't unsubscribe during this method, if we find out the WeakReference
            //  is no longer active should we mark that in a field?
        }
        
        protected bool ForwardDestabilizeToOwner()
        {
            if (subscriber != null)
            {
                return subscriber.Destabilize();
            }
            else
            {
                if (weakReferenceToSubscriber.TryGetTarget(out var weakSubscriber))
                {
                    return weakSubscriber.Destabilize();
                }
                else
                {
                    return false;
                }
            }
        }

        #endregion

        public WeakSubscriber(IFactorSubscriber subscriberToCall, bool necessary = false)
        {
            weakReferenceToSubscriber = new WeakReference<IFactorSubscriber>(subscriberToCall);

            if (necessary)
            {
                subscriber  = subscriberToCall;
                IsNecessary = true;
            }
        }
    }
}