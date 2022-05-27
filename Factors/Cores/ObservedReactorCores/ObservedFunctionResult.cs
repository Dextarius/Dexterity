using System;
using System.Collections.Generic;
using Core.Factors;
using Core.Tools;
using JetBrains.Annotations;

namespace Factors.Cores.ObservedReactorCores
{
    public class ObservedFunctionResult<T> : ObservedResult<T>
    {
        #region Instance Fields

        [NotNull]
        private readonly Func<T> valueFunction;

        #endregion
        

        #region Instance Methods

        protected override T GenerateValue() => valueFunction();

        public override string ToString() => Delegates.GetClassAndMethodName(valueFunction);

        #endregion

        
        #region Constructors

        public ObservedFunctionResult(Func<T>              functionThatDeterminesValue,
                                      IEqualityComparer<T> comparer = null)
            : base(comparer)
        {
            valueFunction = functionThatDeterminesValue??  
                            throw new ArgumentNullException(nameof(functionThatDeterminesValue));
        }

        #endregion
    }

    
    public static class FunctionResult
    {
        #region Static Methods

        public static ObservedFunctionResult<TValue> CreateFrom<TValue>(Func<TValue> function, 
                                                                IEqualityComparer<TValue> comparer = null)
        {
            if (function is null) { throw new ArgumentNullException(nameof(function)); }
            
            return new ObservedFunctionResult<TValue>(function, comparer);
        }

        #endregion
    }
}