using System;
using Factors;
using Factors.Cores.DirectReactorCores;
using Factors.Cores.ProactiveCores;
using NUnit.Framework;
using static Tests.Tools.Tools;

namespace Tests.Class_Tests.Cores.DirectReactorCores
{
    public class DirectFunctionResult_Tests
    {
        [Test]
        public void WhenCreatedWithFunction_ValueMatchesTheOneReturnedByFunction()
        {
            var            valueSource     = new Proactive<int>(GenerateRandomInt());
            Func<int, int> valueFunction   = (value) => value;
            var            coreBeingTested = new DirectFunctionResult<int, int>(valueFunction, valueSource);
            var            reactive        = new Reactive<int>(coreBeingTested);
            int            functionValue;

            functionValue = valueFunction.Invoke(valueSource.Value);
            Assert.That(reactive.Value, Is.EqualTo(functionValue));
        }
        
        [Test]
        public void IfGivenAFactorWithAValue_ReactUpdatesTheResultToMatchTheOneReturnedByFactor()
        {
            var            valueSource     = new Proactive<int>(GenerateRandomInt());
            Func<int, int> valueFunction   = (value) => value;
            var            coreBeingTested = new DirectFunctionResult<int, int>(valueFunction, valueSource);
            var            reactive        = new Reactive<int>(coreBeingTested);
            int            functionValue;

            for (int i = 0; i < 100; i++)
            {
                valueSource.Value = i;
                reactive.ForceReaction();
                functionValue = valueFunction.Invoke(valueSource.Value);
                
                Assert.That(reactive.Value, Is.EqualTo(functionValue));
            }
        }
    }
}