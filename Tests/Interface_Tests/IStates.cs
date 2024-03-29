﻿using Core.States;
using Factors;
using Factors.Cores.ProactiveCores;
using NUnit.Framework;
using Tests.Tools;
using Tests.Tools.Factories;
using Tests.Tools.Interfaces;
using Tests.Tools.Mocks;
using static Dextarius.Utilities.Types;
using static Tests.Tools.Tools;

namespace Tests.Interface_Tests
{
    [TestFixture(typeof(Proactive<int>),   typeof(DirectStateCore_Int_Factory),   typeof(int))]
    [TestFixture(typeof(Proactive<int>), typeof(ObservedStateCore_Int_Factory), typeof(int))]
    public class IStates<TState, TStateFactory, TValue> 
        where TState         : IProactive<TValue>
        where TStateFactory  : IFactor_T_Factory<TState, TValue>, new()
    {
        #region Instance Fields

        private TStateFactory factory = new TStateFactory();

        #endregion
        
        
        #region Tests

        [Test]
        public void WhenGivenANewValue_NotEqualToTheCurrentValue_HasThatValue()
        {
            TState stateBeingTested = factory.CreateInstance();
            TValue initialValue     = stateBeingTested.Value;
            TValue updatedValue     = factory.CreateRandomValueNotEqualTo(initialValue);
            TValue actualValue;
            
            stateBeingTested.Value = updatedValue;
            actualValue = stateBeingTested.Value;
            
            Assert.That(actualValue, Is.EqualTo(updatedValue));
            WriteExpectedAndActualValuesToTestContext(updatedValue, actualValue);
        }

      // [Test]
        public void WhenGivenANewValue_UsesComparerToDetermineIfNewValueIsEqualToCurrentValue()
        {
            
        }

        #endregion
    }
}