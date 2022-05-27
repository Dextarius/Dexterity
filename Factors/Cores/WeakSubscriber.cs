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
            if (HasBeenTriggered is false)
            {
                if (subscriber != null)
                {
                    return subscriber.Trigger(triggeringFactor, out removeSubscription);
                }
                else
                {
                    if (weakReferenceToSubscriber.TryGetTarget(out var weakSubscriber))
                    {
                        return weakSubscriber.Trigger(triggeringFactor, out removeSubscription);
                    }
                    else
                    {
                        removeSubscription = true;
                        return false;
                    }
                }
            }
            else if (subscriber == null)
            {
                if (EnsureReferenceIsStillValid())
                {
                    interactionCount++;
                    removeSubscription = false;
                }
                else
                {
                    removeSubscription = true;
                }
                
                return false;
            }
            else
            {
                removeSubscription = false;
                return false;
            }
        }

        private bool EnsureReferenceIsStillValid()
        {
            if (interactionCount >= 10)
            {
                if (weakReferenceToSubscriber.TryGetTarget(out var target))
                {
                    interactionCount = 0;
                    return true;
                }
                else return false;
            }
            else return true;
        }

        public bool Destabilize(IFactor factor)
        {
            if (IsUnstable is false)
            {
                if (IsNecessary)
                {
                    if (subscriber != null) return true;
                    
                    if (EnsureReferenceIsStillValid())
                    {
                        interactionCount++;
                    }
                    else
                    {
                        factor.Unsubscribe(this);
                        return false;
                    }

                    return true;
                }
                else if (subscriber != null)
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
                        factor.Unsubscribe(this);
                        return false;
                    }
                }
            }
            else if (subscriber == null)
            {
                if (EnsureReferenceIsStillValid())
                {
                    interactionCount++;
                }
                
                return false;
            }
            else
            {
                factor.Unsubscribe(this);
                return false;
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