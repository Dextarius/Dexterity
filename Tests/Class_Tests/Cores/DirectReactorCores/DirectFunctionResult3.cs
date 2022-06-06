using System;
using System.Collections.Generic;
using Core.Factors;
using Core.Tools;
using Factors;
using Factors.Cores.DirectReactorCores;
using Factors.Cores.ProactiveCores;
using JetBrains.Annotations;
using NUnit.Framework;
using static Tests.Tools.Tools;

namespace Tests.Class_Tests.Cores.DirectReactorCores
{
    public class DirectFunctionResults3
    {
        [Test]
        public void WhenCreatedWithFunction_ValueMatchesTheOneReturnedByFunction()
        {
            var                      valueSource1    = new Proactive<int>(GenerateRandomInt());
            var                      valueSource2    = new Proactive<int>(GenerateRandomInt());
            var                      valueSource3    = new Proactive<int>(GenerateRandomInt());
            Func<int, int, int, int> valueFunction   = (value1, value2, value3) => value1 + value2 + value3;
            var                      coreBeingTested = new DirectFunctionResult<int, int, int, int>(
                                                           valueFunction, valueSource1, valueSource2, valueSource3);
            var                      reactive        = new Reactive<int>(coreBeingTested);
            int                      functionValue;

            functionValue = valueFunction.Invoke(valueSource1.Value, valueSource2.Value, valueSource3.Value);
            Assert.That(reactive.Value, Is.EqualTo(functionValue));
        }
        
        [Test]
        public void IfGivenAFactorWithAValue_ReactUpdatesTheResultToMatchTheOneReturnedByFactor()
        {
            var                      valueSource1    = new Proactive<int>(0);
            var                      valueSource2    = new Proactive<int>(0);
            var                      valueSource3    = new Proactive<int>(0);
            Func<int, int, int, int> valueFunction   = (value1, value2, value3) => value1 + value2 + value3;
            var                      coreBeingTested = new DirectFunctionResult<int, int, int, int>(
                                                           valueFunction, valueSource1, valueSource2, valueSource3);
            var                      reactive        = new Reactive<int>(coreBeingTested);
            int                      functionValue;

            for (int i = 0; i < 100; i++)
            {
                valueSource1.Value = GenerateRandomInt();
                valueSource2.Value = GenerateRandomInt();
                valueSource3.Value = GenerateRandomInt();
                
                coreBeingTested.GenerateOutcome();
                functionValue = valueFunction.Invoke(valueSource1.Value, valueSource2.Value, valueSource3.Value);
                
                Assert.That(reactive.Value, Is.EqualTo(functionValue));
            }
        }
    }
    
}