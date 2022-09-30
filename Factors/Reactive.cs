using System;
using System.Collections.Generic;
using Core.Factors;
using Core.States;
using Factors.Cores.DirectReactorCores;
using Factors.Cores.ObservedReactorCores;
using JetBrains.Annotations;

namespace Factors
{
    public class Reactive<T> : ReactiveValue<T, IResult<T>>
    {
        #region Constructors

        public Reactive([NotNull] IResult<T> valueSource, string name = null) : base(valueSource, name)
        {
        }

        public Reactive(Func<T> functionToDetermineValue, IEqualityComparer<T> comparer, string name = null) : 
            this(FunctionResult.CreateFrom(functionToDetermineValue, comparer), 
                name?? CreateDefaultName<Reactive<T>>(functionToDetermineValue) )
        {
            if (functionToDetermineValue == null)
            {
                throw new ArgumentNullException(nameof(functionToDetermineValue), CannotUseNullFunction);
            }
        }

        public Reactive(Func<T> functionToDetermineValue, string name = null) : 
            this(functionToDetermineValue, null, name)
        {
        }

        public Reactive(IFactor<T> factorToGetValueOf, string name = null) : 
            this(new DirectRelayCore<T>(factorToGetValueOf), name)
        {
        }
        
        #endregion
    }

    // public static class Reactive
    // {
    //     [Flags]
    //     public enum ReactiveType { None = 0, Reflexive = 1, Looping = 2, Async = 4, Caching = 8, };
    //     
    //     private const int Updating    = 0b000_0000_0010;
    //     private const int ThreadSafe  = 0b000_0000_0000;
    //     private const int Frozen      = 0b000_1000_0000;
    //     private const int Disposed    = 0b000_0000_0000;
    //     private const int IsAffecting = 0b000_0000_0000;
    //     private const int Patient     = 0b000_0000_0010;
    //     private const int Async       = 0b000_1000_0000;
    //     private const int Executing   = 0b000_0000_0010;
    //     private const int Exclusive   = 0b000_0000_0010;
    // }
}
