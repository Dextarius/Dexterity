using System;
using Factors.Cores.ObservedReactorCores;
using NUnit.Framework;

namespace Tests.Class_Tests.Cores.ObservedReactorCores
{
    public class ObservedFunctionResults
    {
        [Test]
        public void WhenPassedAFunctionDuringConstruction_ValueMatchesFunctionOutput()
        {
            var       valueToTest    = 100;
            Func<int> functionToTest = () => valueToTest;
            var       objectsToTest  = new[]
            {
                new ObservedFunctionResult<int>(functionToTest),
                new ObservedFunctionResult<int>(functionToTest, "Name")
            };
            
            foreach (var objectBeingTested in objectsToTest)
            {
                int actualValue = objectBeingTested.Value;
                
                Assert.That(actualValue, Is.EqualTo(valueToTest));
            }
        }
    }
}