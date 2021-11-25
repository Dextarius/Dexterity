using System;
using System.Collections.Generic;
using Causality;
using Causality.States;
using Causality.Processes;
using Core.Causality;
using Core.Factors;
using Core.States;
using JetBrains.Annotations;

namespace Factors
{
    public static class Reactive
    {
        [Flags]
        public enum ReactiveType { None = 0, Reflexive = 1, Looping = 2, Async = 4, Caching = 8, };
        
        private const int Updating    = 0b000_0000_0010;
        private const int ThreadSafe  = 0b000_0000_0000;
        private const int Frozen      = 0b000_1000_0000;
        private const int Disposed    = 0b000_0000_0000;
        private const int IsAffecting = 0b000_0000_0000;
        private const int Patient     = 0b000_0000_0010;
        private const int Async       = 0b000_1000_0000;
        private const int Executing   = 0b000_0000_0010;
        private const int Exclusive   = 0b000_0000_0010;
    }


    
    public class Reactive<T> : Reactor, IProcess<T>, IReactive<T>
    {
        #region Constants

        private string CannotUseNullFunction =
            "A Reactive value cannot be constructed with a null " + nameof(Func<T>) + ", as it would never have a value. ";

        #endregion

        #region Instance Fields

        private IResult<T> outcome;

        #endregion

        #region Instance Properties

        protected override IResult Result => outcome;
        public             T       Value   => outcome.Value;

        #endregion


        #region Instance Methods

        public T Peek() => outcome.Peek();

        #endregion

        
        #region Operators

        public static implicit operator T(Reactive<T> reactive) => reactive.Value;

        #endregion
        
        
        #region Constructors

        public Reactive(IProcess<T> processToDetermineValue, IEqualityComparer<T> comparer, string name = null) : base(name)
        {
            if (processToDetermineValue is null) { throw new ArgumentNullException(nameof(processToDetermineValue)); }
            
            outcome = new Result<T>(this, processToDetermineValue, comparer);
        }

        public Reactive(IProcess<T> processToDetermineValue, string name = null) : 
            this(processToDetermineValue, null, name)
        {
        }
        
        public Reactive(Func<T> functionToDetermineValue, IEqualityComparer<T> comparer, string name = null) : 
            this(FunctionalProcess.CreateFrom(functionToDetermineValue), comparer,
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

        #endregion
        
        
        #region Explicit Implementations

        T IProcess<T>.Execute() => Value;

        #endregion

    }
}
