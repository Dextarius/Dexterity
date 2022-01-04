using System;
using System.Collections.Generic;
using Core.Causality;
using Core.Factors;
using Core.States;
using Factors.Outcomes.ObservedOutcomes;
using JetBrains.Annotations;

namespace Factors
{
    public class Reactive<T> : Reactor, IReactive<T>
    {
        #region Constants

        private string CannotUseNullFunction =
            "A Reactive value cannot be constructed with a null " + nameof(Func<T>) + ", as it would never have a value. ";

        #endregion

        
        #region Instance Fields

        [NotNull]
        private readonly IResult<T> result;

        #endregion

        
        #region Instance Properties

        protected override IOutcome Outcome => result;
        public             T        Value   => result.Value;

        #endregion


        #region Instance Methods

        public T Peek() => result.Peek();

        #endregion

        
        #region Operators

        public static implicit operator T(Reactive<T> reactive) => reactive.Value;

        #endregion
        
        
        #region Constructors

        public Reactive([NotNull] IResult<T> valueSource, string name = null) : base(name)
        {
            result = valueSource ?? throw new ArgumentNullException(nameof(valueSource));
        }

        public Reactive(Func<T> functionToDetermineValue, IEqualityComparer<T> comparer, string name = null) : 
            base(name?? CreateDefaultName<Reactive<T>>(functionToDetermineValue) )
        {
            if (functionToDetermineValue == null)
            {
                throw new ArgumentNullException(nameof(functionToDetermineValue), CannotUseNullFunction);
            }

            result = FunctionResult.CreateFrom(functionToDetermineValue, comparer ?? EqualityComparer<T>.Default);
        }

        public Reactive(Func<T> functionToDetermineValue, string name = null) : 
            this(functionToDetermineValue, EqualityComparer<T>.Default, name)
        {
        }

        #endregion
    }

    
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
}
