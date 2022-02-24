using System;
using Factors.Cores.DirectReactorCores;
using Factors.Cores.ProactiveCores;
using NUnit.Framework;
using static Tests.Tools.Tools;

namespace Tests.Class_Tests.Cores.DirectReactorCores
{
    public class DirectFunctionResults
    {
        [Test]
        public void WhenCreatedWithFunction_ValueMatchesTheOneReturnedByFunction()
        {
            var                 valueSource1  = new DirectStateCore<int>(GenerateRandomInt());
            var                 valueSource2  = new DirectStateCore<int>(GenerateRandomInt());
            Func<int, int, int> valueFunction = (value1, value2) => value1 + value2;
            var                 coreBeingTested = new DirectFunctionResult<int, int, int>(
                                                      valueFunction, valueSource1, valueSource2);
            int                 functionValue;

            functionValue = valueFunction.Invoke(valueSource1.Value, valueSource2.Value);
            Assert.That(coreBeingTested.Value, Is.EqualTo(functionValue));
        }
        
        [Test]
        public void IfGivenAFactorWithAValue_ReactUpdatesTheResultToMatchTheOneReturnedByFactor()
        {
            var                 valueSource1    = new DirectStateCore<int>(0);
            var                 valueSource2    = new DirectStateCore<int>(0);
            Func<int, int, int> valueFunction   = (value1, value2) => value1 + value2;
            var                 coreBeingTested = new DirectFunctionResult<int, int, int>(
                                                      valueFunction, valueSource1, valueSource2);
            int                 functionValue;

            for (int i = 0; i < 100; i++)
            {
                valueSource1.Value = GenerateRandomInt();
                valueSource2.Value = GenerateRandomInt();
                coreBeingTested.ForceReaction();
                functionValue = valueFunction.Invoke(valueSource1.Value, valueSource2.Value);
                
                Assert.That(coreBeingTested.Value, Is.EqualTo(functionValue));
            }
        }
    }
}