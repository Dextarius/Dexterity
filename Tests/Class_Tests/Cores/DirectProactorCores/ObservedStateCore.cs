using Factors;
using Factors.Cores.ProactiveCores;
using NUnit.Framework;
using static Tests.Class_Tests.Cores.Shared;
using static Tests.Tools.Tools;

namespace Tests.Class_Tests.Cores.DirectProactorCores
{
    public class ObservedStateCores 
    {
        [Test]
        public void WhenPassedAValueDuringConstruction_HasThatValue()
        {
            int testValue       = GenerateRandomInt();
            var coreBeingTested = new ObservedProactiveCore<int>(testValue);
            var proactive       = new Proactive<int>(coreBeingTested);

            coreBeingTested.SetOwner(proactive);
            AssertThatFactorHasValue(proactive, testValue);
        }
    }
}