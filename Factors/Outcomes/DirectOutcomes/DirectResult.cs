using System;
using System.Collections.Generic;
using Core.Factors;
using Core.States;
using Core.Tools;

namespace Factors.Outcomes.DirectOutcomes
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
        
        protected override void InvalidateOutcome(IFactor changedParentState) { }
        
        #endregion


        #region Constructors
        
        protected DirectResult(string name, IEqualityComparer<TValue> comparer = null) : base(name)
        {
            valueComparer = comparer ?? EqualityComparer<TValue>.Default;
        }

        #endregion
    }
}