using System.Collections.Generic;
using Core.Causality;
using Core.Factors;
using Core.States;

namespace Factors.Cores.ObservedReactorCores
{
    public abstract class ObservedResult<TValue> : ObservedReactorCore, IResult<TValue>, IProcess<TValue>
    {
        #region Instance Fields

        protected readonly IEqualityComparer<TValue>  valueComparer;
        protected          TValue                     currentValue;
        private            ModifierCollection<TValue> modifiers;

        #endregion

        
        #region Properties

        public TValue Value
        {
            get
            {
                NotifyInvolved();
                return currentValue;
            }
        }
        
        public IModifierCollection<TValue> Modifiers => modifiers ??= CreateModifierCollection<TValue>();
        
        #endregion


        #region Instance Methods

        protected override long CreateOutcome()
        {
            TValue oldValue = currentValue;
            TValue newValue = Observer.ObserveInteractions<ObservedResult<TValue>, TValue>(this);
            
            using (Observer.PauseObservation()) //- Prevents us from adding dependencies to any other observations this
            {                                   //  one might be nested inside of.     
                RemoveUnusedTriggers();

                if (ValuesAreDifferent(oldValue, newValue, out var triggerFlags))
                {
                    currentValue = newValue;
                }

                return triggerFlags;
            }
        }

        public bool ValueEquals(TValue valueToCompare) => valueComparer.Equals(currentValue, valueToCompare);

        public TValue Peek() => currentValue;
        
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

        protected abstract TValue GenerateValue();

        //- Should we support a set of events that notifies subscribers that the value is about to change,
        //  so they can alter/stop it?
        //protected virtual void OnValueChanging(ValueChangedEventArgs e)
        
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

        #endregion


        #region Constructors

        protected ObservedResult(IEqualityComparer<TValue> comparer = null) 
        {
            valueComparer = comparer ?? EqualityComparer<TValue>.Default;
        }

        #endregion

        
        #region Explicit Implementations

        TValue IProcess<TValue>.Execute()
        {
            var generatedValue = GenerateValue();

            if (modifiers?.Count > 0)
            {
                return modifiers.Modify(generatedValue);
            }
            else
            {
                return generatedValue;
            }
        }

        #endregion
    }
}