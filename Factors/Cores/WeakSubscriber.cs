using System;
using System.Collections.Generic;
using Core.Factors;
using Core.States;

namespace Factors.Cores
{
    public class WeakSubscriber : IFactorSubscriber
    {
        #region Instance Fields

        private readonly IFactorSubscriber                subscriber;
        private readonly WeakReference<IFactorSubscriber> weakReferenceToSubscriber;
        private          int                              interactionCount;

        #endregion

        
        #region Properties

        public bool IsNecessary      { get; set; }
        public bool IsUnstable       { get; set; }
        public bool HasBeenTriggered { get; set; } = true;

        #endregion


        #region Instance Methods

        public bool Trigger() => Trigger(null, out _);

        public bool Trigger(IFactor triggeringFactor, out bool removeSubscription)
        {
            if (HasBeenTriggered)
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
                return subscriber.Trigger(triggeringFactor, out removeSubscription);
            }
            else if (weakReferenceToSubscriber.TryGetTarget(out var weakSubscriber))
            {
                return weakSubscriber.Trigger(triggeringFactor, out removeSubscription);
            }
            else
            {
                removeSubscription = true;
                return false;
            }

        }

        private bool EnsureReferenceIsStillValid( out bool removeSubscription)
        {
            if (subscriber != null)
            {
                removeSubscription = false;
                return true;
            }
            if (interactionCount >= 10)
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
        
        public bool Destabilize(IFactor factor)
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
                return ForwardDestabilizeToOwner(factor);
            }
            
            //- Since we can't unsubscribe during this method, if we find out the WeakReference
            //  is no longer active should we mark that in a field?
        }
        
        protected bool ForwardDestabilizeToOwner(IFactor factor)
        {
            if (subscriber != null)
            {
                return subscriber.Destabilize(factor);
            }
            else
            {
                if (weakReferenceToSubscriber.TryGetTarget(out var weakSubscriber))
                {
                    return weakSubscriber.Destabilize(factor);
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
                subscriber = subscriberToCall;
            }
        }
    }
}