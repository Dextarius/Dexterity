using Core.Factors;
using Core.States;
using Factors.Cores.ObservedReactorCores;
using Factors.Cores.ProactiveCores;
using NUnit.Framework;
using Tests.Tools;
using Tests.Tools.Factories;
using Tests.Tools.Factories.Controllers;
using Tests.Tools.Interfaces;
using Tests.Tools.Mocks;
using static Tests.Tools.Tools;
using static Core.Tools.Types;

namespace Tests.Interface_Tests
{
    [TestFixture(typeof(ObservedStateCore_Int_Factory),      typeof(ObservedFunctionResult_Controller))]
    [TestFixture(typeof(DirectStateCore_Int_Factory),        typeof(DirectFunctionResult_Controller))]
    [TestFixture(typeof(ObservedFunctionResult_Int_Factory), typeof(ObservedFunctionResult_Controller))]
    [TestFixture(typeof(DirectFunctionResult_Int_Factory),   typeof(DirectFunctionResult_Controller))]
    public class IDeterminants<TFactorFactory, TSubscriberController>
        where TFactorFactory        : IFactory<IDeterminant>, new()
        where TSubscriberController : IFactorSubscriberController<IFactorSubscriber>, new()
    {
        #region Instance Fields

        private readonly TFactorFactory        factorFactory     = new TFactorFactory();
        private readonly TSubscriberController subscriberFactory = new TSubscriberController();

        #endregion


        #region Tests

        //- TODO : All of these subscriber methods may have to test both true and false for isNecessary now.
        [Test]
        public void WhenSubscriberAddsItself_HasSubscriberIsTrue()
        {
            var factorBeingTested = factorFactory.CreateInstance();
            var controller        = new TSubscriberController();
            var subscriber        = controller.ControlledInstance;
            
            if (factorBeingTested.HasSubscribers)
            {
                Assert.Inconclusive($"The {nameof(factorBeingTested.HasSubscribers)} property of the tested " +
                                    $"{factorBeingTested.GetType()} was already true when the object was created. ");
                //- In case the factor already has subscribers when it's created, for whatever reason.
            }

            Assert.That(factorBeingTested.NumberOfSubscribers, Is.Zero,
                ErrorMessages.SubscribersGreaterThanZero<IFactor>(
                    $"when {nameof(factorBeingTested.HasSubscribers)} is false. ", factorBeingTested.NumberOfSubscribers));

            factorBeingTested.Subscribe(subscriber, false);
            Assert.That(factorBeingTested.HasSubscribers, Is.True);
        }
        
        [Test]
        public void WhenSubscriberAddsItself_NumberOfSubscribersGoesUpByOne()
        {
            var factorBeingTested           = factorFactory.CreateInstance();
            int previousNumberOfSubscribers = factorBeingTested.NumberOfSubscribers;

            for (int i = 0; i < 100000; i++)
            {
                var controller = new TSubscriberController();
                var subscriber = controller.ControlledInstance;

                factorBeingTested.Subscribe(subscriber, false);
                
                Assert.That(factorBeingTested.HasSubscribers,      Is.True);
                Assert.That(factorBeingTested.NumberOfSubscribers, Is.EqualTo(previousNumberOfSubscribers + 1));

                previousNumberOfSubscribers++;
            }
        }
        
        [Test]
        public void WhenSubscriberTriesToAddItselfMultipleTimes_NumberOfSubscribersOnlyIncreasesOnce()
        {
            var factorBeingTested           = factorFactory.CreateInstance();
            var controller                  = new TSubscriberController();
            var subscriber                  = controller.ControlledInstance;
            int originalNumberOfSubscribers = factorBeingTested.NumberOfSubscribers;
            int expectedNumberOfSubscribers = originalNumberOfSubscribers + 1;

            for (int i = 0; i < 100000; i++)
            {
                factorBeingTested.Subscribe(subscriber, false);
            
                Assert.That(factorBeingTested.HasSubscribers,      Is.True);
                Assert.That(factorBeingTested.NumberOfSubscribers, Is.EqualTo(expectedNumberOfSubscribers));
            }
        }

        [Test]
        public void WhenTriggersSubscribersIsCalled_AllSubscribersAreTriggered()
        {
            var factorBeingTested   = factorFactory.CreateInstance();
            int          numberOfSubscribers = 10;
            var          controllers         = new TSubscriberController[numberOfSubscribers];

            for (int i = 0; i < numberOfSubscribers; i++)
            {
                var controller = new TSubscriberController();
                var subscriber = controller.ControlledInstance;

                controllers[i] = controller;
                controller.MakeStableAndUntriggered();

                Assert.That(controller.CheckIfTriggered(), Is.False);

                factorBeingTested.Subscribe(subscriber, false);
            }

            factorBeingTested.TriggerSubscribers();

            for (int i = 0; i < numberOfSubscribers; i++)
            {
                var controller = controllers[i];

                Assert.That(controller.CheckIfTriggered(), Is.True);
            }
        }

        [Test]
        public void WhenNotifiedNecessary_ByACurrentSubscriber_AlsoBecomesNecessary()
        {
            var factorToTest = factorFactory.CreateInstance();
            var subscriber   = new MockFactorSubscriber();
            
            Assert.That(factorToTest.IsNecessary, Is.False);
            Assert.That(subscriber.IsNecessary,   Is.False);
            
            factorToTest.Subscribe(subscriber, false); 
            
            Assert.That(factorToTest.IsNecessary, Is.False);

            factorToTest.NotifyNecessary(subscriber);  
            
            Assert.That(factorToTest.IsNecessary, Is.True);
        }
        
        [Test]
        public void WhenFactorIsSubscribedTo_IfSubscriberPassesTrueForIsNecessaryArg_FactorBecomesNecessary()
        {
            var factorToTest = factorFactory.CreateInstance();
            var subscriber   = new MockFactorSubscriber();
            
            Assert.That(factorToTest.IsNecessary, Is.False);
            Assert.That(subscriber.IsNecessary,   Is.False);
            
            factorToTest.Subscribe(subscriber, true); 
            
            Assert.That(factorToTest.IsNecessary, Is.True);
        }

        [Test]
        public void WhenAllNecessarySubscribersHaveUnsubscribedFromFactor_FactorIsNoLongerNecessary()
        {
            var factorToTest          = factorFactory.CreateStableInstance();
            int numberOfSubscribers   = 10;
            var subscriberControllers = new TSubscriberController[numberOfSubscribers];

            if (factorToTest.IsNecessary)
            {
                Assert.Inconclusive("The tested Factor was already Necessary before subscribers were added.");
            }
            
            for (int i = 0; i < numberOfSubscribers; i++)
            {
                var subscriberController = new TSubscriberController();
                var subscriber           = subscriberController.ControlledInstance;
                
                subscriberControllers[i] = subscriberController;
                subscriberController.MakeStableAndUntriggered();
             // controller.MakeNecessary();
             // Assert.That(subscriber.IsNecessary, Is.True);
                factorToTest.Subscribe(subscriber, true);
            }

            Assert.That(factorToTest.IsNecessary, Is.True);

            //- Unsubscribe all subscribers except the first.
            for (int i = 1; i < numberOfSubscribers; i++)
            {
                factorToTest.Unsubscribe(subscriberControllers[i].ControlledInstance);
            }
            
            //- The first subscriber should still be subscribed, so we should still be Necessary.
            Assert.That(factorToTest.IsNecessary, Is.True);
            
            //- Now we remove the only remaining subscriber.
            factorToTest.Unsubscribe(subscriberControllers[0].ControlledInstance);
            Assert.That(factorToTest.IsNecessary, Is.False);
        }

        [Test]
        public void WhenLastNecessarySubscriberNotifiesNotNecessary_IsNoLongerNecessary()
        {
            var factorToTest        = factorFactory.CreateInstance();
            int numberOfSubscribers = 10;
            var subscribers         = new MockFactorSubscriber[numberOfSubscribers];

            if (factorToTest.IsNecessary)
            {
                Assert.Inconclusive("The tested IFactor was already Necessary before subscribers were added.");
            }
            
            for (int i = 0; i < numberOfSubscribers; i++)
            {
                var createdSubscriber = new MockFactorSubscriber();
                
                subscribers[i] = createdSubscriber;
                createdSubscriber.ResetHasBeenTriggeredToFalse();
            //  createdSubscriber.MakeNecessary(); 
                factorToTest.Subscribe(createdSubscriber, true);
            }

            Assert.That(factorToTest.IsNecessary, Is.True);

            //- Notify not necessary numberOfSubscribers - 1 times.
            for (int i = 0; i < numberOfSubscribers - 1; i++)
            {
                var currentSubscriber = subscribers[i];

                factorToTest.NotifyNotNecessary(currentSubscriber);
            }
            
            Assert.That(factorToTest.IsNecessary, Is.True);
            
            factorToTest.NotifyNotNecessary(subscribers[numberOfSubscribers - 1]);
            Assert.That(factorToTest.IsNecessary, Is.False);
        }
        
        [Test]
        public void WhenSubscribersAreTriggered_IfSubscriberDoesNotRequestToBeRemoved_SubscriberIsNotRemovedFromSubscriptionList()
        {
            var factorBeingTested           = factorFactory.CreateInstance();
            int initialNumberOfSubscribers  = factorBeingTested.NumberOfSubscribers;
            int numberOfSubscribersToAdd    = 10;
            int expectedNumberOfSubscribers = initialNumberOfSubscribers + numberOfSubscribersToAdd;
            
            AddSubscribersTo(factorBeingTested, numberOfSubscribersToAdd);
            
            Assert.That(factorBeingTested.HasSubscribers,      Is.True);
            Assert.That(factorBeingTested.NumberOfSubscribers, Is.EqualTo(expectedNumberOfSubscribers));
            
            factorBeingTested.TriggerSubscribers();
            
            Assert.That(factorBeingTested.HasSubscribers, Is.True);
            Assert.That(factorBeingTested.NumberOfSubscribers, Is.EqualTo(expectedNumberOfSubscribers),
            $"Triggering the subscribers of an {nameof(IFactor)} removed one or more of those subscribers. ");
        }
        
        [Test]
        public void WhenSubscribersAreTriggered_IfSubscriberRequestsToBeRemoved_SubscriberIsRemovedFromSubscriptionList()
        {
            var factorBeingTested          = factorFactory.CreateInstance();
            int initialNumberOfSubscribers = factorBeingTested.NumberOfSubscribers;
            int numberOfSubscribersToAdd   = 10;
            var subscribers                = new MockFactorSubscriber[numberOfSubscribersToAdd];

            for (int i = 0; i < numberOfSubscribersToAdd; i++)
            {
                var createdSubscriber = new MockFactorSubscriber();
                
                subscribers[i] = createdSubscriber;
                createdSubscriber.ResetHasBeenTriggeredToFalse();
                createdSubscriber.RemoveSubscriptionOnTrigger = true;
                factorBeingTested.Subscribe(createdSubscriber, false);
            }
            
            Assert.That(factorBeingTested.NumberOfSubscribers, 
                Is.EqualTo(initialNumberOfSubscribers + numberOfSubscribersToAdd));
            
            factorBeingTested.TriggerSubscribers();
            
            Assert.That(factorBeingTested.NumberOfSubscribers, 
                Is.EqualTo(initialNumberOfSubscribers),
                $"One or more of an {nameof(IDeterminant)}s subscribers were not removed when triggered, even though they " +
                 "indicated they wished to be removed from the subscription list. ");
        }

       // [Test]
        public void WhenTriggeringSubscribers_IfSubscriberRemovesItself_ExceptionIsNotThrown()
        {
            //- A test to make sure whatever process the Factor goes through to trigger it's subscribers
            //  doesn't use a foreach loop or something else that will throw when the collection of subscribers
            //  is changed.
        }
        
        // [Test]
        public void TestReconcile()
        {

        }
        
        

        
        
        
        
        //- This may seem like a strange test, but the nature of Proactives is to store references to the
        //  things they affect, and the nature of Reactives to store reference to things that affect them
        //  can easily lead to a bunch of objects never being collected because they all reference each other.
        //  Both classes are structured in a way that prevents them from creating circular references
        //  to each other, and this is here to make sure that is/remains true. 
        // [Test]
        // public void WhenAffectedFactorsNoLongerInUse_CanBeGarbageCollected()
        // {
        //     HashSet<WeakReference<Reactive<int>>> reactiveReferences = new HashSet<WeakReference<Reactive<int>>>();
        //     WeakReference<Proactive<int>>         proactiveReference = GenerateChainOfFactors(reactiveReferences);
        //
        //     GC.Collect();
        //     Thread.Sleep(1000);
        //
        //     foreach (var reference in reactiveReferences)
        //     {
        //         reference.TryGetTarget(out var reactive);
        //         Assert.That(reactive, Is.Null);
        //     }
        //
        //     proactiveReference.TryGetTarget(out var proactive);
        //     Assert.That(proactive, Is.Null);
        // }
        //
        //
        // [MethodImpl(MethodImplOptions.NoInlining)]
        // private static WeakReference<Proactive<int>> GenerateChainOfFactors(HashSet<WeakReference<Reactive<int>>> references)
        // {
        //     Proactive<int>                proactiveValue       = new Proactive<int>(5);
        //     WeakReference<Proactive<int>> referenceToProactive = new WeakReference<Proactive<int>>(proactiveValue);
        //     Reactive<int>                 createdReactive      = CreateReactiveThatGetsValueOf(proactiveValue);
        //     int                           triggerAReaction     = createdReactive.Value;
        //
        //     for (int i = 0; i < 3; i++)
        //     {
        //         createdReactive = CreateReactiveThatDependsOn(createdReactive);
        //         references.Add(new WeakReference<Reactive<int>>(createdReactive));
        //     }
        //
        //     foreach (var reference in references)
        //     {
        //         reference.TryGetTarget(out var reactive);
        //         Assert.That(reactive, Is.Not.Null);
        //     }
        //
        //     Assert.That(proactiveValue.HasSubscribers);
        //
        //     return referenceToProactive;
        // }
        
        #region Outdated Tests

        // [Test]
        // public void WhenChanged_InvalidatesDependents()
        // {
        //     IFactor factorBeingTested = factory.CreateInstance();
        //     var     interaction       = new MockDependent();
        //
        //     factorBeingTested.AddDependent(interaction);
        //     Assert.That(interaction.IsValid, Is.True);
        //
        //     factorBeingTested..NotifyChanged();
        //     Assert.That(interaction.IsValid, Is.False);
        // }

        #endregion

        #endregion
    }
    
    
    
