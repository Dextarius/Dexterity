using System;
using System.Collections.Generic;
using Causality;
using Causality.States;
using Causality.Processes;
using Core.Causality;
using Core.Factors;
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
    
    public class Reactive<T> : SubscribableReactor<T,T>
    {

        #region Instance Fields

        [NotNull]
        protected          IOutcome<T>          outcome = InvalidOutcome<T>.Default;
        protected readonly IProcess<T>          reactionProcess;
        protected readonly IEqualityComparer<T> valueComparer;

        #endregion
        
        
        #region Static Properties

        public static IEqualityComparer<T> DefaultValueComparer { get; set; } = EqualityComparer<T>.Default;
        
        
        #endregion

        
        #region Instance Properties

        protected override IOutcome Outcome => outcome;

        public T Value
        {
            get
            {
                React();
                return outcome.Value;
            }
        }

        #endregion


        #region Instance Methods

        protected override bool Act()
        {
            IOutcome<T> oldOutcome = outcome;
            IOutcome<T> newOutcome = IsReflexive?  new Outcome<T>(this)  :  new Outcome<T>();
            T           newValue   = Observer.ObserveInteractions(reactionProcess, newOutcome);

            using (Observer.PauseObservation()) //- To prevent us from adding dependencies to any other observations this one might be nested in. 
            {
                newOutcome.Value = newValue;
                outcome          = newOutcome;

                if (valueComparer.Equals(oldOutcome.Value, newOutcome.Value))
                {
                    return false;
                }
                else
                {
                    SubscriptionManager.Publish(newValue, oldOutcome.Value);

                    return true;
                }
                
                //- TODO : Consider if we want Outcomes to remain valid if the new value is equal to the old value?  We'd
                //         have to rewrite them to keep their dependents until the value was recalculated.
            }
        }

        #endregion

        
        #region Operators

        public static implicit operator T(Reactive<T> reactive) => reactive.Value;

        #endregion
        
        
        #region Constructors

        public Reactive(IProcess<T> processToDetermineValue, IEqualityComparer<T> comparer, string name = null) : base(name)
        {
            reactionProcess  = processToDetermineValue?? throw new ArgumentNullException(nameof(processToDetermineValue));
            valueComparer    = comparer?? DefaultValueComparer;
        }

        public Reactive(IProcess<T> processToDetermineValue, string name = null) : 
            this(processToDetermineValue, DefaultValueComparer, name)
        {
            
        }

        //- TODO : We commented out the use of GetClassAndMethodName() in this constructor because
        //         it was throwing an exception we couldn't solve at the moment, and providing names
        //         for the Reactives wasn't exactly high priority.  When you have time uncomment it and 
        //         try to fix whatever is causing the exception.
        public Reactive(Func<T> functionToDetermineValue, IEqualityComparer<T> comparer, string name = null) : 
            this(FunctionalProcess.CreateFrom(functionToDetermineValue), comparer,
                 name?? "Unnamed" /* GetClassAndMethodName(functionToDetermineValue) */ )
        {
            if (functionToDetermineValue == null) { throw new ArgumentNullException(
                                                   $"A Reactive value cannot be constructed with a null " +
                                                    $"{nameof(Func<T>)}, as it would never have a value. "); }

            reactionProcess = new FunctionalProcess<T>(functionToDetermineValue);
            valueComparer   = comparer?? DefaultValueComparer;
        }

        public Reactive(Func<T> functionToDetermineValue, string name = null) : 
            this(functionToDetermineValue, DefaultValueComparer, name)
        {
        }

        #endregion
    }







}
