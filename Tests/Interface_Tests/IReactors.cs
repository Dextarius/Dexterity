using Core.Factors;
using Core.States;
using Factors;
using Factors.Cores.DirectReactorCores;
using Factors.Cores.ObservedReactorCores;
using NUnit.Framework;
using Tests.Tools;
using Tests.Tools.Factories;
using Tests.Tools.Interfaces;
using Tests.Tools.Mocks;
using Tests.Tools.Mocks.Processes;
using static Core.Tools.Types;
using static Tests.Tools.Tools;

namespace Tests.Interface_Tests
{
    [TestFixture(typeof(DirectFunctionResult_Int_Factory),   typeof(DirectFunctionResult_Int_Factory),   typeof(Reactive<int>))]
    [TestFixture(typeof(DirectActionResponse_Int_Factory),   typeof(DirectActionResponse_Int_Factory),   typeof(Reaction))]
    [TestFixture(typeof(ObservedFunctionResult_Int_Factory), typeof(ObservedFunctionResult_Int_Factory), typeof(Reactive<int>))]
    [TestFixture(typeof(ObservedActionResponse_Factory),     typeof(ObservedActionResponse_Factory),     typeof(Reaction))]
    public class IReactors<TReactorFactory, TSubscriberFactory, TSubscriber>
        where TReactorFactory    : IFactory<IReactor>, new()
        where TSubscriber        : IFactorSubscriber, ITriggeredState
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
            IReactor reactorBeingTested = reactorFactory.CreateInstance();

