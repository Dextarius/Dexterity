using System;
using System.Collections.Generic;
using Core.Factors;
using Core.States;
using Factors.Cores.DirectReactorCores;
using Factors.Observer;

namespace Factors.Cores
{
    public class InteractiveCore<T> : DirectReactorCore, IInteractiveCore<T>
    {
        #region Constants

        private const string DefaultName = "Interactive";

        #endregion
        
        
        #region Instance Fields

        protected readonly List<IFactorModifier<T>> modifiers = new List<IFactorModifier<T>>();
        protected readonly IEqualityComparer<T>     valueComparer;
        protected          T                        modifiedValue;
        protected          T                        baseValue;
        private            int                      updatePriority;

        #endregion


        #region Instance Properties

        public override int  UpdatePriority   => updatePriority;
        public override bool HasTriggers      => modifiers.Count > 0;
        public override int  NumberOfTriggers => modifiers.Count;

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
                bool baseValueIsDifferent = valueComparer.Equals(value, baseValue) is false;
                
                if (baseValueIsDifferent)
                {
                    baseValue = value;
                    Trigger();
                }
            }
        }

        protected override IEnumerable<IFactor> Triggers => modifiers;

        #endregion


        #region Instance Methods

        public void AddModifier(IFactorModifier<T> modifierToAdd)
        {
            if (modifierToAdd is null) { throw new ArgumentNullException(); }

            if (modifiers.Count == 0)
            {
                modifiers.Add(modifierToAdd);
            }
            else
            {
                int indexOfElementWithSamePriority = modifiers.BinarySearch(modifierToAdd); // Find correct index for UpdatePriority

                if (indexOfElementWithSamePriority > -1)
                {
                    modifiers.Insert(indexOfElementWithSamePriority + 1, modifierToAdd);
                }
                else
                {
                    int indexOfElementWithLowerPriority = ~indexOfElementWithSamePriority;
                    
                    modifiers.Insert(indexOfElementWithLowerPriority, modifierToAdd);
                }
                
                //- TODO : Make sure you got the index math right, we were tired.

                AddTrigger(modifierToAdd, IsNecessary); //- Should we use IsNecessary?
                Trigger();
            }
        }

        public void RemoveModifier(IFactorModifier<T> modifierToRemove)
        {
            if (modifierToRemove != null &&
                modifiers.Remove(modifierToRemove))
            {
                RemoveTrigger(modifierToRemove);
                Trigger();  
            }
        }

        public bool ContainsModifier(IFactorModifier<T> modifierToFind)
        {
            return modifiers.Contains(modifierToFind);
        }

        protected override bool CreateOutcome()
        {
            T    oldModifiedValue    = modifiedValue;
            T    newModifiedValue    = ApplyModifiers(baseValue);
            bool newValueIsDifferent = valueComparer.Equals(oldModifiedValue, newModifiedValue) is false;

            if (newValueIsDifferent)
            {
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

                if (modifier.UpdatePriority >= updatePriority)
                {
                    updatePriority = modifier.UpdatePriority + 1;
                }
            }

            return newModifiedValue;
        }

        public string PrintBaseValueAndModifiers()
        {
            string result = $"Base Value = {baseValue}";

            foreach (var modifier in modifiers)
            {
                result += $" | {modifier.Description} |";
            }

            return result;
        }


        #endregion


        #region Constructors
        
        public InteractiveCore(T initialBaseValue , IEqualityComparer<T> equalityComparer = null) 
        {
            valueComparer = equalityComparer ?? EqualityComparer<T>.Default;
            baseValue     = initialBaseValue;
        }


        #endregion
    }
}