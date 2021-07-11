using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Causality;
using Causality.Processes;
using Core.Causality;
using Core.Factors;
using Factors;
using JetBrains.Annotations;
using NUnit.Framework;
using static Core.Tools.Threading;

namespace Tests
{
    public static class Tools
    {
        private static readonly Random numberGenerator = new Random();
        public static int Return42() => 42;

        public static Reactive<T> CreateReactiveThatGetsValueOf<T>(Proactive<T> proactiveSourceValue) => new Reactive<T>(() => proactiveSourceValue.Value);
        public static Reactive<T> CreateReactiveThatGetsValueOf<T>(Reactive<T>  reactiveSourceValue)  => new Reactive<T>(() => reactiveSourceValue.Value);
        
        public static int[] CreateRandomArrayOfNumbers()
        {
            int   arraySize = numberGenerator.Next(1, 1000);
            int[] array     = new int[arraySize];

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = numberGenerator.Next(int.MinValue, int.MaxValue);
            }

            return array;
        }
        
        public static int ReturnTheNumber42() => 42;

        [MethodImpl(MethodImplOptions.NoOptimization)]
        public static Reactive<T> CreateReactiveThatDependsOn<T>(Proactive<T> proactiveSourceValue)
        {
            Reactive<T>reactiveBeingCollected = CreateReactiveThatGetsValueOf(proactiveSourceValue);
            T           triggerAReaction      = reactiveBeingCollected.Value;

            return reactiveBeingCollected;
        }

        public static Reactive<T> CreateReactiveThatDependsOn<T>(Reactive<T> reactiveSourceValue)
        {
            Reactive<T> reactiveBeingCollected = new Reactive<T>(() => reactiveSourceValue.Value);
            T           triggerAReaction       = reactiveBeingCollected.Value;

            return reactiveBeingCollected;
        }

        public static WeakReference<Reactive<T>> CreateUnreferencedReactiveInfluencedBy<T>(Proactive<T> proactiveSourceValue)
        {
           return CreateUnreferencedReactive(proactiveSourceValue);
        }

        public static WeakReference<Reactive<T>> CreateUnreferencedReactive<T>(Proactive<T> proactiveSourceValue) => 
            new WeakReference<Reactive<T>>(CreateReactiveThatDependsOn(proactiveSourceValue));

        public static Proactive<int> CreateChainOfReactions()
        {
            Proactive<int> proactiveValue            = new Proactive<int>(5);
            Reactive<int>  reactiveNotBeingCollected = CreateReactiveThatGetsValueOf(proactiveValue);
            int            triggerAReaction          = reactiveNotBeingCollected.Value;
            
            for (int i = 0; i < 3; i++)
            {
                reactiveNotBeingCollected = CreateReactiveThatGetsValueOf(reactiveNotBeingCollected);
            }

            Assert.True(proactiveValue.IsConsequential);

            return proactiveValue;
        }
        
        public static void WhileUpdatingAReactive_RunActionOnReactive(Action<Reactive<int>> actionToRun) =>
            WhileUpdatingAReactive_RunActionOnReactive(actionToRun, 42);

        public static void WhileUpdatingAReactive_RunActionOnReactive<T>(Action<Reactive<T>> actionToRun, T valueToUse)
        {
            ManualResetEvent updateStarted       = new ManualResetEvent(false);
            ManualResetEvent conditionChecked    = new ManualResetEvent(false);
            Proactive<T>     proactiveWithValue  = new Proactive<T>(valueToUse);
            Reactive<T>      reactiveBeingTested = new Reactive<T>(WaitAndReturnSourceValue);

            StartNewThreadThatRuns(UpdateReactive);
            updateStarted.WaitOne();
            actionToRun(reactiveBeingTested);
            conditionChecked.Set();

            return;


            T WaitAndReturnSourceValue()
            {
                updateStarted.Set();
                conditionChecked.WaitOne();

                return proactiveWithValue.Value;
            }

            void UpdateReactive()
            {
                T valueToTest = reactiveBeingTested.Value;

                Assert.That(valueToTest, Is.EqualTo(valueToUse));
            }
        }
        
        public static void EnsureValuesAreDifferent(ref int firstValue, ref int secondValue)
        {
            if (firstValue == secondValue)
            {
                secondValue++;
            }
        }
        
        public static IProcess GetProcessThatCreatesADependencyOn(IState stateToBeDependentOn) => 
            new ActionProcess(() => Observer.NotifyInvolved(stateToBeDependentOn));
    }
    

}