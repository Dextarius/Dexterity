using Causality.States;
using Core.States;
using Factors;
using NUnit.Framework;
using Tests.Causality.Factories;
using Tests.Causality.Interfaces;
using Tests.Causality.Mocks;
using static Core.Tools.Types;
using static Tests.Tools;

namespace Tests.Causality
{
    [TestFixture(typeof(State<int>),     typeof(State_Int_Factory), typeof(int))]
    [TestFixture(typeof(Proactive<int>), typeof(Proactive_Int_Factory), typeof(int))]
    
    public class IMutableState_T_Tests<TState, TStateFactory, TValue> 
                                 where TState                 : IMutableState<TValue>
                                 where         TStateFactory  : IState_T_Factory<TState, TValue>, new()
    {
        #region Instance Fields

        private TStateFactory factory = new TStateFactory();

        #endregion

        
        #region Tests

        [Test]
        public void WhenPassedAValueDuringConstruction_HasThatValue()
        {
            TValue valueToTest      = factory.CreateRandomInstanceOfValuesType();
            TState stateBeingTested = factory.CreateInstance_WithValue(valueToTest);
            TValue actualValue      = stateBeingTested.Value;
            
            Assert.That(actualValue, Is.EqualTo(valueToTest));
            WriteExpectedAndActualValuesToTestContext(valueToTest, actualValue);
        }

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
        public void WhenValueChanges_DependentsAreInvalidated()
        {
            int    numberOfDependents = 10;
            TValue initialValue       = factory.CreateRandomInstanceOfValuesType();
            TValue updatedValue       = factory.CreateRandomInstanceOfValuesType_NotEqualTo(initialValue);
            TState stateBeingTested   = factory.CreateInstance_WithValue(initialValue);
            var    interactions       = new MockInteraction[numberOfDependents];

            for (int i = 0; i < numberOfDependents; i++)
            {
                var createdInteraction = new MockInteraction();
                
                interactions[i] = createdInteraction;
                createdInteraction.MakeValid();
                stateBeingTested.AddDependent(createdInteraction);
                Assert.That(createdInteraction.IsValid, Is.True);
            }

            Assert.That(stateBeingTested.NumberOfDependents, Is.EqualTo(numberOfDependents));
            Assert.That(stateBeingTested.HasDependents, Is.True, 
                ErrorMessages.FactorDidNotHaveDependents<TState>("despite being used to calculate a value. "));
            
            stateBeingTested.Value = updatedValue;
            
            for (int i = 0; i < numberOfDependents; i++)
            {
                Assert.That(interactions[i].IsValid, Is.False);
            }
        }

        [Test]
        public void WhenGivenAValueEqualToCurrentValue_DependentsAreNotInvalidated()
        {
            int    numberOfDependents = 10;
            TValue initialValue       = factory.CreateRandomInstanceOfValuesType();
            TState stateBeingTested   = factory.CreateInstance_WithValue(initialValue);
            var    interactions       = new MockInteraction[numberOfDependents];

            for (int i = 0; i < numberOfDependents; i++)
            {
                var createdInteraction = new MockInteraction();
                
                interactions[i] = createdInteraction;
                createdInteraction.MakeValid();
                stateBeingTested.AddDependent(createdInteraction);
                Assert.That(createdInteraction.IsValid, Is.True);
            }

            Assert.That(stateBeingTested.NumberOfDependents, Is.EqualTo(numberOfDependents));
            Assert.That(stateBeingTested.HasDependents, Is.True, 
                ErrorMessages.FactorDidNotHaveDependents<TState>("despite being used to calculate a value. "));
            
            stateBeingTested.Value = initialValue;
            
            for (int i = 0; i < numberOfDependents; i++)
            {
                Assert.That(interactions[i].IsValid, Is.True,
                    $"Setting the value of a {NameOf<TState>()} invalidated its dependents even though " +
                    "the value set was equal to the old value. ");
            }
        }
        
        [Test]
        public void WhenValueChanges_DependentsAreRemoved()
        {
            int    numberOfDependents = 10;
            TValue initialValue       = factory.CreateRandomInstanceOfValuesType();
            TValue updatedValue       = factory.CreateRandomInstanceOfValuesType_NotEqualTo(initialValue);
            TState stateBeingTested   = factory.CreateInstance_WithValue(initialValue);
            var    interactions       = new IInteraction[numberOfDependents];

            for (int i = 0; i < numberOfDependents; i++)
            {
                var createdInteraction = new MockInteraction();
                
                interactions[i] = createdInteraction;
                stateBeingTested.AddDependent(createdInteraction);
            }

            Assert.That(stateBeingTested.NumberOfDependents, Is.EqualTo(numberOfDependents));
            Assert.That(stateBeingTested.HasDependents, Is.True, 
                ErrorMessages.FactorDidNotHaveDependents<TState>("despite being used to calculate a value. "));
            
            stateBeingTested.Value = updatedValue;
            
            Assert.That(stateBeingTested.HasDependents, Is.False, 
                ErrorMessages.HasDependents<TState>("despite its value changing. "));
            Assert.That(stateBeingTested.NumberOfDependents, Is.Zero);
        }

        [Test]
        public void WhenGivenAValueEqualToCurrentValue_DependentsAreNotRemoved()
        {
            int    numberOfDependents = 10;
            TValue initialValue       = factory.CreateRandomInstanceOfValuesType();
            TState stateBeingTested   = factory.CreateInstance_WithValue(initialValue);
            var    interactions       = new IInteraction[numberOfDependents];

            for (int i = 0; i < numberOfDependents; i++)
            {
                var createdInteraction = new MockInteraction();
                
                interactions[i] = createdInteraction;
                stateBeingTested.AddDependent(createdInteraction);
            }

            Assert.That(stateBeingTested.NumberOfDependents, Is.EqualTo(numberOfDependents));
            Assert.That(stateBeingTested.HasDependents, Is.True, 
                ErrorMessages.FactorDidNotHaveDependents<TState>("despite being used to calculate a value. "));
            
            stateBeingTested.Value = initialValue;
            
            Assert.That(stateBeingTested.HasDependents, Is.True, 
                ErrorMessages.FactorDidNotHaveDependents<TState>("despite its value not changing. "));
            
            Assert.That(stateBeingTested.NumberOfDependents, Is.EqualTo(numberOfDependents),
                $"Setting the value of a {NameOf<TState>()} removed one or more of its dependents even though " +
                $"the value set was equal to the old value. # Remaining Dependents => {stateBeingTested.NumberOfDependents} ");
        }
        

        #endregion
    }
}