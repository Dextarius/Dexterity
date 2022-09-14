using Factors.Cores.DirectReactorCores;

namespace Factors.Cores
{
    public abstract class Result<TValue> : ReactorCore
    {
        #region Instance Fields

        protected TValue currentValue;

        #endregion
        
        public virtual TValue Value
        {
            get
            {
                AttemptReaction();
                return currentValue;
            }
        }

        protected override long CreateOutcome()
        {
            TValue oldValue = currentValue;
            TValue newValue = GenerateValue();
            
            //- TODO : What if the input is somehow invalidated/changed during GenerateValue()?
            // SubscribeToInputs();
            
            // if (modifiers?.Count > 0)
            // {
            //     newValue = modifiers.Modify(newValue);
            // }

            if (ValuesAreDifferent(oldValue, newValue, out var triggerFlags))
            {
                currentValue = newValue;
            }

            return triggerFlags;
        }
        
        public bool ValueEquals(TValue valueToCompare) => ValuesAreDifferent(currentValue, valueToCompare, out _) is false;


        public    abstract bool   ValuesAreDifferent(TValue first, TValue second, out long triggerFlags);
        protected abstract TValue GenerateValue();
    }
}