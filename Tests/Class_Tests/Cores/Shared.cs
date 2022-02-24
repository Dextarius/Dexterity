using Core.Factors;
using NUnit.Framework;

namespace Tests.Class_Tests.Cores
{
    internal static class Shared
    {
        public static void AssertThatFactorHasValue<TFactor, TValue>(TFactor factorBeingTested, TValue expectedValue)
            where TFactor : IFactor<TValue>
        {
            TValue actualValue = factorBeingTested.Value;
            
            Assert.That(actualValue, Is.EqualTo(expectedValue));
            Tools.Tools.WriteExpectedAndActualValuesToTestContext(expectedValue, actualValue);
        }
    }
}
