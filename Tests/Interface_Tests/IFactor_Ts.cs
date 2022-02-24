using Core.Factors;
using Core.States;
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
    [TestFixture(typeof(ObservedFunctionResult<int>),              typeof(int), typeof(ObservedFunctionResult_Controller))]
    [TestFixture(typeof(DirectFunctionResult<int, int>),           typeof(int), typeof(DirectFunctionResult_Controller))]
    [TestFixture(typeof(DirectFunctionResult<int, int, int>),      typeof(int), typeof(DirectFunctionResult2_Controller))]
    [TestFixture(typeof(DirectFunctionResult<int, int, int, int>), typeof(int), typeof(DirectFunctionResult3_Controller))]
    [TestFixture(typeof(ObservedStateCore<int>),                   typeof(int), typeof(ObservedState_Controller))]
    [TestFixture(typeof(DirectStateCore<int>),                     typeof(int), typeof(DirectState_Controller))]
    public class IFactor_Ts<TFactor, TValue, TController>
        where TFactor            : IFactor<TValue>
        where TController        : IFactor_T_Controller<TFactor, TValue>, new()
    //  where TSubscriber        : IFactorSubscriber
    //  where TSubscriberFactory : IFactory<TSubscriber>, new()
    {
        [Test]
        public void WhenGivenAValueNotEqualToToCurrentValue_SubscribersAreTriggered()
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

            controller.ChangeValueToAnEqualValue();
            
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

            controller.ChangeValueToAnEqualValue();

            Assert.That(factorBeingTested.HasSubscribers, Is.True);
            Assert.That(factorBeingTested.NumberOfSubscribers, 
                Is.EqualTo(initialNumberOfSubscribers + numberOfSubscribersToAdd),
                $"Setting the value of a {NameOf<TFactor>()} removed one or more of its subscribers " +
                 "even though the value set was equal to the old value. \n" +
                $"# Remaining Subscribers => {factorBeingTested.NumberOfSubscribers} ");
        }
    }
}