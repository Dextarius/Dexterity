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
        
        public TValue Value
        {
            get
            {
                AttemptReaction();
                NotifyInvolved();
                return currentValue;
            }
        }


        #region Instance Methods

        protected override bool GenerateOutcome()
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
        
        public TValue Peek() => currentValue;
        
        protected abstract TValue GenerateValue();
        
        #endregion


        #region Constructors
        
        protected DirectResult(string name, IEqualityComparer<TValue> comparer = null) : base(name)
        {
            valueComparer = comparer ?? EqualityComparer<TValue>.Default;
        }

        #endregion
    }
}