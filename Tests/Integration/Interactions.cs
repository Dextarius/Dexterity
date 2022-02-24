using System;
using Factors;
using NUnit.Framework;
using Tests.Tools.Mocks;
using static Tests.Tools.Tools;

namespace Tests.Integration
{
    public class Interactions
    {
        [Test]
        public void WhenGettingValueFromAProactive_HasTheCorrectValue([Random(1)] int value)
        {
            Proactive<int> proactiveBeingTested = new Proactive<int>(value);
            Reactive<int>  reactiveBeingTested  = CreateReactiveThatGetsValueOf(proactiveBeingTested);
            int            actualValue          = reactiveBeingTested.Value; 
            
            Assert.That(actualValue, Is.EqualTo(value));
            TestContext.WriteLine($"Expected Value {value}, Actual Value {actualValue}");
        }
        
        [Test]
        public void WhenGettingValueFromMultipleProactives_HasTheCorrectValue()
        {
            int            firstValue          = GenerateRandomInt();
            int            secondValue         = GenerateRandomIntNotEqualTo(firstValue);
            Proactive<int> firstProactive      = new Proactive<int>(firstValue);
            Proactive<int> secondProactive     = new Proactive<int>(secondValue);
            Func<int>      sumValues           = () => firstProactive.Value + secondProactive.Value;
            Reactive<int>  reactiveBeingTested = new Reactive<int>(sumValues);
            int            expectedValue       = firstValue + secondValue; 
            int            actualValue         = reactiveBeingTested.Value; 
            
            Assert.That(actualValue, Is.EqualTo(expectedValue));
            TestContext.WriteLine($"Expected Value {expectedValue}, Actual Value {actualValue}");
        }
        
        [Test]
        public void WhenDependentOnMultipleProactivesAndAnyChange_ValueChangesCorrectly(
            [Random(1)] int initialValueOne, [Random(1)] int initialValueTwo, [Random(1)] int increment)
        {
            Proactive<int> firstProactive      = new Proactive<int>(initialValueOne);
            Proactive<int> secondProactive     = new Proactive<int>(initialValueTwo);
            Func<int>      sumValues           = () => firstProactive.Value + secondProactive.Value;
            Reactive<int>  reactiveBeingTested = new Reactive<int>(sumValues);
            int            expectedValue       = sumValues();
            int            actualValue         = reactiveBeingTested.Value;
            
            
            Assert.That(actualValue, Is.EqualTo(expectedValue));
            TestContext.WriteLine($"Expected Value {expectedValue}, Actual Value {actualValue}");

            firstProactive.Value += increment;
            expectedValue         = sumValues();
            actualValue           = reactiveBeingTested.Value;
            
            Assert.That(actualValue, Is.EqualTo(expectedValue));
            TestContext.WriteLine($"Expected Value {expectedValue}, Actual Value {actualValue}");

            secondProactive.Value += increment;
            expectedValue          = sumValues();
            actualValue            = reactiveBeingTested.Value;
            
            Assert.That(actualValue, Is.EqualTo(expectedValue));
            TestContext.WriteLine($"Expected Value {expectedValue}, Actual Value {actualValue}");
        }

        [Test]
        public void AfterParentFactorChangesItsValue_IsTriggered()
        {
            int            initialValue        = GenerateRandomInt();
            int            updatedValue        = GenerateRandomIntNotEqualTo(initialValue);
            Proactive<int> proactive           = new Proactive<int>(initialValue);
            Reactive<int>  reactiveBeingTested = new Reactive<int>(() => proactive);
            int            triggerValueUpdate  = reactiveBeingTested.Value;
                
            Assert.That(reactiveBeingTested.HasBeenTriggered, Is.False,
                $"The {nameof(Reactive<int>)} was not valid after determining its value. ");

            proactive.Value = updatedValue;
            
            Assert.That(reactiveBeingTested.HasBeenTriggered, Is.True,
                "The reactive was not marked as triggered after being invalidated.");
            TestContext.WriteLine($"The {nameof(Reactive)} was triggered => {reactiveBeingTested.HasBeenTriggered}");
        }
        
        [Test]
        public void WhenProactiveValueChanges_ReactorsValueAlsoChanges([Random(1)] int initialValue, [Random(1)] int increment)
        {
            Proactive<int> proactiveBeingTested = new Proactive<int>(initialValue);
            Reactive<int>  reactiveBeingTested  = CreateReactiveThatGetsValueOf(proactiveBeingTested);
            int            actualValue          = reactiveBeingTested.Value; 
            
            Assert.That(actualValue, Is.EqualTo(initialValue));
            TestContext.WriteLine($"Expected Value {initialValue}, Actual Value {actualValue}");

            int updatedValue = proactiveBeingTested.Value += increment;
            
            proactiveBeingTested.Value = updatedValue;
            actualValue = reactiveBeingTested.Value;
            
            Assert.That(actualValue, Is.EqualTo(updatedValue));
            TestContext.WriteLine($"Expected Value {updatedValue}, Actual Value {actualValue}");
        }

        [Test]
        public void Proactive_WhenValueRetrievedDuringAReaction_GainsReactorAsADependent()
        {
            Proactive<int> proactiveBeingTested = new Proactive<int>(42);
            Reactive<int>  dependentReactive    = new Reactive<int>(() => proactiveBeingTested);
            int            triggerValueUpdate   = dependentReactive.Value;

            Assert.That(proactiveBeingTested.HasSubscribers);
        }
        
        //- Maybe test IsReflexive instead?
        // [Test]
        // public void WhenSubscriberIsMadeNecessary_ParentFactorIsAlsoNecessary()
        // {
        //     var factorToTest = factorFactory.CreateInstance();
        //     var subscriber   = new MockFactorSubscriber();
        //     
        //     Assert.That(factorToTest.IsNecessary, Is.False);
        //     Assert.That(subscriber.IsNecessary,   Is.False);
        //
        //     factorToTest.Subscribe(subscriber);
        //     subscriber.nec;
        //     
        //     Assert.That(factorToTest.IsNecessary, Is.True);
        // }

    }
    
    
}