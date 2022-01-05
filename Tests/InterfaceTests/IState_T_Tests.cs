using Core.States;
using Factors;
using Factors.Outcomes.Influences;
using NUnit.Framework;
using Tests.Tools;
using Tests.Tools.Factories;
using Tests.Tools.Interfaces;
using static Core.Tools.Types;
using static Tests.Tools.Tools;

namespace Tests.InterfaceTests
{
    [TestFixture(typeof(ObservedState<int>),     typeof(Factor_Int_Factory), typeof(int))]
    [TestFixture(typeof(Proactive<int>), typeof(Proactive_Int_Factory), typeof(int))]
    public class IState_T_Tests<TState, TStateFactory, TValue> 
                          where TState                 : IState<TValue>
                          where         TStateFactory  : IFactor_T_Factory<TState, TValue>, new()
    {
        #region Instance Fields

        private TStateFactory factory = new TStateFactory();

        #endregion

        
        #region Tests
        
        [Test]
        public void WhenGivenANewValue_NotEqualToTheCurrentValue_HasThatValue()
        {
            TValue initialValue     = factory.CreateRandomInstanceOfValuesType();
            TValue updatedValue     = factory.CreateRandomInstanceOfValuesType_NotEqualTo(initialValue);
            TState stateBeingTested = factory.CreateInstance_WithValue(initialValue);
            TValue actualValue      = stateBeingTested.Value;
            
            Assert.That(actualValue, Is.EqualTo(initialValue));
            WriteExpectedAndActualValuesToTestContext(initialValue, actualValue);

            stateBeingTested.Value = updatedValue;
            actualValue = stateBeingTested.Value;
            
            Assert.That(actualValue, Is.EqualTo(updatedValue));
            WriteExpectedAndActualValuesToTestContext(updatedValue, actualValue);
        }

        [Test]
        public void WhenGivenAValueEqualToCurrentValue_DependentsAreNotInvalidated()
        {
            int    numberOfDependents = 10;
            TValue initialValue       = factory.CreateRandomInstanceOfValuesType();
            TState stateBeingTested   = factory.CreateInstance_WithValue(initialValue);
            var    dependents         = AddDependentsTo(stateBeingTested, numberOfDependents);

            Assert.That(stateBeingTested.NumberOfSubscribers, Is.EqualTo(numberOfDependents));

            stateBeingTested.Value = initialValue;
            
            for (int i = 0; i < numberOfDependents; i++)
            {
                Assert.That(dependents[i].IsValid, Is.True,
                    $"Setting the value of a {NameOf<TState>()} invalidated its dependents even though " +
                    "the value set was equal to the old value. ");
            }
        }
        
        [Test]
        public void WhenValueChanges_DependentsAreInvalidated()
        {
            int            numberOfDependents = 10;
            int            initialValue       = GenerateRandomInt();
            int            updatedValue       = GenerateRandomIntNotEqualTo(initialValue);
            Proactive<int> stateBeingTested   = new Proactive<int>(initialValue);
            var            dependents         = AddDependentsTo(stateBeingTested, numberOfDependents);

            Assert.That(stateBeingTested.NumberOfSubscribers, Is.EqualTo(numberOfDependents));
            Assert.That(stateBeingTested.HasSubscribers, Is.True, 
                ErrorMessages.FactorDidNotHaveDependents<Proactive<int>>("despite being used to calculate a value. "));
            
            stateBeingTested.Value = updatedValue;
            
            for (int i = 0; i < numberOfDependents; i++)
            {
                Assert.That(dependents[i].IsValid, Is.False);
            }
        }
        
        #endregion
    }
}