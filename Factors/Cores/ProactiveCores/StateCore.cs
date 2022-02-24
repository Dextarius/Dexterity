using System.Collections.Generic;
using Core.States;
using JetBrains.Annotations;

namespace Factors.Cores.ProactiveCores
{
    public abstract class StateCore<T> : FactorCore, IState<T>
    {
        #region Instance Fields

        [NotNull]
        protected readonly IEqualityComparer<T> valueComparer;
        protected T currentValue;

        #endregion
        
        
        #region Properties

        public abstract T   Value    { get; set; }
        public override int Priority => 0;
        
        #endregion


        #region Instance Methods

        public T Peek() => currentValue;

        #endregion


        #region Constructors

        protected StateCore(T initialValue, string name = null, IEqualityComparer<T> comparer = null) : base(name)
        {
            valueComparer = comparer?? EqualityComparer<T>.Default;
            currentValue  = initialValue;
        }

        #endregion
    }
}