using System;
using Factors;
using NUnit.Framework;
using static Tests.Tools.Tools;

namespace Tests.ObservedObjects
{
    public class ObservedReactive
    {
        //- These need to be rewritten to use proactive cores.
        
        // [Test]
        // public void WhenGettingValueFromAProactive_HasTheCorrectValue([Random(1)] int value)
        // {
        //     Proactive<int> proactiveBeingTested = new Proactive<int>(value);
        //     Reactive<int>  reactiveBeingTested  = CreateReactiveThatGetsValueOf(proactiveBeingTested);
        //     int            actualValue          = reactiveBeingTested.Value; 
        //     
        //     Assert.That(actualValue, Is.EqualTo(value));
        //     TestContext.WriteLine($"Expected Value {value}, Actual Value {actualValue}");
        // }
        //
        // [Test]
        // public void WhenGettingValueFromMultipleProactives_HasTheCorrectValue()
        // {
        //     int            firstValue          = GenerateRandomInt();
        //     int            secondValue         = GenerateRandomIntNotEqualTo(firstValue);
        //     Proactive<int> firstProactive      = new Proactive<int>(firstValue);
        //     Proactive<int> secondProactive     = new Proactive<int>(secondValue);
        //     Func<int>      sumValues           = () => firstProactive.Value + secondProactive.Value;
        //     Reactive<int>  reactiveBeingTested = new Reactive<int>(sumValues);
        //     int            expectedValue       = firstValue + secondValue; 
        //     int            actualValue         = reactiveBeingTested.Value; 
        //     
        //     Assert.That(actualValue, Is.EqualTo(expectedValue));
        //     TestContext.WriteLine($"Expected Value {expectedValue}, Actual Value {actualValue}");
        // }
    }
}