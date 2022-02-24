using Core.Factors;
using Core.States;
using Factors;
using NUnit.Framework;
using Tests.Tools;
using Tests.Tools.Interfaces;
using Tests.Tools.Mocks;
using Tests.Tools.Mocks.Processes;
using static Core.Tools.Types;
using static Tests.Tools.Tools;

namespace Tests.Interface_Tests
{
    
    public class IReactors<TReactor, TReactorFactory, TSubscriber, TSubscriberFactory>
        where TReactor           : IReactor
        where TReactorFactory    : IFactory<TReactor>, new()
        where TSubscriber        : IFactorSubscriber
        where TSubscriberFactory : IFactory<TSubscriber>, new()
    {
        #region Instance Fields

        private readonly TReactorFactory    reactorFactory    = new TReactorFactory();
        private readonly TSubscriberFactory subscriberFactory = new TSubscriberFactory();

        #endregion

        
        #region Tests
        
        [Test]
        public void AfterReacting_HasBeenTriggeredIsFalse()
        {
            TReactor reactorBeingTested = reactorFactory.CreateInstance();

            if (reactorBeingTested.HasBeenTriggered is false)
            {
                reactorBeingTested.Trigger();

                if (reactorBeingTested.HasBeenTriggered is false)
                {
                    Assert.Inconclusive($"The {nameof(TReactor)} could not be put in a triggered state.");
                }
            }
            
            reactorBeingTested.ForceReaction();
            Assert.That(reactorBeingTested.HasBeenTriggered, Is.False, 
                $"{nameof(reactorBeingTested.HasBeenTriggered)} was still true after reacting.");
        }
        
        [Test]
        public void AfterTriggerIsCalled_HasBeenTriggeredIsTrue()
        {
            IReactor reactorToTest = reactorFactory.CreateInstance();

            if (reactorToTest.HasBeenTriggered)
            {
                reactorToTest.ForceReaction();
            }
            
            Assert.That(reactorToTest.HasBeenTriggered, Is.False);
            reactorToTest.Trigger();
            Assert.That(reactorToTest.HasBeenTriggered, Is.True, 
                $"The reactor {nameof(reactorToTest.HasBeenTriggered)} was still false after calling " +
                $"{nameof(reactorToTest.Trigger)}() ");
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
        public void WhenTriggered_AllSubscribersAreDestabilized()
        {
            IReactor               reactorBeingTested  = reactorFactory.CreateInstance();
            int                    numberOfSubscribers = 10;
            MockFactorSubscriber[] subscribers;

            reactorBeingTested.ForceReaction();
            Assert.That(reactorBeingTested.HasBeenTriggered, Is.False);

            subscribers = AddSubscribersTo(reactorBeingTested, numberOfSubscribers);
            reactorBeingTested.Trigger();

            for (int i = 0; i < numberOfSubscribers; i++)
            {
                var subscriber = subscribers[i];

                Assert.That(subscriber.HasBeenTriggered, Is.False);
                Assert.That(subscriber.IsUnstable,       Is.True);
            }
        }
        
        [Test]
        public void WhenDestabilized_AllSubscribersAreDestabilized()
        {
            IReactor               reactorBeingTested  = reactorFactory.CreateInstance();
            int                    numberOfSubscribers = 10;
            MockFactorSubscriber[] subscribers;

            reactorBeingTested.ForceReaction();
            Assert.That(reactorBeingTested.HasBeenTriggered, Is.False);
            
            subscribers = AddSubscribersTo(reactorBeingTested, numberOfSubscribers);
            reactorBeingTested.Destabilize();

            for (int i = 0; i < numberOfSubscribers; i++)
            {
                var subscriber = subscribers[i];

                Assert.That(subscriber.HasBeenTriggered, Is.False);
                Assert.That(subscriber.IsUnstable,       Is.True);
            }
        }
        
        [Test]
        public void IfReactorIsAlreadyTriggeredWhenSubscribedTo_SubscriberIsStillAdded()
        {
            TReactor    reactorToTest = reactorFactory.CreateInstance();
            TSubscriber subscriber    = subscriberFactory.CreateInstance();
            int         originalNumberOfSubscribers;
            int         expectedNumberOfSubscribers;
            int         actualNumberOfSubscribers;

            reactorToTest.Trigger();
            Assert.That(reactorToTest.HasBeenTriggered, Is.True);

            originalNumberOfSubscribers = reactorToTest.NumberOfSubscribers;
            expectedNumberOfSubscribers = originalNumberOfSubscribers + 1;
            reactorToTest.Subscribe(subscriber);
            actualNumberOfSubscribers = reactorToTest.NumberOfSubscribers;
            
            Assert.That(actualNumberOfSubscribers, Is.EqualTo(expectedNumberOfSubscribers));
            WriteExpectedAndActualValuesToTestContext(expectedNumberOfSubscribers, actualNumberOfSubscribers);
        }

        [Test]
        public void IfReactorIsTriggeredWhenSubscribedTo_SubscriberIsDestabilized()
        {
            TReactor reactorToTest = reactorFactory.CreateInstance();
            var      subscriber    = new MockFactorSubscriber();

            reactorToTest.Trigger();
            Assert.That(reactorToTest.HasBeenTriggered, Is.True);

            reactorToTest.Subscribe(subscriber);
            Assert.That(subscriber.IsUnstable, Is.True);
        }

        public void WhenMadeReflexiveWhileUnstable_NotifiesParentItIsNecessary()
        {
            
        }

        public void WhenMadeReflexiveAfterBeingTriggered_NotifiesParentItIsNecessary()
        {
            
        }
        
        // [Test]
        // public void WhenDependentIsReflexive_ParentIsNecessary()
        // {
        //     var reactorToTest   = reactorFactory.CreateInstance();
        //     var dependentResult = subscriberFactory.CreateInstance();
        //
        //     Assert_React_CreatesExclusiveDependencyBetween(reactorToTest, dependentResult);
        //     Assert.That(reactorToTest.IsNecessary,   Is.False);
        //     Assert.That(dependentResult.IsReflexive, Is.False);
        //
        //     dependentResult.IsReflexive = true;
        //
        //     Assert.That(reactorToTest.IsNecessary,    Is.True);
        //     Assert.That(dependentResult.IsReflexive, Is.True);
        // }

        [Test]
        public void WhenReactorWithNecessaryDependentsIsTriggered_ReactorAutomaticallyUpdates()
        {
            var reactorToTest = reactorFactory.CreateInstance();
            var subscriber    = new MockFactorSubscriber();
            
            reactorToTest.ForceReaction();
            Assert.That(reactorToTest.HasBeenTriggered, Is.False);
            
            subscriber.MakeNecessary();
            Assert.That(subscriber.IsNecessary, Is.True);

            reactorToTest.Subscribe(subscriber);
            reactorToTest.Trigger();
            
            Assert.That(reactorToTest.HasBeenTriggered, Is.False);
            //- TODO : This test seems like it's not really testing if the reaction goes off.
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
            TestContext.WriteLine($"IsReflexive => {reactiveToTest.IsReflexive}.");
            proactive.Value = updatedValue;
            TestContext.WriteLine($"IsValid => {reactiveToTest.HasBeenTriggered}.");


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

        // [Test]
        // public void WhenParentWithNecessaryDependents_DependentResultsAreTriggered()
        // {
        //     IFactor             parentFactor     = parentFactory.CreateInstance();
        //     IncrementingProcess necessaryProcess = new IncrementingProcess(parentFactor);
        //     IResult             necessaryResult  = resultFactory.CreateInstance_WhoseUpdateCalls(necessaryProcess);
        //     IncrementingProcess reflexiveProcess = new IncrementingProcess(necessaryResult);
        //     IResult             reflexiveResult  = resultFactory.CreateInstance_WhoseUpdateCalls(reflexiveProcess);
        //
        //     necessaryResult.ForceReaction();
        //     reflexiveResult.ForceReaction();
        //     
        //     Assert.That(necessaryResult.IsValid,                Is.True);
        //     Assert.That(reflexiveResult.IsValid,                Is.True);
        //     Assert.That(parentFactor.HasSubscribers,            Is.True);
        //     Assert.That(necessaryResult.HasSubscribers,         Is.True);
        //     Assert.That(necessaryProcess.NumberOfTimesExecuted, Is.EqualTo(1));
        //     Assert.That(reflexiveProcess.NumberOfTimesExecuted, Is.EqualTo(1));
        //     
        //     reflexiveResult.IsReflexive = true;
        //     
        //     Assert.That(necessaryResult.IsNecessary, Is.True);
        //     
        //     parentFactor.TriggerSubscribers();
        //     
        //     Assert.That(necessaryResult.IsValid,                Is.True);
        //     Assert.That(reflexiveResult.IsValid,                Is.True);
        //     Assert.That(necessaryProcess.NumberOfTimesExecuted, Is.EqualTo(2));
        //     Assert.That(reflexiveProcess.NumberOfTimesExecuted, Is.EqualTo(2));
        // }

        
        

        
        //[Test]
        public void WhenReactorWithNoNecessarySubscribersIsTriggered_SubscribersAreDestabilized()
        {
            
        }
        
        //[Test]
        public void WhenReactorWithNecessarySubscribersIsTriggered_ReactorUpdates()
        {
            
        }
        
        //[Test]
        public void WhenNecessaryDependentIsAddedToParent_ParentIsNecessary()
        {
           
        }
        //[Test]
        public void WhenAResultBecomesNecessary_ItOnlyNotifiesTheParentItIsNecessaryIfTheResultNeedsToBeUpdated()
        {
            
        }
        
        public void WhenInvalidReactiveIntendsToUpdate_ParentReactivesAreUpdatedFirst()
        {
            
        }
        
        //[Test]
        public void AttemptReaction_ReturnsFalse_IfReactorCannotReact()
        {
            
        }
        
        //[Test]
        public void Reconcile_Fails_IfXXX()
        {
            
        }
        
        //- Not enough access to test
        
        
        // [Test]
        // public void AfterReacting_DoesNotReactAgainWithoutBeingTriggered()
        // {
        //     var      process      = new IncrementingProcess();
        //     TReactor resultToTest = resultFactory.CreateInstance_WhoseUpdateCalls(process);
        //     
        //     
        //     Assert.That(resultToTest.CanReact,         Is.False);
        //     Assert.That(process.NumberOfTimesExecuted, Is.EqualTo(0));
        //
        //     resultToTest.ForceReaction();
        //     
        //     Assert.That(process.NumberOfTimesExecuted, Is.EqualTo(1));
        //
        //     resultToTest.Reconcile();
        //     resultToTest.Reconcile();
        //     resultToTest.Reconcile();
        //     resultToTest.Reconcile();
        //     
        //     Assert.That(process.NumberOfTimesExecuted, Is.EqualTo(1));
        // }
        
        
        //- No Longer Relevant

        public void IfAThreadTriesToUpdateWhileAnotherThreadIsAlreadyUpdating_UpdateWaitsUntilThePriorThreadFinishes()
        {
            // Reactive<int> reactiveToTest = new Reactive<int>();
        }

        #endregion
    }
}