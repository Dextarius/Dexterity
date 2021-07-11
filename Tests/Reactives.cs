using System;
using System.Threading;
using Factors;
using NUnit.Framework;
using static Tests.Tools;
using static Core.Tools.Types;
using static Tests.ErrorMessages;

namespace Tests
{
    public class Reactives
    {
        //- TODO : Some of these methods shouldn't be using Random parameters, since we could end up with both parameters 
        //         being the same value, which means any Proactives won't update if given them.
        
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Constructor_WhenGivenNullDelegate_ThrowsException() => 
            Assert.Throws<ArgumentNullException>(() => new Reactive<int>((Func<int>) null));

        [Test]
        public void WhenGivenANameDuringConstruction_HasThatName()
        {
            string        givenName           = "Some Reactive";
            Reactive<int> reactiveBeingTested = new Reactive<int>(Return42, givenName);
            string        actualName          = reactiveBeingTested.Name; 
            
            Assert.That(actualName,Is.EqualTo(givenName));
            TestContext.WriteLine($"Expected Value => {givenName},\nActual Value => {actualName}");
        }

        [Test]
        public void Constructor_WhenGivenNullName_UsesADefault()
        {
            Reactive<int> testReactive = new Reactive<int>(Return42, null);

            Assert.NotNull(testReactive.Name);
        }

        [Test]
        public void AfterConstruction_IsInvalid()
        {
            Reactive<int> reactiveToTest = new Reactive<int>(Return42);
            
            Assert.That(reactiveToTest.IsValid, Is.False, $"The property {nameof(reactiveToTest.IsValid)} was marked as true during construction.");
        }

        [Test]
        public void AfterConstruction_IsNotReflexive()
        {
            Reactive<int> reactiveToTest = new Reactive<int>(Return42);
            
            Assert.That(reactiveToTest.IsReflexive, Is.False, $"The property {nameof(reactiveToTest.IsReflexive)} was marked as true during construction.");
        }

        [Test]
        public void AfterConstruction_IsNotConsequential()
        {
            Reactive<int> reactiveToTest = new Reactive<int>(Return42);
            
            Assert.That(reactiveToTest.IsConsequential, Is.False, $"The property {nameof(reactiveToTest.IsConsequential)} was marked as true during construction.");
        }

        [Test]
        public void AfterConstruction_IsNotUpdating()
        {
            Reactive<int> reactiveToTest = new Reactive<int>(Return42);
            
            Assert.That(reactiveToTest.IsUpdating, Is.False, $"The property {nameof(reactiveToTest.IsUpdating)} was marked as true during construction.");
        }

        [Test]
        public void WhenValueRetrieved_MatchesFunctionOutput()
        {
            Reactive<int> reactiveBeingTested = new Reactive<int>(Return42);
            int           expectedValue       = Return42();
            int           actualValue         = reactiveBeingTested.Value; 
            
            Assert.That(actualValue,Is.EqualTo(expectedValue));
            TestContext.WriteLine($"Expected Value {expectedValue}, Actual Value {actualValue}");
        }

        [Test]
        public void WhenGettingValueFromAProactive_HasTheCorrectValue([Random(1)] int value)
        {
            Proactive<int> proactiveBeingTested = new Proactive<int>(value);
            Reactive<int>  reactiveBeingTested  = CreateReactiveThatGetsValueOf(proactiveBeingTested);
            int            actualValue          = reactiveBeingTested.Value; 
            
            Assert.That(actualValue,Is.EqualTo(value));
            TestContext.WriteLine($"Expected Value {value}, Actual Value {actualValue}");
        }

        [Test]
        public void WhenCreated_DoesNotReactUntilValueIsRequested()
        {
            bool          processExecuted = false;
            Reactive<int> reactiveToTest  = new Reactive<int>(UpdateBoolAndReturn);
            
            Assert.That(processExecuted, Is.False, "The reactive executed its process during construction.");
            TestContext.WriteLine($"The process was executed prior to retrieving value => {processExecuted}");
            
            int triggerProcess = reactiveToTest.Value;
            
            Assert.That(processExecuted, "The method used to conduct this test does not mark that the process was executed.");
            TestContext.WriteLine($"The process was executed after retrieving value => {processExecuted}");

            int UpdateBoolAndReturn()
            {
                processExecuted = true;
                return 42;
            }
        }

