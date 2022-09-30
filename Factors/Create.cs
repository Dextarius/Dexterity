using System;
using System.Collections.Generic;
using Core.Factors;
using Core.States;
using Factors.Cores;
using Factors.Cores.DirectReactorCores;
using Factors.Modifiers;
using Factors.Modifiers.Cores;

namespace Factors
{
    public static class Create
    {
        //- TODO : Come up with a better naming scheme.
        
        public static class A_Modifier
        {
            public static Modifier<double> ThatLimitsResultToValuesBetween(IFactor<double> minimum, IFactor<double> maximum)
            {
                var core     = new DoubleRangeLimiterModCore(minimum, maximum);
                var modifier = new Modifier<double>(core);

                return modifier;
            }

            public static Modifier<int> ThatLimitsResultToValuesBetween(IFactor<int> minimum, IFactor<int> maximum)
            {
                var core     = new IntRangeLimiterModCore(minimum, maximum);
                var modifier = new Modifier<int>(core);

                return modifier;
            }

            public static Modifier<uint> ThatLimitsResultToValuesBetween(IFactor<uint> minimum, IFactor<uint> maximum)
            {
                var core     = new UIntRangeLimiterModCore(minimum, maximum);
                var modifier = new Modifier<uint>(core);

                return modifier;
            }

            public static Modifier<TimeSpan> ThatLimitsResultToValuesBetween(
                IFactor<TimeSpan> minimum, IFactor<TimeSpan> maximum)
            {
                var core     = new TimeSpanRangeLimiterModCore(minimum, maximum);
                var modifier = new Modifier<TimeSpan>(core);

                return modifier;
            }
        }
        
        public static class A_Reactive
        {
            public static Reactive<TOutput> ThatPassesFactorsValueToFunction<TOutput, TInput>(
                IFactor<TInput> valueSource, Func<TInput, TOutput> valueFunction, 
                IEqualityComparer<TOutput> comparer = null)
            {
                var core = Create.A_Core.ThatPassesFactorsValueToFunction(valueSource, valueFunction, comparer);
                return new Reactive<TOutput>(core);
            }
        }
        
        public static class A_Condition
        {
            public static ReactiveCondition ThatUsesFunctionToCheckValue<TInput>(
                IFactor<TInput> valueSource, Func<TInput, bool> valueFunction)
            {
                var core = Create.A_Core.ThatPassesFactorsValueToFunction(valueSource, valueFunction);
                return new ReactiveCondition(core);
            }
        }
        
        
        public static class A_Core
        {
            public static IResult<TOutput> ThatPassesFactorsValueToFunction<TOutput, TInput>(
                IFactor<TInput> valueSource, Func<TInput, TOutput> valueFunction, 
                IEqualityComparer<TOutput> comparer = null) => 
                    new DirectFunctionResult<TInput, TOutput>(valueSource, valueFunction, comparer);
            
            public static IReactorCore ThatTriggersWhenConditionIsTrue(ICondition condition, Action actionToTake) => 
                new DirectActionResponse(condition.OnTrue, actionToTake);
            
            public static IReactorCore ThatTriggersWhenConditionIsFalse(ICondition condition, Action actionToTake) => 
                new DirectActionResponse(condition.OnFalse, actionToTake);
            
            public static IReactorCore ThatUsesCurrentAndPreviousValue<T>(IFactor<T> valueSource, Action<T,T> actionToTake) => 
                new HistoricDirectActionResponse<T>(valueSource, actionToTake);
        }
        
        
        public static class A_Reaction
        {
            public static Reaction ThatTriggersWhenConditionIsTrue(ICondition condition, Action actionToTake)
            {
                var core = Create.A_Core.ThatTriggersWhenConditionIsTrue(condition, actionToTake);
                return new Reaction(core);
            }
            
            public static Reaction ThatTriggersWhenConditionIsFalse(ICondition condition, Action actionToTake)
            {
                var core = Create.A_Core.ThatTriggersWhenConditionIsFalse(condition, actionToTake);
                return new Reaction(core);
            }

            public static Reaction ThatUsesCurrentAndPreviousValue<T>(IFactor<T> valueSource, Action<T,T> actionToTake)
            {
                var core = Create.A_Core.ThatUsesCurrentAndPreviousValue(valueSource, actionToTake);
                return new Reaction(core);
            }
        }
        
        public static class A_Modifiable
        {
            public static Modifiable<T> WithTheValue<T>(T initialValue)
            {
                var core = new ModifiableCore<T>(initialValue);
                return new Modifiable<T>(core);
            }
        }
        
        public static class A_Modified
        {
            public static Modified<T> WithTheValue<T>(T initialValue) => new Modified<T>(initialValue);
        }
    }
}

