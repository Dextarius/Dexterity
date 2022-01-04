using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Core.Causality;
using Core.Factors;
using Core.States;
using Factors;
using NUnit.Framework;
using Tests.Tools.Mocks;
using Tests.Tools.Mocks.Processes;
using static Core.Tools.Threading;

namespace Tests.Tools
{
    public static class Tools
    {
        private static readonly Random numberGenerator = new Random();
        
        public static int Return42() => 42;

        public static Reactive<T> CreateReactiveThatGetsValueOf<T>(Proactive<T> proactiveSourceValue) => 
            new Reactive<T>(() => proactiveSourceValue.Value);
        
        public static Reactive<T> CreateReactiveThatGetsValueOf<T>(Reactive<T>  reactiveSourceValue)  => 
            new Reactive<T>(() => reactiveSourceValue.Value);
        
        public static int[] CreateArrayOfRandomNumbers()
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

            Assert.True(proactiveValue.HasDependents);

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
        
        // public static IProcess GetProcessThatCreatesADependencyOn(IState stateToBeDependentOn) => 
        //     new ActionProcess(() => CausalObserver.ForThread.NotifyInvolved(stateToBeDependentOn));
        
        
        public static void DoNothing() { }

        public static bool ConvertBinaryIntToBool(int value) =>  (value == 1);
        
        // public static Action GetActionThatInvolves(IState stateToInvolve) 
        // {
        //     return InvolveState;
        //     
        //     
        //     void InvolveState()
        //     {
        //         stateToInvolve.Involved();
        //     }
        // }
        
        public static void AssumeHasNoInfluences<TFactor>(TFactor factor)  where TFactor : IOutcome
        {
            int numberOfInfluences = factor.NumberOfInfluences;
            
            Assume.That(factor.IsBeingInfluenced, Is.False, 
                ErrorMessages.HasInfluences<TFactor>("before being used. "));
            
            Assume.That(numberOfInfluences,        Is.Zero, 
                ErrorMessages.InfluencesGreaterThanZero<TFactor>("before being used. ", numberOfInfluences));
        }
        
        
        public static IProcess CreateProcessThatCallsNotifyInvolvedOn(IFactor factorToInvolve) => 
            new InvolveFactorProcess(factorToInvolve);

        public static IProcess CreateProcessThatRetrievesValueOf<T>(IFactor<T> factorToInvolve) => 
            new RetrieveValueProcess<T>(factorToInvolve);

        public static IProcess CreateProcessThatPeeksAtTheValueOf<T>(IFactor<T> factorToInvolve) => 
            new PeekValueProcess<T>(factorToInvolve);

        public static void WriteExpectedAndActualValuesToTestContext<T>(T expected, T actual) =>
            TestContext.WriteLine($"Expected Value {expected}, Actual Value {actual}");
        
        public static void WriteNameAndValueToTestContext<T>(string name, T value) =>
            TestContext.WriteLine($"{name} => {value}");

        public static void Assert_React_CreatesExclusiveDependencyBetween(IDeterminant parent, IReactor dependent)
        {
            Assert.That(dependent.IsValid,           Is.False);
            Assert.That(dependent.IsBeingInfluenced, Is.False);
            Assert.That(parent.HasDependents,    Is.False);
            
            dependent.React();
            
            Assert.That(dependent.IsBeingInfluenced, Is.True);
            Assert.That(parent.HasDependents,    Is.True);
        }

        public static int GenerateRandomInt() => numberGenerator.Next();
        
        public static int GenerateRandomIntNotEqualTo(int valueToAvoid) 
        {
            int number = numberGenerator.Next();

            while (number == valueToAvoid)
            {
                number = numberGenerator.Next();
            }

            return number;
        }

        public static MockDependent[] AddDependentsTo(IFactor factor, int numberOfDependents)
        {
            var dependents = new MockDependent[numberOfDependents];

            for (int i = 0; i < numberOfDependents; i++)
            {
                var createdDependent = new MockDependent();
                
                dependents[i] = createdDependent;
                createdDependent.MakeValid();
                factor.AddDependent(createdDependent);
            }

            return dependents;
        }
    }
    

}