namespace DirectFactors
{
    public abstract class Subscriber<T> : IFactorSubscriber<T>
    {
        private IDirectFactor<T> subscribedFactor;

        public T Value => subscribedFactor.Value;

        public bool Subscribe()          => subscribedFactor.Subscribe(this, IsNecessary);
        public void Unsubscribe()        => subscribedFactor.Unsubscribe(this);
        public void NotifyNecessary()    => subscribedFactor.NotifyNecessary(this);
        public void NotifyNotNecessary() => subscribedFactor.NotifyNotNecessary(this);

        public bool Destabilize(IDirectFactor factor)
        {
            
        }

        public abstract void ValueChanged(IDirectFactor<T> factor, T oldValue, T newValue, out bool removeSubscription);
    }
    
    public class BasicSubscriber<T> : Subscriber<T>
    {
        private SubscriptionList owner;

        public override void ValueChanged(IDirectFactor<T> factor, T oldValue, T newValue, out bool removeSubscription)
        {
            owner.ValueChanged(factor, oldValue, newValue, out removeSubscription);
        }
    }

    public abstract class SingleSubscriber<T> : Subscriber<T>
    {
    
    }

    public abstract class SubscriberCore
    {
        public bool IsNecessary      { get; set; }
        public bool IsUnstable       { get; set; }
        public bool HasBeenTriggered { get; set; } = true;

        public abstract void ValueChanged<T>(IDirectFactor<T> factor, T oldValue, T newValue, out bool removeSubscription);

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
    }
    
    
    public class BasicSubscriptionList
    {
        protected readonly IReactiveCore core;

        public bool IsNecessary      { get; set; }
        public bool IsUnstable       { get; set; }
        public bool HasBeenTriggered { get; set; } = true;

        public void UpdateOwner() => core.Update();

        public void ValueChanged<T>(IDirectFactor<T> factor, T oldValue, T newValue, out bool removeSubscription)
        {
            removeSubscription = false;
            Update();
        }

        protected void Update()
        {
            
        }
    }
}