    //     [TestFixture(typeof(ObservedStateCore_Int_Factory),      typeof(ObservedFunctionResult_Int_Factory))]
    // [TestFixture(typeof(DirectStateCore_Int_Factory),        typeof(DirectFunctionResult_Int_Factory))]
    // [TestFixture(typeof(ObservedFunctionResult_Int_Factory), typeof(ObservedFunctionResult_Int_Factory))]
    // [TestFixture(typeof(DirectFunctionResult_Int_Factory),   typeof(DirectFunctionResult_Int_Factory))]
    // public class IFactors<TFactorFactory, TSubscriberFactory>
    //     where TFactorFactory     : IFactory<IFactor>, new()
    //     where TSubscriberFactory : IFactorSubscriberFactory<IFactorSubscriber>, new()
    // {
    //     #region Instance Fields
    //
    //     private readonly TFactorFactory     factorFactory     = new TFactorFactory();
    //     private readonly TSubscriberFactory subscriberFactory = new TSubscriberFactory();
    //
    //     #endregion
    //
    //
    //     #region Tests
    //
    //     [Test]
    //     public void WhenSubscriberAddsItself_HasSubscriberIsTrue()
    //     {
    //         IFactor factorBeingTested = factorFactory.CreateInstance();
    //         var     subscriber        = subscriberFactory.CreateInstance();
    //         
    //         if (factorBeingTested.HasSubscribers)
    //         {
    //             Assert.Inconclusive($"The {nameof(factorBeingTested.HasSubscribers)} property of the tested " +
    //                                 $"{factorBeingTested.GetType()} was already true when the object was created. ");
    //             //- In case the factor already has subscribers when it's created, for whatever reason.
    //         }
    //
    //         Assert.That(factorBeingTested.NumberOfSubscribers, Is.Zero,
    //             ErrorMessages.SubscribersGreaterThanZero<IFactor>(
    //                 $"when {nameof(factorBeingTested.HasSubscribers)} is false. ", factorBeingTested.NumberOfSubscribers));
    //
    //         factorBeingTested.Subscribe(subscriber);
    //         Assert.That(factorBeingTested.HasSubscribers, Is.True);
    //     }
    //     
    //     [Test]
    //     public void WhenSubscriberAddsItself_NumberOfSubscribersGoesUpByOne()
    //     {
    //         IFactor factorBeingTested           = factorFactory.CreateInstance();
    //         int     previousNumberOfSubscribers = factorBeingTested.NumberOfSubscribers;
    //
    //         for (int i = 0; i < 100000; i++)
    //         {
    //             var subscriber = subscriberFactory.CreateInstance();
    //
    //             factorBeingTested.Subscribe(subscriber);
    //             
    //             Assert.That(factorBeingTested.HasSubscribers,      Is.True);
    //             Assert.That(factorBeingTested.NumberOfSubscribers, Is.EqualTo(previousNumberOfSubscribers + 1));
    //
    //             previousNumberOfSubscribers++;
    //         }
    //     }
    //     
    //     [Test]
    //     public void WhenSubscriberTriesToAddItselfMultipleTimes_NumberOfSubscribersOnlyIncreasesOnce()
    //     {
    //         IFactor factorBeingTested           = factorFactory.CreateInstance();
    //         var     subscriber                  = subscriberFactory.CreateInstance();
    //         int     originalNumberOfSubscribers = factorBeingTested.NumberOfSubscribers;
    //         int     expectedNumberOfSubscribers = originalNumberOfSubscribers + 1;
    //
    //         for (int i = 0; i < 100000; i++)
    //         {
    //             factorBeingTested.Subscribe(subscriber);
    //         
    //             Assert.That(factorBeingTested.HasSubscribers,      Is.True);
    //             Assert.That(factorBeingTested.NumberOfSubscribers, Is.EqualTo(expectedNumberOfSubscribers));
    //         }
    //     }
    //
    //     [Test]
    //     public void WhenTriggersSubscribersIsCalled_AllSubscribersAreTriggered()
    //     {
    //         IFactor factorBeingTested   = factorFactory.CreateInstance();
    //         int     numberOfSubscribers = 10;
    //         var     subscribers         = new IFactorSubscriber[numberOfSubscribers];
    //         
    //         for (int i = 0; i < numberOfSubscribers; i++)
    //         {
    //             var createdSubscriber = new MockFactorSubscriber();
    //             
    //             subscribers[i] = createdSubscriber;
    //             createdSubscriber.ResetHasBeenTriggeredToFalse();
    //             factorBeingTested.Subscribe(createdSubscriber);
    //         }
    //         
    //         factorBeingTested.TriggerSubscribers();
    //
    //         for (int i = 0; i < numberOfSubscribers; i++)
    //         {
    //             var subscriber = subscribers[i];
    //             
    //             Assert.That(subscriber.IsTriggered, Is.True);
    //         }
    //     }
    //     
    //     [Test]
    //     public void WhenLastNecessarySubscriberUnsubscribes_IsNoLongerNecessary()
    //     {
    //         var factorToTest        = factorFactory.CreateInstance();
    //         int numberOfSubscribers = 10;
    //         var subscribers         = new MockFactorSubscriber[numberOfSubscribers];
    //
    //         if (factorToTest.IsNecessary)
    //         {
    //             Assert.Inconclusive("The tested IFactor was already Necessary before subscribers were added.");
    //         }
    //         
    //         for (int i = 0; i < numberOfSubscribers; i++)
    //         {
    //             var createdSubscriber = new MockFactorSubscriber();
    //             
    //             subscribers[i] = createdSubscriber;
    //             createdSubscriber.ResetHasBeenTriggeredToFalse();
    //             createdSubscriber.MakeNecessary(); 
    //             factorToTest.Subscribe(createdSubscriber);
    //         }
    //
    //         Assert.That(factorToTest.IsNecessary, Is.True);
    //
    //         //- Unsubscribe all subscribers except the first.
    //         for (int i = 1; i < numberOfSubscribers; i++)
    //         {
    //             factorToTest.Unsubscribe(subscribers[i]);
    //         }
    //         
    //         //- The first subscriber should still be subscribed, so we should still be Necessary.
    //         Assert.That(factorToTest.IsNecessary, Is.True);
    //         
    //         //- Now we remove the only remaining subscriber.
    //         factorToTest.Unsubscribe(subscribers[0]);
    //         Assert.That(factorToTest.IsNecessary, Is.False);
    //     }
    //
    //     [Test]
    //     public void WhenLastNecessarySubscriberNotifiesNotNecessary_IsNoLongerNecessary()
    //     {
    //         var factorToTest        = factorFactory.CreateInstance();
    //         int numberOfSubscribers = 10;
    //         var subscribers         = new MockFactorSubscriber[numberOfSubscribers];
    //
    //         if (factorToTest.IsNecessary)
    //         {
    //             Assert.Inconclusive("The tested IFactor was already Necessary before subscribers were added.");
    //         }
    //         
    //         for (int i = 0; i < numberOfSubscribers; i++)
    //         {
    //             var createdSubscriber = new MockFactorSubscriber();
    //             
    //             subscribers[i] = createdSubscriber;
    //             createdSubscriber.ResetHasBeenTriggeredToFalse();
    //             createdSubscriber.MakeNecessary(); 
    //             factorToTest.Subscribe(createdSubscriber);
    //         }
    //
    //         Assert.That(factorToTest.IsNecessary, Is.True);
    //
    //         //- Notify not necessary numberOfSubscribers - 1 times.
    //         for (int i = 0; i < numberOfSubscribers - 1; i++)
    //         {
    //             factorToTest.NotifyNotNecessary();
    //         }
    //         
    //         Assert.That(factorToTest.IsNecessary, Is.True);
    //         
    //         factorToTest.NotifyNotNecessary();
    //         Assert.That(factorToTest.IsNecessary, Is.False);
    //     }
    //     
    //     [Test]
    //     public void WhenNotifiedNecessaryByASubscriber_IsNecessary()
    //     {
    //         var factorToTest = factorFactory.CreateInstance();
    //         var subscriber   = new MockFactorSubscriber();
    //         
    //         Assert.That(factorToTest.IsNecessary, Is.False);
    //         Assert.That(subscriber.IsNecessary,   Is.False);
    //         
    //         factorToTest.Subscribe(subscriber); 
    //         factorToTest.NotifyNecessary();  
    //         
    //         Assert.That(factorToTest.IsNecessary, Is.True);
    //     }
    //
    //     public void WhenSubscribersAreTriggered_IfSubscriberDoesNotRequestToBeRemoved_SubscriberIsNotRemovedFromSubscriptionList()
    //     {
    //         var factorBeingTested          = factorFactory.CreateInstance();
    //         int initialNumberOfSubscribers = factorBeingTested.NumberOfSubscribers;
    //         int numberOfSubscribersToAdd   = 10;
    //         var subscribers                = AddSubscribersTo(factorBeingTested, numberOfSubscribersToAdd);
    //         
    //         Assert.That(factorBeingTested.HasSubscribers, Is.True);
    //         Assert.That(factorBeingTested.NumberOfSubscribers, 
    //             Is.EqualTo(initialNumberOfSubscribers + numberOfSubscribersToAdd));
    //         
    //         factorBeingTested.TriggerSubscribers();
    //         
    //         Assert.That(factorBeingTested.HasSubscribers, Is.True);
    //         Assert.That(factorBeingTested.NumberOfSubscribers, 
    //             Is.EqualTo(initialNumberOfSubscribers + numberOfSubscribersToAdd),
    //         $"Triggering the subscribers of an {nameof(IFactor)} removed one or more of those subscribers. ");
    //     }
    //     
    //     [Test]
    //     public void WhenSubscribersAreTriggered_IfSubscriberRequestsToBeRemoved_SubscriberIsRemovedFromSubscriptionList()
    //     {
    //         var factorBeingTested          = factorFactory.CreateInstance();
    //         int initialNumberOfSubscribers = factorBeingTested.NumberOfSubscribers;
    //         int numberOfSubscribersToAdd   = 10;
    //         var subscribers                = new MockFactorSubscriber[numberOfSubscribersToAdd];
    //
    //         for (int i = 0; i < numberOfSubscribersToAdd; i++)
    //         {
    //             var createdSubscriber = new MockFactorSubscriber();
    //             
    //             subscribers[i] = createdSubscriber;
    //             createdSubscriber.ResetHasBeenTriggeredToFalse();
    //             createdSubscriber.RemoveSubscriptionOnTrigger = true;
    //             factorBeingTested.Subscribe(createdSubscriber);
    //         }
    //         
    //         Assert.That(factorBeingTested.NumberOfSubscribers, 
    //             Is.EqualTo(initialNumberOfSubscribers + numberOfSubscribersToAdd));
    //         
    //         factorBeingTested.TriggerSubscribers();
    //         
    //         Assert.That(factorBeingTested.NumberOfSubscribers, 
    //             Is.EqualTo(initialNumberOfSubscribers),
    //             $"One or more of an {nameof(IFactor)}s subscribers were not removed when triggered, even though they " +
    //              "indicated they wished to be removed from the subscription list. ");
    //     }
    //     
    //     
    //
    //     
    //     
    //     
    //     
    //     //- This may seem like a strange test, but the nature of Proactives is to store references to the
    //     //  things they affect, and the nature of Reactives to store reference to things that affect them
    //     //  can easily lead to a bunch of objects never being collected because they all reference each other.
    //     //  Both classes are structured in a way that prevents them from creating circular references
    //     //  to each other, and this is here to make sure that is/remains true. 
    //     // [Test]
    //     // public void WhenAffectedFactorsNoLongerInUse_CanBeGarbageCollected()
    //     // {
    //     //     HashSet<WeakReference<Reactive<int>>> reactiveReferences = new HashSet<WeakReference<Reactive<int>>>();
    //     //     WeakReference<Proactive<int>>         proactiveReference = GenerateChainOfFactors(reactiveReferences);
    //     //
    //     //     GC.Collect();
    //     //     Thread.Sleep(1000);
    //     //
    //     //     foreach (var reference in reactiveReferences)
    //     //     {
    //     //         reference.TryGetTarget(out var reactive);
    //     //         Assert.That(reactive, Is.Null);
    //     //     }
    //     //
    //     //     proactiveReference.TryGetTarget(out var proactive);
    //     //     Assert.That(proactive, Is.Null);
    //     // }
    //     //
    //     //
    //     // [MethodImpl(MethodImplOptions.NoInlining)]
    //     // private static WeakReference<Proactive<int>> GenerateChainOfFactors(HashSet<WeakReference<Reactive<int>>> references)
    //     // {
    //     //     Proactive<int>                proactiveValue       = new Proactive<int>(5);
    //     //     WeakReference<Proactive<int>> referenceToProactive = new WeakReference<Proactive<int>>(proactiveValue);
    //     //     Reactive<int>                 createdReactive      = CreateReactiveThatGetsValueOf(proactiveValue);
    //     //     int                           triggerAReaction     = createdReactive.Value;
    //     //
    //     //     for (int i = 0; i < 3; i++)
    //     //     {
    //     //         createdReactive = CreateReactiveThatDependsOn(createdReactive);
    //     //         references.Add(new WeakReference<Reactive<int>>(createdReactive));
    //     //     }
    //     //
    //     //     foreach (var reference in references)
    //     //     {
    //     //         reference.TryGetTarget(out var reactive);
    //     //         Assert.That(reactive, Is.Not.Null);
    //     //     }
    //     //
    //     //     Assert.That(proactiveValue.HasSubscribers);
    //     //
    //     //     return referenceToProactive;
    //     // }
    //     
    //     #region Outdated Tests
    //
    //     // [Test]
    //     // public void WhenChanged_InvalidatesDependents()
    //     // {
    //     //     IFactor factorBeingTested = factory.CreateInstance();
    //     //     var     interaction       = new MockDependent();
    //     //
    //     //     factorBeingTested.AddDependent(interaction);
    //     //     Assert.That(interaction.IsValid, Is.True);
    //     //
    //     //     factorBeingTested..NotifyChanged();
    //     //     Assert.That(interaction.IsValid, Is.False);
    //     // }
    //
    //     #endregion
    //
    //     #endregion
    // }
}