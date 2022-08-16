using System;
using Factors;
using Factors.Cores.DirectReactorCores;
using Factors.Cores.ProactiveCores;
using NUnit.Framework;
using static Tests.Tools.Tools;

namespace Tests.Class_Tests.Cores.DirectReactorCores
{
    //- TODO : We should be able to combine these DirectFunctionResults classes now that cores are 
    //         only expected to work after being put in a reactor.
    public class DirectFunctionResults
    {
        [Test]
        public void WhenCreatedWithFunction_ValueMatchesTheOneReturnedByFunction()
        {
            var                 valueSource1    = new Proactive<int>(GenerateRandomInt());
            var                 valueSource2    = new Proactive<int>(GenerateRandomInt());
            Func<int, int, int> valueFunction   = (value1, value2) => value1 + value2;
            var                 coreBeingTested = new DirectFunctionResult<int, int, int>(
                                                      valueFunction, valueSource1, valueSource2);
            var                 reactive        = new Reactive<int>(coreBeingTested);
            int                 functionValue;

            functionValue = valueFunction.Invoke(valueSource1.Value, valueSource2.Value);
            Assert.That(reactive.Value, Is.EqualTo(functionValue));
        }
        
        [Test]
        public void IfGivenAFactorWithAValue_ReactUpdatesTheResultToMatchTheOneReturnedByFactor()
        {
            var                 valueSource1  = new Proactive<int>(0);
            var                 valueSource2  = new Proactive<int>(0);
            Func<int, int, int> valueFunction = (value1, value2) => value1 + value2;
            var                 coreBeingTested = new DirectFunctionResult<int, int, int>(
                                                      valueFunction, valueSource1, valueSource2);
            var                 reactive        = new Reactive<int>(coreBeingTested);
            int                 functionValue;

            for (int i = 0; i < 100; i++)
            {
                valueSource1.Value = GenerateRandomInt();
                valueSource2.Value = GenerateRandomInt();
                coreBeingTested.AttemptReaction();
                functionValue = valueFunction.Invoke(valueSource1.Value, valueSource2.Value);
                
                Assert.That(reactive.Value, Is.EqualTo(functionValue));
            }
        }
    }
}