            if (reactorBeingTested.HasBeenTriggered is false)
            {
                reactorBeingTested.Trigger();

                if (reactorBeingTested.HasBeenTriggered is false)
                {
                    Assert.Inconclusive($"The {nameof(IReactor)} could not be put in a triggered state.");
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
        public void WhenValueForIsReflexiveIsSet_IsReflexiveMatchesSetValue()
        {
            IReactor reactorBeingTested = reactorFactory.CreateInstance();

            Assert.That(reactorBeingTested.IsReflexive,  Is.False,
                $"The {NameOf<IReactor>()} was marked as reflexive, despite that option not being set. ");

            reactorBeingTested.IsReflexive = true;
                
            Assert.That(reactorBeingTested.IsReflexive,
                $"The {NameOf<IReactor>()} was marked as not being reflexive after setting that option to true. ");
                
            reactorBeingTested.IsReflexive = false;
                
            Assert.That(reactorBeingTested.IsReflexive,  Is.False,
                $"The {NameOf<IReactor>()} was marked as being reflexive after setting that option to false. ");
        }
        
        [Test]
        public void WhenTriggered_AndNotNecessary_AllSubscribersAreDestabilized()
        {
            IReactor      reactorBeingTested  = reactorFactory.CreateStableInstance();
            int           numberOfSubscribers = 10;
            TSubscriber[] subscribers         = AddSubscribersTo(reactorBeingTested, numberOfSubscribers, subscriberFactory);

            Assert.That(reactorBeingTested.IsReflexive, Is.False);
            Assert.That(reactorBeingTested.IsNecessary, Is.False);
            Assert.That(reactorBeingTested.HasBeenTriggered, Is.False);
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
            IReactor      reactorBeingTested  = reactorFactory.CreateInstance();
            int           numberOfSubscribers = 10;
            TSubscriber[] subscribers;

            reactorBeingTested.ForceReaction();
            Assert.That(reactorBeingTested.HasBeenTriggered, Is.False);
            
            subscribers = AddSubscribersTo(reactorBeingTested, numberOfSubscribers, subscriberFactory);
            reactorBeingTested.Destabilize(null);

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
            IReactor    reactorToTest = reactorFactory.CreateInstance();
            TSubscriber subscriber    = subscriberFactory.CreateInstance();
            int         originalNumberOfSubscribers;
            int         expectedNumberOfSubscribers;
            int         actualNumberOfSubscribers;

            if (reactorToTest.HasBeenTriggered is false)
            {
                reactorToTest.Trigger();
                Assert.That(reactorToTest.HasBeenTriggered, Is.True);
            }
            
            originalNumberOfSubscribers = reactorToTest.NumberOfSubscribers;
            expectedNumberOfSubscribers = originalNumberOfSubscribers + 1;
            reactorToTest.Subscribe(subscriber, false);
            actualNumberOfSubscribers = reactorToTest.NumberOfSubscribers;
            
            Assert.That(actualNumberOfSubscribers, Is.EqualTo(expectedNumberOfSubscribers));
            WriteExpectedAndActualValuesToTestContext(expectedNumberOfSubscribers, actualNumberOfSubscribers);
        }
        
        [Test]
        public void IfReactorIsAlreadyUnstableWhenSubscribedTo_SubscriberIsStillAdded()
        {
            IReactor    reactorToTest = reactorFactory.CreateStableInstance();
            TSubscriber subscriber    = subscriberFactory.CreateInstance();
            int         originalNumberOfSubscribers;
            int         expectedNumberOfSubscribers;
            int         actualNumberOfSubscribers;

            reactorToTest.Destabilize(null);
            Assert.That(reactorToTest.IsUnstable, Is.True);

            originalNumberOfSubscribers = reactorToTest.NumberOfSubscribers;
            expectedNumberOfSubscribers = originalNumberOfSubscribers + 1;
            reactorToTest.Subscribe(subscriber, false);
            actualNumberOfSubscribers = reactorToTest.NumberOfSubscribers;
            
            Assert.That(actualNumberOfSubscribers, Is.EqualTo(expectedNumberOfSubscribers));
            WriteExpectedAndActualValuesToTestContext(expectedNumberOfSubscribers, actualNumberOfSubscribers);
        }

        [Test]
        public void WhenSubscribedTo_IfReactorIsTriggered_SubscriberIsDestabilized()
        {
            IReactor reactorToTest = reactorFactory.CreateInstance();
            var      subscriber    = subscriberFactory.CreateStableInstance();

            reactorToTest.Trigger();
            Assert.That(reactorToTest.HasBeenTriggered, Is.True);

            reactorToTest.Subscribe(subscriber, false);
            Assert.That(subscriber.IsUnstable, Is.True);
        }
        
        [Test]
        public void WhenSubscribedTo_IfReactorIsUnstable_SubscriberIsDestabilized()
        {
            IReactor reactorToTest = reactorFactory.CreateStableInstance();
            var      subscriber    = subscriberFactory.CreateStableInstance();

            reactorToTest.Destabilize(null);
            Assert.That(reactorToTest.IsUnstable, Is.True);

            Assert.That(subscriber.IsUnstable, Is.False);
            reactorToTest.Subscribe(subscriber, false);
            Assert.That(subscriber.IsUnstable, Is.True);
        }

        [Test]
        public void WhenReactorWithNecessaryDependentsIsTriggered_ReactorAutomaticallyUpdates()
        {
            var  reactorToTest               = reactorFactory.CreateStableInstance();
            var  subscriber                  = subscriberFactory.CreateStableInstance();
            uint initialNumberOfTimesReacted = reactorToTest.VersionNumber;
            
            Assert.That(reactorToTest.HasBeenTriggered, Is.False);
            
            reactorToTest.Subscribe(subscriber, true);
            reactorToTest.Trigger();
            
            Assert.That(reactorToTest.VersionNumber, Is.EqualTo(initialNumberOfTimesReacted + 1));
            //- TODO : Is there a better way of testing if the reaction goes off?
        }
        
        [Test]
        public void Destabilize_WhenReactorHasNecessaryDependents_ReturnsTrue()
        {
            var  reactorToTest               = reactorFactory.CreateStableInstance();
            var  subscriber                  = subscriberFactory.CreateStableInstance();
            
            Assert.That(reactorToTest.IsUnstable, Is.False);
            reactorToTest.Subscribe(subscriber, true);

            Assert.That(reactorToTest.Destabilize(null), Is.True);
            //- TODO : Is there a better way of testing if the reaction goes off?
        }
        
        [Test]
        public void IfMadeReflexive_WhileTriggered_ShouldReact()
        {
            var  reactorToTest            = reactorFactory.CreateStableInstance();
            uint initialNumberOfReactions;

            reactorToTest.IsReflexive = false;
            Assert.That(reactorToTest.IsReflexive, Is.False);
            
            reactorToTest.Trigger();
            Assert.That(reactorToTest.HasBeenTriggered, Is.True);
            
            initialNumberOfReactions  = reactorToTest.VersionNumber;
            reactorToTest.IsReflexive = true;
            
            Assert.That(reactorToTest.VersionNumber, Is.EqualTo(initialNumberOfReactions + 1));
        }

        
        [Test]
        public void IfReconcileIsCalled_WhileNotTriggeredAndNotUnstable_ReturnsTrue()
        {
            var reactorToTest = reactorFactory.CreateStableInstance();
            
            Assert.That(reactorToTest.HasBeenTriggered, Is.False);
            Assert.That(reactorToTest.IsUnstable,       Is.False);
            Assert.That(reactorToTest.Reconcile(),      Is.True);
        }
        
        // [Test]
        // public void IfReconcileIsCalled_WhileUnstable_ReturnsFalse()
        // {
        //     var reactorToTest = reactorFactory.CreateStableInstance();
        //     
        //     reactorToTest.Destabilize(null);
        //     Assert.That(reactorToTest.IsUnstable,  Is.True);
        //     Assert.That(reactorToTest.Reconcile(), Is.False);
        // }
        
        [Test]
        public void IfReconcileIsCalled_WhileTriggered_ReturnsFalse()
        {
            var reactorToTest = reactorFactory.CreateStableInstance();

            reactorToTest.Trigger();
            Assert.That(reactorToTest.HasBeenTriggered, Is.True);
            Assert.That(reactorToTest.Reconcile(),      Is.False);
        }
        
        [Test]
        public void AttemptReaction_IfReactorCannotReact_ReturnsFalse()
        {
            var reactorToTest = reactorFactory.CreateStableInstance();

            Assert.That(reactorToTest.HasBeenTriggered,  Is.False);
            Assert.That(reactorToTest.IsUnstable,        Is.False);
            Assert.That(reactorToTest.AttemptReaction(), Is.False);
        }
        
        [Test]
        public void AttemptReaction_IfCanReact_ReturnsTrue()
        {
            var reactorToTest = reactorFactory.CreateStableInstance();

            reactorToTest.Trigger();
            Assert.That(reactorToTest.HasBeenTriggered,  Is.True);
            Assert.That(reactorToTest.AttemptReaction(), Is.True);
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
        
        //[Test]
        public void WhenTriggeredReactorIntendsToUpdate_ParentReactorsAreUpdatedFirst()
        {
            
        }
        
        //[Test]
        public void AttemptReaction_WhenUnstable_ReactsAndReturnsTrue()
        {
            
        }
        
        //[Test]

        public void AttemptReaction_WhenTriggered_ReactsAndReturnsTrue()
        {
            
        }
        
        //[Test]
        public void AttemptReaction_WhenNotTriggeredOrUnstable_ReturnsFalse()
        {
            
        }
        
        //[Test]
        public void AttemptReaction_WhenSuccessfullyStabilized_ReturnsFalse()
        {
            
        }
        
        //[Test]
        public void Reconcile_Fails_IfXXX()
        {
            
        }
        
        //v Not enough access to test 
        
        
        // [Test]
        // public void AfterReacting_DoesNotReactAgainWithoutBeingTriggered()
        // {
        //     var      process      = new IncrementingProcess();
        //     IReactor resultToTest = resultFactory.CreateInstance_WhoseUpdateCalls(process);
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
        
        
        // [Test]
        // public void IfMadeReflexive_WhileUnstable_ShouldStabilizeOrReact()
        // {
        //     var  reactorToTest = reactorFactory.CreateStableInstance();
        //     uint initialNumberOfReactions;
        //
        //     reactorToTest.IsReflexive = false;
        //     Assert.That(reactorToTest.IsReflexive, Is.False);
        //     
        //     reactorToTest.Destabilize();
        //     Assert.That(reactorToTest.IsUnstable, Is.True);
        //     
        //     initialNumberOfReactions  = reactorToTest.VersionNumber;
        //     reactorToTest.IsReflexive = true;
        //     
        //     Assert.That(reactorToTest.VersionNumber, Is.EqualTo(initialNumberOfReactions + 1));
        //     
        //     //- The reactor is just going to stabilize, and without owning the Triggers we can't
        //     //  confirm Reconcile is called or control its return value.
        // }
        
        
        //v No Longer Relevant

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
        
        // public void IfAThreadTriesToUpdateWhileAnotherThreadIsAlreadyUpdating_UpdateWaitsUntilThePriorThreadFinishes()
        // {
        //     // Reactive<int> reactiveToTest = new Reactive<int>();
        // }

        #endregion
    }
}