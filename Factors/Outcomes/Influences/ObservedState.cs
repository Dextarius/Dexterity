using System.Collections.Generic;
using Core.Factors;
using Core.States;
using Factors.Observer;
using JetBrains.Annotations;

namespace Factors.Outcomes.Influences
{
    public class ObservedState<T> : ObservedFactorCore, IState<T>
    {
        [NotNull]
        protected readonly IEqualityComparer<T> valueComparer;
        protected          T                    currentValue;
        
        #region Instance Properties

        public virtual T Value 
        {
            get
            {
                NotifyInvolved();
                return currentValue;
            }
            set
            {
                bool valueIsTheSame = valueComparer.Equals(value, currentValue);
                
                if (valueIsTheSame is false)
                {
                    currentValue = value;
                    Observer.NotifyChanged(this);
                }
            }
        }
        
        public T Peek() => currentValue;

        #endregion
        

        public ObservedState(T initialValue, string name = null, IEqualityComparer<T> comparer = null) : base(name)
        {
            valueComparer = comparer?? EqualityComparer<T>.Default;
            currentValue  = initialValue;
        }

        public ObservedState(T initialValue, IEqualityComparer<T> comparer) : this(initialValue, null, comparer)
        {
        }
    }
}