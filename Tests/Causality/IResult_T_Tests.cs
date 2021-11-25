using Causality.States;
using Core.Causality;
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
    //- TODO : We're trying to get these arguments right, so these tests will run
    
    [TestFixture(typeof(Result<int>  ), typeof(Result_Int_Factory  ), typeof(CausalFactor), typeof(CausalFactor_Factory), typeof(int))]
    [TestFixture(typeof(Reactive<int>), typeof(Reactive_Int_Factory), typeof(CausalFactor), typeof(CausalFactor_Factory), typeof(int))]
   
    public class IResult_T_Tests<TResult, TDependent, TOutcomeFactory, TDependentFactory, TValue>  
        where TResult           : IResult<TValue> 
        where TOutcomeFactory   : IResultFactory<TResult, TValue>, new()
        where TDependent        : IInteraction, IResult
        where TDependentFactory : IInteractionFactory<TDependent>, new()
    {

        private TOutcomeFactory   resultFactory    = new TOutcomeFactory();
        private TDependentFactory dependentFactory = new TDependentFactory();

        [Test]
        public void IfGivenAProcessThatReturnsAValue_ReactUpdatesTheResultToMatchTheOneReturnedByProcess()
        {
            var     storedValue  = new StoredValueProcess<TValue>(default);
            TResult resultToTest = resultFactory.CreateInstance_WhoseUpdateCalls(storedValue);
            TValue  returnedValue;

            for (int i = 0; i < 100; i++)
            {
                storedValue.Value = resultFactory.CreateRandomInstanceOfValuesType_NotEqualTo(storedValue.Value);
                resultToTest.React();
                returnedValue = resultToTest.Value;
                
                Assert.That(returnedValue, Is.EqualTo(storedValue.Value));
            }
        }

        [Test]
        public void IfRecalculatingReturnsADifferentValueThanTheCurrentValue_DependentsAreInvalidated()
        {
            TValue       initialValue       = resultFactory.CreateRandomInstanceOfValuesType();
            TValue       updatedValue       = resultFactory.CreateRandomInstanceOfValuesType_NotEqualTo(initialValue);
            var          valueProcess       = new StoredValueProcess<TValue>(initialValue);
            TResult      outcomeBeingTested = resultFactory.CreateInstance_WhoseUpdateCalls(valueProcess);
            int          numberOfDependents = 10;
            TDependent[] dependents         = new TDependent[numberOfDependents];

            Assert.That(outcomeBeingTested.HasDependents, Is.False,
                $"The {NameOf<TResult>()} was marked as consequential before being used. ");
            
            AssumeHasNoDependents(outcomeBeingTested);

            for (int i = 0; i < numberOfDependents; i++)
            {
                var createdDependent = dependentFactory.CreateInstance();

                dependents[i] = createdDependent;
                outcomeBeingTested.AddDependent(createdDependent);
                Assert.That(createdDependent.IsValid, Is.True);
            }

            AssumeHasSpecificNumberOfDependents(outcomeBeingTested, numberOfDependents);

            valueProcess.Value = updatedValue;
            outcomeBeingTested.React();
            
            for (int i = 0; i < numberOfDependents; i++)
            {
                var createdDependent = dependentFactory.CreateInstance();

                dependents[i] = createdDependent;
                outcomeBeingTested.AddDependent(createdDependent);
                Assert.That(createdDependent.IsValid, Is.False);
            }
        }
        

        [Test]
        public void IfRecalculatingReturnsAValueEqualToCurrentValue_DependentsAreNotInvalidated()
        {
            TValue       initialValue       = resultFactory.CreateRandomInstanceOfValuesType();
            var          valueProcess       = new StoredValueProcess<TValue>(initialValue);
            TResult      outcomeBeingTested = resultFactory.CreateInstance_WhoseUpdateCalls(valueProcess);
            int          numberOfDependents = 10;
            TDependent[] dependents         = new TDependent[numberOfDependents];

            Assert.That(outcomeBeingTested.HasDependents, Is.False,
                $"The {NameOf<TResult>()} was marked as consequential before being used. ");
            
            AssumeHasNoDependents(outcomeBeingTested);

            for (int i = 0; i < numberOfDependents; i++)
            {
                var createdDependent = dependentFactory.CreateInstance();

                dependents[i] = createdDependent;
                outcomeBeingTested.AddDependent(createdDependent);
                Assert.That(createdDependent.IsValid, Is.True);
            }

            AssumeHasSpecificNumberOfDependents(outcomeBeingTested, numberOfDependents);

            outcomeBeingTested.React();
            
            for (int i = 0; i < numberOfDependents; i++)
            {
                var createdDependent = dependentFactory.CreateInstance();

                dependents[i] = createdDependent;
                outcomeBeingTested.AddDependent(createdDependent);
                Assert.That(createdDependent.IsValid, Is.True, 
                   ErrorMessages.ValueFactorInvalidatedDependentsWhenGivenAnEquivalentValue<TResult>());
            }
        }
    }
    
}