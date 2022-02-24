using System;
using Core.Factors;
using Factors;
using NUnit.Framework;
using Tests.Tools;
using Tests.Tools.Interfaces;
using Tests.Tools.Mocks.Processes;

namespace Tests.Class_Tests
{
    public class Reactives<TReactive, TManipulable, TSubscriber, TReactiveFactory, TSubscriberFactory, TValue>  
        where TReactive          : IReactive<TValue>
        where TManipulable       : IReactive_Controller<TReactive, TValue>, new()
        where TReactiveFactory   : new()
        where TSubscriberFactory : new()
    {
        #region Instance Fields

        private TReactiveFactory   reactiveFactory   = new TReactiveFactory();
        private TSubscriberFactory subscriberFactory = new TSubscriberFactory();

        #endregion

        
        #region Tests

        [Test]
        public void Constructor_WhenGivenNullDelegate_ThrowsException() => 
            Assert.Throws<ArgumentNullException>(() => new Reactive<int>((Func<int>) null));
        
        
        [Test]
        public void WhenValueRetrieved_MatchesExpectedValue()
        {
            TManipulable controller          = new TManipulable();
            TReactive    reactiveBeingTested = controller.ControlledInstance;
            TValue       expectedValue       = controller.ExpectedValue;
            TValue       actualValue         = reactiveBeingTested.Value; 
            
            Assert.That(actualValue, Is.EqualTo(expectedValue));
            TestContext.WriteLine($"Expected Value {expectedValue}, Actual Value {actualValue}");
        }
        
        // [Test]
        // public void IfRecalculatingReturnsAValueEqualToCurrentValue_SubscribersAreNotInvalidated()
        // {
        //     TValue        initialValue        = reactiveFactory.CreateRandomInstanceOfValuesType();
        //     var           valueProcess        = new StoredValueProcess<TValue>(initialValue);
        //     TResult       outcomeBeingTested  = reactiveFactory.CreateInstance_WhoseUpdateCalls(valueProcess);
        //     int           numberOfSubscribers = 10;
        //     TSubscriber[] subscribers         = new TSubscriber[numberOfSubscribers];
        //
        //     Assert.That(outcomeBeingTested.HasSubscribers, Is.False,
        //         $"The {NameOf<TResult>()} was marked as consequential before being used. ");
        //     
        //     AssumeHasNoSubscribers(outcomeBeingTested);
        //
        //     for (int i = 0; i < numberOfSubscribers; i++)
        //     {
        //         var createdSubscriber = subscriberFactory.CreateInstance();
        //
        //         subscribers[i] = createdSubscriber;
        //         outcomeBeingTested.Subscribe(createdSubscriber);
        //         Assert.That(createdSubscriber.IsValid, Is.True);
        //     }
        //
        //     AssumeHasSpecificNumberOfSubscribers(outcomeBeingTested, numberOfSubscribers);
        //
        //     outcomeBeingTested.ForceReaction();
        //     
        //     for (int i = 0; i < numberOfSubscribers; i++)
        //     {
        //         var createdSubscriber = subscriberFactory.CreateInstance();
        //
        //         subscribers[i] = createdSubscriber;
        //         outcomeBeingTested.Subscribe(createdSubscriber);
        //         Assert.That(createdSubscriber.IsValid, Is.True);
        //     }
        // }
        
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

        public void BeforeReturningValue_AttemptsReaction()
        {
            
        }

        #endregion
    }
}