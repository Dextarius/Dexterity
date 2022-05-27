using System.Collections.Generic;
using Core.Factors;
using Core.States;

namespace Factors.Cores.DirectReactorCores
{
    public abstract class DirectResult<TValue> : DirectReactorCore, IResult<TValue>
    {
        #region Instance Fields

        protected readonly IEqualityComparer<TValue> valueComparer;
        protected          TValue                    currentValue;

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

        #endregion


        #region Instance Methods

        protected override bool CreateOutcome()
        {
            TValue oldValue = currentValue;
            TValue newValue = GenerateValue();

            SubscribeToInputs();
            //- TODO : What if the input is somehow invalidated/changed during GenerateValue()?

            if (valueComparer.Equals(oldValue, newValue))
            {
                return false;
            }
            else
            {
                currentValue = newValue;
                return true;
            }
        }
        
        public bool ValueEquals(TValue valueToCompare) => valueComparer.Equals(currentValue, valueToCompare);
        
        public TValue Peek() => currentValue;
        
        protected abstract TValue GenerateValue();
        
        #endregion


        #region Constructors
        
        protected DirectResult(IEqualityComparer<TValue> comparer = null) 
        {
            valueComparer = comparer ?? EqualityComparer<TValue>.Default;
        }

        #endregion
        
    }
}