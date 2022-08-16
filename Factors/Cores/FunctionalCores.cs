using System;
using System.Collections.Generic;
using Core.Factors;
using Factors.Cores.DirectReactorCores;

namespace Factors.Cores
{
    public static class FunctionalCores
    {
        #region Static Methods
        
        public static DirectFunctionResult<TArg, TReturn> CreateFrom<TArg, TReturn>(
            Func<TArg, TReturn> function, IFactor<TArg> argSource, IEqualityComparer<TReturn> comparer = null)
        {
            if (function is null) { throw new ArgumentNullException(nameof(function)); }
            
            return new DirectFunctionResult<TArg, TReturn>(function, argSource, comparer);
        }

        public static DirectFunctionResult<TArg1, TArg2, TReturn> CreateFrom<TArg1, TArg2, TReturn>(
            Func<TArg1, TArg2, TReturn> function, IFactor<TArg1> arg1Source, IFactor<TArg2> arg2Source,
            IEqualityComparer<TReturn> comparer = null)
        {
            if (function is null) { throw new ArgumentNullException(nameof(function)); }
            
            return new DirectFunctionResult<TArg1, TArg2, TReturn>(function, arg1Source, arg2Source, comparer);
        }
        
        public static DirectFunctionResult<TArg1, TArg2, TArg3, TReturn> CreateFrom<TArg1, TArg2, TArg3, TReturn>(
            Func<TArg1, TArg2, TArg3, TReturn> function, IFactor<TArg1> arg1Source, IFactor<TArg2> arg2Source,
            IFactor<TArg3> arg3Source, IEqualityComparer<TReturn> comparer = null)
        {
            if (function is null) { throw new ArgumentNullException(nameof(function)); }
            
            return new DirectFunctionResult<TArg1, TArg2, TArg3, TReturn>(
                function, arg1Source, arg2Source, arg3Source, comparer);
        }

        #endregion
    }
}