        [Test]
        public void WhenGettingValueFromMultipleProactives_HasTheCorrectValue([Random(1)] int firstValue, [Random(1)] int secondValue)
        {
            Proactive<int> firstProactive       = new Proactive<int>(firstValue);
            Proactive<int> secondProactive      = new Proactive<int>(secondValue);
            Func<int>      sumValues            = () => firstProactive.Value + secondProactive.Value;
            Reactive<int>  reactiveBeingTested  = new Reactive<int>(sumValues);
            int            expectedValue        = firstValue + secondValue; 
            int            actualValue          = reactiveBeingTested.Value; 
            
            Assert.That(actualValue,Is.EqualTo(expectedValue));
            TestContext.WriteLine($"Expected Value {expectedValue}, Actual Value {actualValue}");
        }

        [Test]
        public void AfterDependentFactorChangesItsValue_IsInvalid([Random(1)] int initialValue, [Random(1)] int updatedValue)
        {
            Proactive<int> proactive           = new Proactive<int>(initialValue);
            Reactive<int>  reactiveBeingTested = new Reactive<int>(() => proactive);
            int            triggerValueUpdate  = reactiveBeingTested.Value;
                
            Assert.That(reactiveBeingTested.IsValid, "The reactive was not valid after initial construction. ");

            proactive.Value = updatedValue;
            
            Assert.That(reactiveBeingTested.IsValid == false, "The reactive was not marked as invalid after being invalidated.");
            TestContext.WriteLine($"The {nameof(Reactive)} was valid => {reactiveBeingTested.IsValid}");
        }

        [Test]
        public void WhenProactiveValueChanges_ItsValueAlsoChanges([Random(1)] int initialValue, [Random(1)] int increment)
        {
            Proactive<int> proactiveBeingTested = new Proactive<int>(initialValue);
            Reactive<int>  reactiveBeingTested  = CreateReactiveThatGetsValueOf(proactiveBeingTested);
            int            actualValue          = reactiveBeingTested.Value; 
            
            Assert.That(actualValue,Is.EqualTo(initialValue));
            TestContext.WriteLine($"Expected Value {initialValue}, Actual Value {actualValue}");

            int updatedValue = proactiveBeingTested.Value += increment;
            
            proactiveBeingTested.Value = updatedValue;
            actualValue = reactiveBeingTested.Value;
            
            Assert.That(actualValue, Is.EqualTo(updatedValue));
            TestContext.WriteLine($"Expected Value {updatedValue}, Actual Value {actualValue}");
        }

