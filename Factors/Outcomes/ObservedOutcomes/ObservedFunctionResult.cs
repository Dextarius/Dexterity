using System;
using System.Collections.Generic;
using Core.States;
using Core.Tools;
using JetBrains.Annotations;

namespace Factors.Outcomes.ObservedOutcomes
{
    public class ObservedFunctionResult<T> : ObservedResult<T>
    {
        #region Instance Fields

        [NotNull]
        private readonly Func<T> valueFunction;

        #endregion
        

        #region Instance Methods

        protected override T GenerateValue() => valueFunction();

        #endregion

        
        #region Constructors

        public ObservedFunctionResult(Func<T> functionThatDeterminesValue, IEqualityComparer<T> comparer = null, string name = null)
            : base(name ?? Delegates.GetClassAndMethodName(functionThatDeterminesValue), comparer)
        {
            valueFunction = functionThatDeterminesValue??  
                            throw new ArgumentNullException(nameof(functionThatDeterminesValue));
        }
        
        public ObservedFunctionResult(Func<T> functionThatDeterminesValue, string name)
            : this(functionThatDeterminesValue, null, name)
        {
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