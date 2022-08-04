using static Tests.Tools.Tools;
using        Core.Factors;
using        Factors.Modifiers;
using        NUnit.Framework;
using        Tests.Tools.Interfaces;

namespace Tests.Class_Tests
{
    public class ModifiableNumbers<TModifiable, TController, TSubscriber, TReactiveFactory, TSubscriberFactory>  
        where TModifiable        : IModifiableNumber
        where TController        : IFactor_T_Controller<TModifiable, double>, new()
        where TReactiveFactory   : new()
        where TSubscriberFactory : new()
    {
        public void WhenModifiedValueIsRetrieved_WhileFactorIsInATriggeredState_Reacts()
        {
            TController controller        = new TController();
            TModifiable factorBeingTested = controller.ControlledInstance;
            uint        versionNumber    = factorBeingTested.VersionNumber;

            if (factorBeingTested.HasBeenTriggered is false)
            {
                factorBeingTested.Trigger();
            }
            
            Assert.That(factorBeingTested.HasBeenTriggered, Is.True);
            _ = factorBeingTested.Value;
            Assert.That(factorBeingTested.HasBeenTriggered, Is.False);
            Assert.That(factorBeingTested.IsUnstable,       Is.False);
        }
        
        [Test]
        public void WhenModifierAdded_IsTriggered()
        {
            TController controller        = new TController();
            TModifiable factorBeingTested = controller.ControlledInstance;

            if (factorBeingTested.HasBeenTriggered)
            {
                _ = factorBeingTested.Value;
            }
            
         // factorBeingTested.AddModifier(new NumericModifier());

            Assert.That(factorBeingTested.HasBeenTriggered, Is.False);
            Assert.That(factorBeingTested.IsUnstable,       Is.False);
        }        
        
       // [Test]
        public void XXX()
        {
            TController controller        = new TController();
            TModifiable factorBeingTested = controller.ControlledInstance;

            if (factorBeingTested.HasBeenTriggered)
            {
                _ = factorBeingTested.Value;
            }
            
            Assert.That(factorBeingTested.HasBeenTriggered, Is.False);
            _ = factorBeingTested.Value;
            Assert.That(factorBeingTested.HasBeenTriggered, Is.False);
            Assert.That(factorBeingTested.IsUnstable,       Is.False);
        }
        
        
        [Test]
        public void WhenBaseValueIsSet_BaseValueReturnsSetValue()
        {
            TController controller        = new TController();
            TModifiable     factorBeingTested = controller.ControlledInstance;
            int         numberOfTests     = 100;

            for (int i = 0; i < numberOfTests; i++)
            {
                var previousValue = factorBeingTested.BaseValue;
                var newValue      = controller.GetRandomInstanceOfValuesType_NotEqualTo(previousValue);

                factorBeingTested.BaseValue = newValue;
                Assert.That(factorBeingTested.BaseValue, Is.EqualTo(newValue));
            }
        }
        
        [Test]
        public void WhenNoModifiersArePresent_ModifiedValueMatchesBaseValue()
        {
            TController controller        = new TController();
            TModifiable     factorBeingTested = controller.ControlledInstance;
            int         numberOfTests     = 100;

            for (int i = 0; i < numberOfTests; i++)
            {
                var previousValue = factorBeingTested.BaseValue;
                var newValue      = controller.GetRandomInstanceOfValuesType_NotEqualTo(previousValue);

                factorBeingTested.BaseValue = newValue;
                Assert.That(factorBeingTested.Value, Is.EqualTo(newValue));
            }
        }
        
        [Test]
        public void WhenBaseValueIsChangedToANonEqualValue_SubscribersAreDestabilized()
        {
            TController controller               = new TController();
            TModifiable factorBeingTested        = controller.ControlledInstance;
            double      initializeValue          = factorBeingTested.Value; //- Values for non-necessary Reactors are lazily calculated.
            int         numberOfSubscribersToAdd = 10;
            var         subscribers              = AddSubscribersTo(factorBeingTested, numberOfSubscribersToAdd);
            
            foreach (var subscriber in subscribers)
            {
                Assert.That(subscriber.IsUnstable, Is.False);
            }

            controller.ChangeValueToANonEqualValue();
            
            foreach (var subscriber in subscribers)
            {
                Assert.That(subscriber.IsUnstable, Is.True);
            }
        }
        
        [Test]
        public void WhenChangingBaseValue_CausesModifiedValueToChangeToANonEqualValue_SubscribersAreTriggered()
        {
            TController controller               = new TController();
            TModifiable factorBeingTested        = controller.ControlledInstance;
            double      initializeValue          = factorBeingTested.Value;
            int         numberOfSubscribersToAdd = 10;
            var         subscribers              = AddSubscribersTo(factorBeingTested, numberOfSubscribersToAdd);
            
            foreach (var subscriber in subscribers)
            {
                Assert.That(subscriber.HasBeenTriggered, Is.False);
            }

            controller.ChangeValueToANonEqualValue();
            initializeValue = factorBeingTested.Value;
            
            foreach (var subscriber in subscribers)
            {
                Assert.That(subscriber.HasBeenTriggered, Is.True);
            }
        }
    }
}