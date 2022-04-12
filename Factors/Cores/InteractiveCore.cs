using System.Collections.Generic;
using Core.Factors;

namespace Factors.Cores
{
    public class ValueModifier<T> : ReactorCore
    {
        #region Instance Fields

        protected readonly List<IFactorModifier<T>> modifiers;
        protected readonly IEqualityComparer<T>     valueComparer;
        protected          T                        modifiedValue;
        protected          T                        baseValue;
        private            int                      priority;


        #endregion
        
        #region Static Properties

        protected static CausalObserver Observer => CausalObserver.ForThread;

        #endregion

        #region Instance Properties

        public    override int  Priority         { get; }
        public    override bool HasTriggers      { get; }
        public    override int  NumberOfTriggers { get; }
        

        public T Value
        {
            get
            {
                AttemptReaction();
                NotifyInvolved();
                
                return modifiedValue;
            }
        }

        public T BaseValue
        {
            get => baseValue;
            set
            {
                baseValue = value;
                GenerateModifiedValue(value);
            }
        }

        protected override IEnumerable<IXXX> Triggers
        {
            get
            {
                foreach (var factorModifier in modifiers)
                {
                    yield return factorModifier.ModifierChanged;
                }
            }
        }

        #endregion


        #region Instance Methods

        void AddModifier(IFactorModifier<T> modifierToAdd)
        {
            int modifierIndex          = ___; // Find correct index for Priority
            var modifierChangedTrigger = modifierToAdd.ModifierChanged;
            
            modifiers.Insert(modifierIndex, modifierToAdd);
            modifierToAdd.ModifierChanged.Subscribe(this, IsNecessary); //- Should we use IsNecessary?
        }

        void RemoveModifier(IFactorModifier<T> modifierToRemove)
        {
            if (modifiers.Remove(modifierToRemove))
            {
                modifierToRemove.ModifierChanged?.Unsubscribe(this); //- Should we use IsNecessary?
            }
        }

        bool ContainsModifier(IFactorModifier<T> modifierToFind)
        {
            
        }

        protected override bool GenerateOutcome()
        {
            T    oldModifiedValue    = modifiedValue;
            T    newModifiedValue    = ApplyModifiers(newBaseValue);
            bool newValueIsDifferent = valueComparer.Equals(oldModifiedValue, newModifiedValue) is false;

            if (newValueIsDifferent)
            {
                TriggerSubscribers();
                modifiedValue = newModifiedValue;

                return true;
            }
            else return false;
        }
        
        protected T ApplyModifiers(T newBaseValue)
        {
            T newModifiedValue = newBaseValue;

            foreach (var modifier in modifiers)
            {
                newModifiedValue = modifier.Modify(newModifiedValue);

                if (modifier.Priority >= priority)
                {
                    priority = modifier.Priority + 1;
                }
            }

            return newModifiedValue;
        }


        protected override void InvalidateOutcome(IFactor changedParentState) { }
        
        public void NotifyInvolved() => Observer.NotifyInvolved(this);

        //- TODO : Do we want NotifyChanged?
        
        #endregion


        #region Constructors

        public ValueModifier(string name) : base(name)
        {
            
        }

        #endregion
    }
}