        [Test]
        public void WhenDependentOnMultipleProactivesAndAnyChange_ValueChangesCorrectly(
            [Random(1)] int initialValueOne, [Random(1)] int initialValueTwo, [Random(1)] int increment)
        {
            Proactive<int> firstProactive       = new Proactive<int>(initialValueOne);
            Proactive<int> secondProactive      = new Proactive<int>(initialValueTwo);
            Func<int>      sumValues            = () => firstProactive.Value + secondProactive.Value;
            Reactive<int>  reactiveBeingTested  = new Reactive<int>(sumValues);
            int            expectedValue        = sumValues();
            int            actualValue          = reactiveBeingTested.Value;
            
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
        public void WhenManuallyInvalidated_IsInvalid()
        {
            Proactive<int> proactive           = new Proactive<int>(42);
            Reactive<int>  reactiveBeingTested = new Reactive<int>(() => proactive);
            int            triggerValueUpdate  = reactiveBeingTested.Value;
                
            Assert.That(reactiveBeingTested.IsValid, "The reactive was not valid after initial construction. ");
            reactiveBeingTested.Invalidate();
            Assert.That(reactiveBeingTested.IsValid == false, "The reactive was not marked as invalid after being invalidated.");
            TestContext.WriteLine($"The {nameof(Reactive)} was valid => {reactiveBeingTested.IsValid}");
        }
        
        [Test]
        public void WhileUpdating_IsInvalid()
        {
            ReactiveManipulator<int> manipulator = new ReactiveManipulator<int>(42);

            using (manipulator.PutReactiveIntoUpdatingState())
            {
                Assert.That(manipulator.Reactive.IsValid, Is.False,
                    $"{NameOf<Reactive<int>>()}.{nameof(Reactive<int>.IsValid)} was true during its update. ");
                TestContext.WriteLine($"{nameof(Reactive<int>.IsValid)} was {manipulator.Reactive.IsValid}. ");
            }
        }
        
        [Test]
        public void WhileUpdating_IsUpdatingReturnsTrue()
        {
            ReactiveManipulator<int> manipulator = new ReactiveManipulator<int>(42);

            using (manipulator.PutReactiveIntoUpdatingState())
            {
                Assert.That(manipulator.Reactive.IsUpdating, $"{NameOf<Reactive<int>>()}.{nameof(Reactive<int>.IsUpdating)} was false during its update. ");
                TestContext.WriteLine($"{nameof(Reactive<int>.IsUpdating)} was {manipulator.Reactive.IsUpdating}. ");
            }
        }

        [Test]
        public void AfterReacting_IsValid([Random(1)] int initialValue, [Random(1)] int increment)
        {
            Proactive<int> proactive           = new Proactive<int>(initialValue);
            Reactive<int>  reactiveBeingTested = new Reactive<int>(() => proactive);
            int            triggerValueUpdate  = reactiveBeingTested.Value;
                
            Assert.That(reactiveBeingTested.IsValid, "The reactive was not valid after initial construction. ");

            proactive.Value += increment;
            
            Assert.That(reactiveBeingTested.IsValid == false, "The reactive was not marked as invalid after being invalidated.");
            TestContext.WriteLine($"The {nameof(Reactive)} was valid => {reactiveBeingTested.IsValid}");
            
            triggerValueUpdate = reactiveBeingTested.Value;
            Assert.That(reactiveBeingTested.IsValid, "The reactive was not marked as valid after reacting.");
            TestContext.WriteLine($"The {nameof(Reactive)} was valid => {reactiveBeingTested.IsValid}");
        }

        [Test]
        public void Reactive_AfterReacting_DoesNotUpdateValueWithoutBeingTriggered([Random(1)] int initialValue)
        {
            int           numberOfTimesFunctionIsRun = 0;
            int           updatedValue               = initialValue;
            Reactive<int> testReactive               = new Reactive<int>(IncrementAndReturnTestValue);
            int           triggerReaction            = testReactive.Value;
            
            updatedValue    += 1000;
            triggerReaction += testReactive.Value;

            Assert.That(numberOfTimesFunctionIsRun, Is.EqualTo(1));
            Assert.That(testReactive.Value, Is.EqualTo(initialValue));

            int IncrementAndReturnTestValue()
            {
                Interlocked.Increment(ref numberOfTimesFunctionIsRun);
                return updatedValue;
            }
        }
        
        [Test]
        public void WhenIsReflexiveSet_GetIsReflexiveMatchesSetValue()
        {
            Reactive<int> reactiveBeingTested = new Reactive<int>(ReturnTheNumber42);
                
            Assert.That(reactiveBeingTested.IsReflexive,  Is.False,
                $"The {NameOf<Reactive<int>>()} was marked as reflexive, despite that option not being set. ");

            reactiveBeingTested.IsReflexive = true;
                
            Assert.That(reactiveBeingTested.IsReflexive,
                $"The {NameOf<Reactive<int>>()} was marked as not being reflexive after setting that option to true. ");
                
            reactiveBeingTested.IsReflexive = false;
                
            Assert.That(reactiveBeingTested.IsReflexive,  Is.False,
                $"The {NameOf<Reactive<int>>()} was marked as being reflexive after setting that option to false. ");
        }
        

        [Test]
        public void IfInvalidatedWhileReflexive_AutomaticallyReacts()
        {
            int            initialValue       = 42;
            int            updatedValue       = 13;
            int            numberOfExecutions = 0;
            Proactive<int> proactive          = new Proactive<int>(initialValue);
            Reactive<int>  reactiveToTest     = new Reactive<int>(IncrementNumExecutionsAndReturn);

            Assert.That(numberOfExecutions, Is.Zero);

            int triggerProcess = reactiveToTest.Value;
            TestContext.WriteLine("Initial value calculated.");

            Assert.That(numberOfExecutions, Is.EqualTo(1));

            reactiveToTest.IsReflexive = true;
            proactive.Value = updatedValue;

            Assert.That(numberOfExecutions, Is.EqualTo(2));
            Assert.That(reactiveToTest.Value, Is.EqualTo(updatedValue), 
                ErrorMessages.ValueDidNotMatch<Reactive<int>>("the value returned by the function it was given."));
            
            
            int IncrementNumExecutionsAndReturn()
            {
                numberOfExecutions++;
                TestContext.WriteLine("The process was executed.");

                return proactive.Value;
            }
        }
        
        

        [Test]
        public void IfInvalidatedWhenNotReflexive_DoesNotUpdateValue()
        {
            int            numberOfExecutions = 0;
            Proactive<int> proactive          = new Proactive<int>(42);
            Reactive<int>  reactiveToTest     = new Reactive<int>(IncrementNumExecutionsAndReturn);

            Assert.That(numberOfExecutions, Is.Zero);
            Assert.That(reactiveToTest.IsReflexive, Is.False, $"The value of {nameof(reactiveToTest.IsReflexive)} was {reactiveToTest.IsReflexive}");

            int triggerProcess = reactiveToTest.Value;
            TestContext.WriteLine("Initial value calculated.");

            Assert.That(numberOfExecutions, Is.EqualTo(1));

            proactive.Value = 13;
            TestContext.WriteLine("Proactive value updated.");
            
            Assert.That(numberOfExecutions, Is.EqualTo(1));
            
            triggerProcess = reactiveToTest.Value;
            TestContext.WriteLine("Reactive value updated.");

            Assert.That(numberOfExecutions, Is.EqualTo(2));


            int IncrementNumExecutionsAndReturn()
            {
                numberOfExecutions++;
                TestContext.WriteLine("The process was executed.");

                return proactive.Value;
            }
        }

        [Test]
        public void AfterUpdating_NotifiesSubscribersOfValueChange()
        {
            bool           methodWasExecuted = false;
            int            initialValue      = 13; 
            int            updatedValue      = 42; 
            Proactive<int> proactive         = new Proactive<int>(initialValue);
            Reactive<int>  reactiveToTest    = CreateReactiveThatGetsValueOf(proactive);
            int            result            = reactiveToTest.Value;
            
            Assert.That(result, Is.EqualTo(initialValue));

            reactiveToTest.Subscriptions.Subscribe(TestPublishedValues);
            proactive.Value = updatedValue;
            result = reactiveToTest.Value;
            
            Assert.That(result, Is.EqualTo(updatedValue));
            Assert.That(methodWasExecuted, "The subscriber method was not executed.");
            

            void TestPublishedValues(int newValue, int oldValue)
            {
                Assert.That(newValue, Is.EqualTo(updatedValue));
                TestContext.WriteLine($"New value is {newValue}.");
                
                Assert.That(oldValue, Is.EqualTo(initialValue));
                TestContext.WriteLine($"Old value is {oldValue}.");

                methodWasExecuted = true;
            }
        }

        [Test]
        public void WhenSubscribedTo_HasSubscribersIsTrue()
        {
            Reactive<int> reactiveToTest = new Reactive<int>(Return42);

            Assert.That(reactiveToTest.Subscriptions.HasSubscribers, Is.False);
            reactiveToTest.Subscriptions.Subscribe(() => Assert.Fail("Method was executed"));
            Assert.That(reactiveToTest.Subscriptions.HasSubscribers);
        }
        
        [Test]
        public void WhenValueRetrievedDuringAReaction_BecomesConsequential()
        {
            Reactive<int> reactiveBeingTested = new Reactive<int>(ReturnTheNumber42);
            Reactive<int> dependentReactor    = CreateReactiveThatGetsValueOf(reactiveBeingTested);

            Assert.That(reactiveBeingTested.IsConsequential, Is.False, $"{FactorWasConsequential<Reactive<int>>("before being used")}");
            int triggerAReaction = dependentReactor.Value;
            
            Assert.That(reactiveBeingTested.IsConsequential, 
                $"{FactorWasNotConsequential<Proactive<int>>("despite being used to calculate a value")}");
        }

        public void IfUpdateProcessChangesAProactive_WarnsUser()
        {

        }

        public void WhileUpdateInProgress_DoesNotNotifySubscribersBeforeUpdateIsFinished()
        {
            
        }

        public void WhenGivenAnActionByASubscriber_AddsThatActionToSubscriberList()
        {
            
        }

        public void IfAThreadTriesToUpdateWhileAnotherThreadIsAlreadyUpdating_UpdateWaitsUntilThePriorThreadFinishes()
        {
           // Reactive<int> reactiveToTest = new Reactive<int>();
            
        }

        public void WhenReactIsManuallyCalled_TriggersReactionIfInvalid()
        {
            
        }
        
        //public void SetInMotion_AfterPuttingAnInitialConsequenceInMotion_EnsuresThatUpdatesAreBeingQueued()
        //{
        //
        //}
        
    }
}