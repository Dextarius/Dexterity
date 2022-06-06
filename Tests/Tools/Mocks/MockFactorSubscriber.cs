using System;
using Core.Factors;
using Core.States;

namespace Tests.Tools.Mocks
{
    public class MockFactorSubscriber : IFactorSubscriber
    {
        private WeakReference<IFactorSubscriber> weakReference;

        public WeakReference<IFactorSubscriber> WeakReference => weakReference ??= new WeakReference<IFactorSubscriber>(this);
        
        public bool IsNecessary                 { get; private set; }
        public bool HasBeenTriggered            { get; private set; }
        public bool IsUnstable                  { get; private set; }
        public bool RemoveSubscriptionOnTrigger { get; set; }
        


        public bool Trigger() => Trigger(null, out _);
        public bool Trigger(IFactor triggeringFactor, out bool removeSubscription)
        {
            removeSubscription = RemoveSubscriptionOnTrigger;
            
            if (HasBeenTriggered)
            {
                return false;
            }
            else
            {
                HasBeenTriggered = true;
                return true;
            }
        }
        
        public bool Destabilize(IFactor factor)
        {
            if (IsUnstable is false)
            {
                IsUnstable = true;
            }
            
            return IsNecessary;
        }

        public void ResetHasBeenTriggeredToFalse() => HasBeenTriggered = false;
        public void ResetIsUnstableToFalse()       => IsUnstable = false;
        public void MakeNecessary()                => IsNecessary = true;
        public void MakeUnnecessary()              => IsNecessary = false;

        public void NotifyNecessary()    => throw new NotSupportedException();
        public void NotifyNotNecessary() => throw new NotSupportedException();
    }
}