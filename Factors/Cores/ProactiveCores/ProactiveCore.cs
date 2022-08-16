using System.Collections.Generic;
using Core.States;
using JetBrains.Annotations;

namespace Factors.Cores.ProactiveCores
{
    public abstract class ProactiveCore<T> : ProactorCore, IProactiveCore<T>
    {
        #region Instance Fields

        [NotNull]
        protected readonly IEqualityComparer<T> valueComparer;
        protected          T                    currentValue;

        #endregion
        
        
        #region Properties

        public virtual T Value => currentValue;

        #endregion


        #region Instance Methods

        public bool ValueEquals(T valueToCompare) => valueComparer.Equals(currentValue, valueToCompare);
        
        public virtual bool SetValueIfNotEqual(T newValue)
        {
            if (valueComparer.Equals(currentValue, newValue) is false)
            {
                currentValue = newValue;
                Callback.CoreUpdated(this);
                
                return true;
            }
            else return false;
        }

        #endregion


        #region Constructors

        protected ProactiveCore(T initialValue, IEqualityComparer<T> comparer = null)
        {
            valueComparer = comparer?? EqualityComparer<T>.Default;
            currentValue  = initialValue;
        }

        #endregion
    }
}