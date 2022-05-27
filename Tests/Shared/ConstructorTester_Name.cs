using System.Collections.Generic;
using Core.States;
using Factors.Cores.ProactiveCores;
using NUnit.Framework;
using Tests.Class_Tests.Cores.DirectProactorCores;
using Tests.Tools.Interfaces;

namespace Tests.Shared
{
    [TestFixture(typeof(DirectProactiveCore<int>), typeof(DirectStateCores))]
    public class ConstructorTester_Name<TTested, TTestClass>
        where TTestClass : ITestableConstructor_Name<TTested> , new() 
        where TTested    : INameable
    {
        public TTestClass testClass = new TTestClass();
        
        [Test]
        public void WhenGivenANameDuringConstruction_HasThatName()
        {
            string nameToTest    = "Some Name";
            var    objectsToTest = testClass.CallAllConstructors_AndPassName(nameToTest);

            foreach (var namedObject in objectsToTest)
            {
                string actualName = namedObject.Name;
                
                Assert.That(actualName, Is.EqualTo(nameToTest));
            }
        }
    }
}