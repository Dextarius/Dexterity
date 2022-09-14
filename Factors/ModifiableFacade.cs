using System;
using Core.Factors;
using Core.States;

namespace Factors
{
    public class Modified<T> : IProactive<T>, IReactive<T>, IModifiedProactive<T>, IModifiedFactor<T>
    {
        private readonly Proactive<T> baseValue;
        private readonly Reactive<T>  modifiedValue;

        public string                 Name                => modifiedValue.Name;
        public int                    UpdatePriority      => modifiedValue.UpdatePriority;
        public uint                   VersionNumber       => modifiedValue.VersionNumber;
        public bool                   IsNecessary         => baseValue.IsNecessary    || modifiedValue.IsNecessary;
        public bool                   HasSubscribers      => baseValue.HasSubscribers || modifiedValue.HasSubscribers;
        public int                    NumberOfSubscribers => baseValue.NumberOfSubscribers + modifiedValue.NumberOfSubscribers;
        public bool                   IsUnstable          => modifiedValue.IsUnstable;
        public bool                   IsReacting          => modifiedValue.IsReacting;
        public bool                   IsStabilizing       => modifiedValue.IsStabilizing;
        public bool                   HasReacted          => modifiedValue.HasReacted;
        public bool                   HasTriggers         => modifiedValue.HasTriggers;
        public bool                   IsTriggered         => modifiedValue.IsTriggered;
        public int                    NumberOfTriggers    => modifiedValue.NumberOfTriggers;
        public IModifierCollection<T> Modifiers           => modifiedValue.Modifiers;

        public bool IsReflexive
        {
            get => modifiedValue.IsReflexive;
            set => modifiedValue.IsReflexive = value;
        }

        public bool AutomaticallyReacts
        {
            get => modifiedValue.AutomaticallyReacts;
            set => modifiedValue.AutomaticallyReacts = value;
        }

        public T Value
        {
            get => modifiedValue.Value;
            set => baseValue.Value = value;
        }
        
        public IProactive<T> BaseValue => baseValue;

        public bool Subscribe(IFactorSubscriber subscriberToAdd, bool isNecessary)                    => modifiedValue.Subscribe(subscriberToAdd, isNecessary);
        public void Unsubscribe(IFactorSubscriber subscriberToRemove)                                 => modifiedValue.Unsubscribe(subscriberToRemove);
        public void NotifyNecessary(IFactorSubscriber necessarySubscriber)                            => modifiedValue.NotifyNecessary(necessarySubscriber);
        public void NotifyNotNecessary(IFactorSubscriber unnecessarySubscriber)                       => modifiedValue.NotifyNotNecessary(unnecessarySubscriber);
        public bool Reconcile()                                                                       => modifiedValue.Reconcile();
        public bool ValueEquals(T valueToCompare)                                                     => modifiedValue.ValueEquals(valueToCompare);
        public void TriggerSubscribers()                                                              => modifiedValue.TriggerSubscribers();
        public bool Trigger()                                                                         => modifiedValue.Trigger();
        public bool Trigger(IFactor triggeringFactor, long triggerFlags, out bool removeSubscription) => modifiedValue.Trigger(triggeringFactor, triggerFlags, out removeSubscription);
        public bool Destabilize()                                                                     => modifiedValue.Destabilize();
        public bool AttemptReaction()                                                                 => modifiedValue.AttemptReaction();
        public bool ForceReaction()                                                                   => modifiedValue.ForceReaction();
        public T    Peek()                                                                            => modifiedValue.Peek();


        #region Operators

        public static implicit operator T(Modified<T> modified) => modified.Value;

        #endregion
        
        public Modified(Proactive<T> factorToUseAsBaseValue)
        {
            baseValue     = factorToUseAsBaseValue  ??  throw new ArgumentNullException(nameof(factorToUseAsBaseValue));
            modifiedValue = new Reactive<T>(baseValue);
        }

        public Modified(T initialValue) : this(new Proactive<T>(initialValue))
        {
            
        }
        
        // public Modified(IResult<T> result)
        // {
        //     baseValue     = factorToUseAsBaseValue  ??  throw new ArgumentNullException(nameof(factorToUseAsBaseValue));
        //     modifiedValue = new Reactive<T>(result);
        // }
        
        
        IFactor<T> IModifiedFactor<T>.BaseValue => BaseValue;
    }
}