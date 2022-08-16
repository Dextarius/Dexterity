using Factors.Cores.DirectReactorCores;

namespace Factors.Cores
{
    public abstract class ValueCore<TValue> : ReactorCore
    {
        
    }

    public abstract class Result<TValue> : ReactorCore
    {
        #region Instance Fields

        protected TValue currentValue;

        #endregion
        
        public virtual TValue Value => currentValue;

        protected override bool CreateOutcome()
        {
            
            TValue oldValue = currentValue;
            TValue newValue = GenerateValue();
            
         // SubscribeToInputs();
            //- TODO : What if the input is somehow invalidated/changed during GenerateValue()?

            if (ValuesAreDifferent(oldValue, newValue))
            {
                return false;
            }
            else
            {
                currentValue = newValue;
                return true;
            }
        }
        
        
        
        public bool ValueEquals(TValue valueToCompare) => ValuesAreDifferent(currentValue, valueToCompare) is false;


        public    abstract bool   ValuesAreDifferent(TValue value1, TValue value2);
        protected abstract TValue GenerateValue();
    }
}