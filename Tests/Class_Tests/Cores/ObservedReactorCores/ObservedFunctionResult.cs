using System;
using Factors;
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
               new Reactive<int>(new ObservedFunctionResult<int>(functionToTest)),
               new Reactive<int>(new ObservedFunctionResult<int>(functionToTest))
            };
            
            foreach (var objectBeingTested in objectsToTest)
            {
                int actualValue = objectBeingTested.Value;
                
                Assert.That(actualValue, Is.EqualTo(valueToTest));
            }
        }
    }
}