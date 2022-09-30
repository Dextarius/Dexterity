using System;
using System.Collections.Generic;
using Core.Factors;
using Core.States;

namespace Factors.Cores
{
    public class RelayTrigger : IFactor
    {
        protected IFactor ActualFactor { get; }


        public string Name           => ActualFactor.Name;
        public int    UpdatePriority => ActualFactor.UpdatePriority + 1;
        public uint   VersionNumber  => ActualFactor.VersionNumber;

        public bool Subscribe(IFactorSubscriber subscriberToAdd, bool isNecessary) =>
            ActualFactor.Subscribe(subscriberToAdd, isNecessary);

        public void Unsubscribe(IFactorSubscriber subscriberToRemove) =>
            ActualFactor.Unsubscribe(subscriberToRemove);

        public void NotifyNecessary(IFactorSubscriber necessarySubscriber) =>
            ActualFactor.NotifyNecessary(necessarySubscriber);

        public void NotifyNotNecessary(IFactorSubscriber unnecessarySubscriber) =>
            ActualFactor.NotifyNecessary(unnecessarySubscriber);


        public bool Reconcile() => ActualFactor.Reconcile();
    }

    
    
    public abstract class RelayTrigger<T> : RelayTrigger, IFactor<T>, IFactorSubscriber
    {
        protected readonly IEqualityComparer<T> comparer;
        private            T                    currentValue;
        protected          Influence            influence;


        public T Value => GetValue();

        public bool ValueEquals(T valueToCompare)
        {
            throw new System.NotImplementedException();
        }

        protected abstract T GetValue();

        public T Peek() => currentValue;


        public bool Trigger(IFactor triggeringFactor, long triggerFlags, out bool removeSubscription)
        {
            T newValue = GetValue();
            
            if (comparer.Equals(currentValue, newValue) is false)
            {
                currentValue = newValue;
                throw new NotImplementedException();
              //  TriggerSubscribers(triggerFlags);
            }

            removeSubscription = false;
            return true;
        }
        
        public bool Trigger() => Trigger(null, TriggerFlags.Default, out _);

        public bool Destabilize()
        {
            throw new System.NotImplementedException();
        }
        
        
        protected RelayTrigger(IEqualityComparer<T> comparerToUse)
        {
            comparer = comparerToUse;
        }
    }

}