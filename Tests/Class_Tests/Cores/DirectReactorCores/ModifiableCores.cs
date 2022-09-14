using System;
using Core.Factors;
using NUnit.Framework;
using Tests.Tools.Interfaces;
using static Tests.Tools.Tools;

namespace Tests.Class_Tests.Cores.DirectReactorCores
{
    public class ModifiableCores<TFactor, TController, TValue>
        where TFactor     : ValueController<TValue> //- TODO : Try to separate the IDeterminant related parts
        where TController : IFactor_T_Controller<TFactor, TValue>, new()
    {
        //- WhenModifierRemoved_IsTriggered()
        //- WhenBaseValueChanges_IsTriggered()

        //- WhenModifierAdded_ValueChangesAppropriately()
        //- WhenModifierRemoved_ValueChangesAppropriately()
        
        //- WhenMultipleModifiersAreAdded_ModifiersAreAppliedInOrderOfPriority()
        //- WhenMultipleModifiersArePresent_AndOneIsRemoved_RemainingModifiersAreAppliedInOrderOfPriority()
        
        //- When_MadeNecessary_AChangeInAModifierTriggersValueToUpdate()
        //- When_MadeNotNecessary_ModifiersBecomeNotNecessary()
        //
        // [Test]
        // public void WhenModifiedValueIsRetrieved_WhenFactorIsInATriggeredState_Reacts()
        // {
        //     TController controller        = new TController();
        //     TFactor     factorBeingTested = controller.ControlledInstance;
        //
        //     if (factorBeingTested.IsTriggered is false)
        //     {
        //         factorBeingTested.Trigger();
        //     }
        //     
        //     Assert.That(factorBeingTested.IsTriggered, Is.True);
        //     _ = factorBeingTested.Value;
        //     Assert.That(factorBeingTested.IsTriggered, Is.False);
        //     Assert.That(factorBeingTested.IsUnstable,       Is.False);
        // }
        //
        // [Test]
        // public void WhenModifierAdded_IsTriggered()
        // {
        //     TController controller        = new TController();
        //     TFactor     factorBeingTested = controller.ControlledInstance;
        //
        //     if (factorBeingTested.IsTriggered)
        //     {
        //         _ = factorBeingTested.Value;
        //     }
        //     
        //     Assert.That(factorBeingTested.IsTriggered, Is.False);
        //     _ = factorBeingTested.Value;
        //     Assert.That(factorBeingTested.IsTriggered, Is.False);
        //     Assert.That(factorBeingTested.IsUnstable,       Is.False);
        // }
        //
        //
        // [Test]
        // public void WhenBaseValueIsSet_BaseValueReturnsSetValue()
        // {
        //     TController controller        = new TController();
        //     TFactor     factorBeingTested = controller.ControlledInstance;
        //     int         numberOfTests     = 100;
        //
        //     for (int i = 0; i < numberOfTests; i++)
        //     {
        //         var previousValue = factorBeingTested.BaseValue;
        //         var newValue      = controller.GetRandomInstanceOfValuesType_NotEqualTo(previousValue);
        //
        //         factorBeingTested.BaseValue = newValue;
        //         Assert.That(factorBeingTested.BaseValue, Is.EqualTo(newValue));
        //     }
        // }
        //
        // [Test]
        // public void WhenNoModifiersArePresent_ModifiedValueMatchesBaseValue()
        // {
        //     TController controller        = new TController();
        //     TFactor     factorBeingTested = controller.ControlledInstance;
        //     int         numberOfTests     = 100;
        //
        //     for (int i = 0; i < numberOfTests; i++)
        //     {
        //         var previousValue = factorBeingTested.BaseValue;
        //         var newValue      = controller.GetRandomInstanceOfValuesType_NotEqualTo(previousValue);
        //
        //         factorBeingTested.BaseValue = newValue;
        //         Assert.That(factorBeingTested.Value, Is.EqualTo(newValue));
        //     }
        // }
        //
        // [Test]
        // public void WhenBaseValueIsChangedToANonEqualValue_SubscribersAreDestabilized()
        // {
        //     TController controller               = new TController();
        //     TFactor     factorBeingTested        = controller.ControlledInstance;
        //     double      initializeValue          = factorBeingTested.Value; //- Values for non-necessary Reactors are lazily calculated.
        //     int         numberOfSubscribersToAdd = 10;
        //     var         subscribers              = AddSubscribersTo(factorBeingTested, numberOfSubscribersToAdd);
        //     
        //     foreach (var subscriber in subscribers)
        //     {
        //         Assert.That(subscriber.IsUnstable, Is.False);
        //     }
        //
        //     controller.ChangeValueToANonEqualValue();
        //     
        //     foreach (var subscriber in subscribers)
        //     {
        //         Assert.That(subscriber.IsUnstable, Is.True);
        //     }
        // }
        //
        // [Test]
        // public void WhenChangingBaseValue_CausesModifiedValueToChangeToANonEqualValue_SubscribersAreTriggered()
        // {
        //     TController controller               = new TController();
        //     TFactor     factorBeingTested        = controller.ControlledInstance;
        //     double      initializeValue          = factorBeingTested.Value;
        //     int         numberOfSubscribersToAdd = 10;
        //     var         subscribers              = AddSubscribersTo(factorBeingTested, numberOfSubscribersToAdd);
        //     
        //     foreach (var subscriber in subscribers)
        //     {
        //         Assert.That(subscriber.IsTriggered, Is.False);
        //     }
        //
        //     controller.ChangeValueToANonEqualValue();
        //     initializeValue = factorBeingTested.Value;
        //     
        //     foreach (var subscriber in subscribers)
        //     {
        //         Assert.That(subscriber.IsTriggered, Is.True);
        //     }
        // }
    }
}