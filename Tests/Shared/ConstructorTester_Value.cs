using Core.Factors;
using Core.Redirection;
using Factors.Cores.ProactiveCores;
using NUnit.Framework;
using Tests.Class_Tests.Cores.DirectProactorCores;
using Tests.Tools.Interfaces;

namespace Tests.Shared
{
    [TestFixture(typeof(DirectProactiveCore<int>), typeof(DirectStateCores), typeof(int))]
    public class ConstructorTester_Value<TTested, TTestClass, TValue>
        where TTested    : IValue<TValue> 
        where TTestClass : ITestableConstructor_Value<TTested, TValue> , new()
    {
        private TTestClass testClass = new TTestClass();

        [Test]
        public void WhenPassedAValueDuringConstruction_HasThatValue()
        {
            var valueToTest   = testClass.CreateRandomValue();
            var objectsToTest = testClass.CallAllConstructors_AndPassValue(valueToTest);
            
            foreach (var objectBeingTested in objectsToTest)
            {
                TValue actualValue = objectBeingTested.Value;
                
                Assert.That(actualValue, Is.EqualTo(valueToTest)); 
            }
        }
    }
}