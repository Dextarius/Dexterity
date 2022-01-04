using Factors;
using NUnit.Framework;
using Tests.Tools;
using Tests.Tools.Mocks;
using static Core.Tools.Types;
using static Tests.Tools.Tools;

namespace Tests
{
    public class Proactives
    {
        [Test]
        public void WhenGivenANameDuringConstruction_HasThatName([Random(1)] int value)
        {
            string         givenName            = "Some Proactive";
            Proactive<int> proactiveBeingTested = new Proactive<int>(value, givenName);
            string         actualName           = proactiveBeingTested.Name; 
            
            Assert.That(actualName, Is.EqualTo(givenName));
            TestContext.WriteLine($"Expected Value => {givenName},\nActual Value => {actualName}");
        }

        [Test]
        public void Constructor_WhenGivenNullName_UsesADefault([Random(1)] int value)
        {
            Proactive<int> testProactive = new Proactive<int>(value, null);

            Assert.NotNull(testProactive.Name);
        }
        
        [Test]
        public void WhenPassedAValueDuringConstruction_HasThatValue([Random(1)] int value)
        {
            Proactive<int> stateBeingTested = new Proactive<int>(value);
            int            actualValue      = stateBeingTested.Value;
            
            Assert.That(actualValue, Is.EqualTo(value));
            WriteExpectedAndActualValuesToTestContext(value, actualValue);
        }
        
        [Test]
        public void AfterConstruction_HasDependentsIsFalse([Random(1)] int value)
        {
            Proactive<int> proactiveToTest = new Proactive<int>(value);
            
            Assert.That(proactiveToTest.HasDependents, Is.False, 
                $"The property {nameof(proactiveToTest.HasDependents)} was marked as true during construction.");
        }

        [Test]
        public void WhenValueChanges_DependentsAreRemoved()
        {
            int            numberOfDependents = 10;
            int            initialValue       = GenerateRandomInt();
            int            updatedValue       = GenerateRandomIntNotEqualTo(initialValue);
            Proactive<int> stateBeingTested   = new Proactive<int>(initialValue);
            var            interactions       = new MockDependent[numberOfDependents];

            for (int i = 0; i < numberOfDependents; i++)
            {
                var createdInteraction = new MockDependent();
                
                interactions[i] = createdInteraction;
                stateBeingTested.AddDependent(createdInteraction);
            }

            Assert.That(stateBeingTested.NumberOfDependents, Is.EqualTo(numberOfDependents));
            stateBeingTested.Value = updatedValue;

            Assert.That(stateBeingTested.NumberOfDependents, Is.Zero);
            Assert.That(stateBeingTested.HasDependents,      Is.False, 
                ErrorMessages.HasDependents<Proactive<int>>("despite its value changing. "));
        }
        
        [Test]
        public void WhenGivenANewValue_NotEqualToTheCurrentValue_HasThatValue()
        {
            int            initialValue     = GenerateRandomInt();
            int            updatedValue     = GenerateRandomIntNotEqualTo(initialValue);
            Proactive<int> stateBeingTested = new Proactive<int>(initialValue);
            int            actualValue      = stateBeingTested.Value;
            
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
            int            numberOfDependents = 10;
            int            initialValue       = GenerateRandomInt();
            int            updatedValue       = GenerateRandomIntNotEqualTo(initialValue);
            Proactive<int> stateBeingTested   = new Proactive<int>(initialValue);
            var            dependents         = new MockDependent[numberOfDependents];

            for (int i = 0; i < numberOfDependents; i++)
            {
                var dependent = new MockDependent();
                
                dependents[i] = dependent;
                stateBeingTested.AddDependent(dependent);
                Assert.That(dependent.IsValid, Is.True);
            }

            Assert.That(stateBeingTested.NumberOfDependents, Is.EqualTo(numberOfDependents));
            Assert.That(stateBeingTested.HasDependents, Is.True, 
                ErrorMessages.FactorDidNotHaveDependents<Proactive<int>>("despite being used to calculate a value. "));
            
            stateBeingTested.Value = updatedValue;
            
            for (int i = 0; i < numberOfDependents; i++)
            {
                Assert.That(dependents[i].IsValid, Is.False);
            }
        }

        [Test]
        public void WhenGivenAValueEqualToCurrentValue_DependentsAreNotInvalidated()
        {
            int              numberOfDependents   = 10;
            int              initialValue         = 5;
            Proactive<int>   proactiveBeingTested = new Proactive<int>(initialValue);
            Reactive<int>[]  dependents           = new Reactive<int>[numberOfDependents];

            for (int i = 0; i < numberOfDependents; i++)
            {
                Reactive<int> dependentReactive = CreateReactiveThatGetsValueOf(proactiveBeingTested);
                int           triggerAReaction  = dependentReactive.Value;
                
                Assert.That(dependentReactive.IsValid, Is.True, ErrorMessages.ReactorWasNotValid<Reactive<int>>());
                dependents[i] = dependentReactive;
            }
            
            Assert.That(proactiveBeingTested.NumberOfDependents, Is.EqualTo(numberOfDependents),
                ErrorMessages.FactorDidNotHaveDependents<Proactive<int>>("despite being used to calculate a value. "));

            proactiveBeingTested.Value = initialValue;

            for (int i = 0; i < numberOfDependents; i++)
            {

                Assert.That(dependents[i].IsValid,
                    $"Setting the value of a {NameOf<Proactive<int>>()} invalidated its dependents even though " +
                    "the value set was equal to the old value. ");
            }
            

            Assert.That(proactiveBeingTested.HasDependents,
                ErrorMessages.FactorDidNotHaveDependents<Proactive<int>>("despite being used to calculate a value. "));
        }
        
        [Test]
        public void WhenGivenAValueEqualToCurrentValue_DependentsAreNotRemoved()
        {
            int    numberOfDependents = 10;
            int initialValue       = GenerateRandomInt();
            Proactive<int> stateBeingTested   = new Proactive<int>(initialValue);
            var    interactions       = new MockDependent[numberOfDependents];

            for (int i = 0; i < numberOfDependents; i++)
            {
                var createdInteraction = new MockDependent();
                
                interactions[i] = createdInteraction;
                stateBeingTested.AddDependent(createdInteraction);
            }

            Assert.That(stateBeingTested.NumberOfDependents, Is.EqualTo(numberOfDependents));
            Assert.That(stateBeingTested.HasDependents, Is.True, 
                ErrorMessages.FactorDidNotHaveDependents<Proactive<int>>("despite being used to calculate a value. "));
            
            stateBeingTested.Value = initialValue;
            
            Assert.That(stateBeingTested.HasDependents, Is.True, 
                ErrorMessages.FactorDidNotHaveDependents<Proactive<int>>("despite its value not changing. "));
            
            Assert.That(stateBeingTested.NumberOfDependents, Is.EqualTo(numberOfDependents),
                $"Setting the value of a {NameOf<Proactive<int>>()} removed one or more of its dependents even though " +
                $"the value set was equal to the old value. # Remaining Dependents => {stateBeingTested.NumberOfDependents} ");
        }
    }
}