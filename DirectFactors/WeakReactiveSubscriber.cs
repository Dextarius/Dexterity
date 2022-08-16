using Core.Factors;
using Core.States;

namespace DirectFactors
{
    public class WeakSubscriber<T> : IFactorSubscriber<T>
    {
        #region Instance Fields

        private readonly IFactorSubscriber<T>                subscriber;
        private readonly WeakReference<IFactorSubscriber<T>> weakReferenceToSubscriber;
        private          int                                 interactionCount;

        #endregion

        
        #region Properties

        public bool IsNecessary      { get; set; }
        public bool IsUnstable       { get; set; }
        public bool HasBeenTriggered { get; set; } = true;

        #endregion


        #region Instance Methods

        public void ValueChanged(IDirectFactor<T> factor, T oldValue, T newValue, out bool removeSubscription)
        {
            if (subscriber != null)
            {
                subscriber.ValueChanged(factor, oldValue, newValue, out removeSubscription);
            }
            else
            {
                if (weakReferenceToSubscriber.TryGetTarget(out var weakSubscriber))
                {
                    weakSubscriber.ValueChanged(factor, oldValue, newValue, out removeSubscription);
                }
                else
                {
                    removeSubscription = true;
                }
            }
        }

        private bool EnsureReferenceIsStillValid(out bool removeSubscription)
        {
            if (subscriber != null)
            {
                removeSubscription = false;
                return true;
            }
            if (interactionCount >= 10) // else?
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
        
        public bool Destabilize(IDirectFactor factor)
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
            
            //- TODO : Since we can't unsubscribe during this method, if we find out the WeakReference
            //  is no longer active should we mark that in a field?
        }
        
        protected bool ForwardDestabilizeToOwner(IDirectFactor factor)
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

        public WeakSubscriber(IFactorSubscriber<T> subscriberToCall, bool necessary = false)
        {
            weakReferenceToSubscriber = new WeakReference<IFactorSubscriber<T>>(subscriberToCall);

            if (necessary)
            {
                subscriber = subscriberToCall;
            }
        }
    }
}