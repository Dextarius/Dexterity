using System.Collections.Generic;
using Core.Causality;
using Core.States;
using JetBrains.Annotations;

namespace Causality.States
{
    //- TODO : Come up with a better name for this.
    public class State<T> : CausalFactor, IMutableState<T>
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
                    OnChanged();
                }
            }
        }

        public T Peek() => currentValue;
        
        

        #endregion
        

        public State(object ownerToReference, T initialValue, IEqualityComparer<T> comparer) : base(ownerToReference)
        {
            valueComparer = comparer?? EqualityComparer<T>.Default;
            currentValue  = initialValue;
        }

        public State(object ownerToReference, T initialValue) : 
            this(ownerToReference, initialValue, EqualityComparer<T>.Default)
        {
        }
        
        public State(T initialValue) : 
            this(null, initialValue, EqualityComparer<T>.Default)
        {
        }
    }
}