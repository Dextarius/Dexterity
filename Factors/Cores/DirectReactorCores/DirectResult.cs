using System.Collections.Generic;
using Core.Factors;
using Core.States;

namespace Factors.Cores.DirectReactorCores
{
    public abstract class DirectResult<TValue> : DirectReactorCore, IResult<TValue>
    {
        #region Instance Fields

        protected readonly IEqualityComparer<TValue>  valueComparer;
        protected          TValue                     currentValue;
        protected          ModifierCollection<TValue> modifiers;

        #endregion

        
        #region Properties

        public TValue Value
        {
            get
            {
                AttemptReaction();
                return currentValue;
            }
        }

        public IModifierCollection<TValue> Modifiers => modifiers ??= CreateModifierCollection<TValue>();

        #endregion


        #region Instance Methods
        
        protected override long CreateOutcome()
        {
            TValue oldValue = currentValue;
            TValue newValue = GenerateValue();
            
            //- TODO : What if the input is somehow invalidated/changed during GenerateValue()?
            SubscribeToInputs();
            
            if (modifiers?.Count > 0)
            {
                newValue = modifiers.Modify(newValue);
            }

            if (ValuesAreDifferent(oldValue, newValue, out var triggerFlags))
            {
                currentValue = newValue;
            }

            return triggerFlags;
        }

        public bool ValueEquals(TValue valueToCompare) => ValuesAreDifferent(currentValue, valueToCompare, out _) is false;
        
        public TValue Peek() => currentValue;

        protected abstract TValue GenerateValue();
        
        //v This method is duplicated in ObservedResult.
        public ModifierCollection<TValue> ReplaceModifierCollection(ModifierCollection<TValue> newCollection)
        {
            var oldCollection = modifiers;

            if (oldCollection != null)
            {
                RemoveTrigger(oldCollection);
            }

            modifiers = newCollection;
            AddTrigger(newCollection, IsReflexive);
            return oldCollection;
        }
        
        protected virtual bool ValuesAreDifferent(TValue first, TValue second, out long triggerFlags)
        {
            if (valueComparer.Equals(first, second))
            {
                triggerFlags = TriggerFlags.None;
                return false;
            }
            else
            {
                triggerFlags = TriggerFlags.Default;
                return true;
            }
        }

        #endregion


        #region Constructors
        
        protected DirectResult(IEqualityComparer<TValue> comparer = null) 
        {
            valueComparer = comparer ?? EqualityComparer<TValue>.Default;
        }

        #endregion
        
    }
}