using Core.Factors;
using Core.States;
using Factors;
using Factors.Cores.DirectReactorCores;
using Factors.Cores.ObservedReactorCores;
using Factors.Cores.ProactiveCores;
using NUnit.Framework;
using Tests.Tools.Factories.Controllers;
using Tests.Tools.Interfaces;
using Tests.Tools.Mocks.Processes;
using static Core.Tools.Types;
using static Tests.Tools.Tools;

namespace Tests.Interface_Tests
{
    [TestFixture(typeof(Reactive<int>),  typeof(int), typeof(Reactive_Controller<ObservedFunctionResult_Controller, int>))]
    [TestFixture(typeof(Reactive<int>),  typeof(int), typeof(Reactive_Controller<DirectFunctionResult_Controller,   int>))]
    [TestFixture(typeof(Reactive<int>),  typeof(int), typeof(Reactive_Controller<DirectFunctionResult2_Controller,  int>))]
    [TestFixture(typeof(Reactive<int>),  typeof(int), typeof(Reactive_Controller<DirectFunctionResult3_Controller,  int>))]
    [TestFixture(typeof(Proactive<int>), typeof(int), typeof(Proactive_Controller<ObservedProactiveCore_Controller, int>))]
    [TestFixture(typeof(Proactive<int>), typeof(int), typeof(Proactive_Controller<DirectProactiveCore_Controller,   int>))]
    public class IFactor_Ts<TFactor, TValue, TController>
        where TFactor            : IFactor<TValue>, IDeterminant  //- TODO : Try to separate the IDeterminant related parts
        where TController        : IFactor_T_Controller<TFactor, TValue>, new()
    //  where TSubscriber        : IFactorSubscriber
    //  where TSubscriberFactory : IFactory<TSubscriber>, new()
    {
        [Test]
        public void WhenGivenAValueNotEqualToCurrentValue_SubscribersAreTriggered()
        {
            TController controller               = new TController();
            TFactor     factorBeingTested        = controller.ControlledInstance;
            TValue      initialValue             = factorBeingTested.Value;  //- Reactors need their value to be initialized.
            int         numberOfSubscribersToAdd = 10;
            var         subscribers              = AddSubscribersTo(factorBeingTested, numberOfSubscribersToAdd);
            
            foreach (var subscriber in subscribers)
            {
                Assert.That(subscriber.HasBeenTriggered, Is.False);
            }

            controller.ChangeValueToANonEqualValue();
            
            foreach (var subscriber in subscribers)
            {
                Assert.That(subscriber.HasBeenTriggered, Is.True);
            }
        }
        
        [Test]
        public void WhenGivenAValueEqualToToCurrentValue_SubscribersAreNotTriggered()
        {
            TController controller               = new TController();
            TFactor     factorBeingTested        = controller.ControlledInstance;
            TValue      initialValue             = factorBeingTested.Value; 
            int         numberOfSubscribersToAdd = 10;
            var         subscribers              = AddSubscribersTo(factorBeingTested, numberOfSubscribersToAdd);
            
            foreach (var subscriber in subscribers)
            {
                Assert.That(subscriber.HasBeenTriggered, Is.False);
            }

            controller.SetValueToAnEqualValue();
            
            foreach (var subscriber in subscribers)
            {
                Assert.That(subscriber.HasBeenTriggered, Is.False, 
                    $"Setting the value of a {NameOf<TFactor>()} triggered its subscribers even though " +
                     "the value set was equal to the old value. ");
            }
        }
        
        [Test]
        public void WhenSetToANewValue_SubscribersAreNotRemoved()
        {
            TController controller                 = new TController();
            TFactor     factorBeingTested          = controller.ControlledInstance;
            TValue      initialValue               = factorBeingTested.Value;
            int         initialNumberOfSubscribers = factorBeingTested.NumberOfSubscribers;
            int         numberOfSubscribersToAdd   = 10;
            var         subscribers                = AddSubscribersTo(factorBeingTested, numberOfSubscribersToAdd);
            
            Assert.That(factorBeingTested.HasSubscribers, Is.True);
            Assert.That(factorBeingTested.NumberOfSubscribers, 
                        Is.EqualTo(initialNumberOfSubscribers + numberOfSubscribersToAdd));
        
            controller.ChangeValueToANonEqualValue();
        
            Assert.That(factorBeingTested.HasSubscribers,      Is.True);
            Assert.That(factorBeingTested.NumberOfSubscribers, 
                Is.EqualTo(initialNumberOfSubscribers + numberOfSubscribersToAdd),
                $"Setting the value of a {NameOf<TFactor>()} removed one or more of its subscribers. ");
        }
        
        [Test]
        public void WhenNewValueIsSetToAValueEqualToCurrentValue_SubscribersAreNotRemoved()
        {
            TController controller                 = new TController();
            TFactor     factorBeingTested          = controller.ControlledInstance;
            TValue      initialValue               = factorBeingTested.Value;
            int         initialNumberOfSubscribers = factorBeingTested.NumberOfSubscribers;
            int         numberOfSubscribersToAdd   = 10;
            var         subscribers                = AddSubscribersTo(factorBeingTested, numberOfSubscribersToAdd);
            
            Assert.That(factorBeingTested.HasSubscribers, Is.True);
            Assert.That(factorBeingTested.NumberOfSubscribers, 
                Is.EqualTo(initialNumberOfSubscribers + numberOfSubscribersToAdd));

            controller.SetValueToAnEqualValue();

            Assert.That(factorBeingTested.HasSubscribers, Is.True);
            Assert.That(factorBeingTested.NumberOfSubscribers, 
                Is.EqualTo(initialNumberOfSubscribers + numberOfSubscribersToAdd),
                $"Setting the value of a {NameOf<TFactor>()} removed one or more of its subscribers " +
                 "even though the value set was equal to the old value. \n" +
                $"# Remaining Subscribers => {factorBeingTested.NumberOfSubscribers} ");
        }
    }
}