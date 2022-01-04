using Factors;
using NUnit.Framework;

namespace Tests.ObservedObjects
{
    public class ObservedFactor_Tests
    {
        [Test]
        public void WhenValueRetrievedDuringAReaction_HasDependents()
        {
            Proactive<int> proactiveBeingTested = new Proactive<int>(42);
            Reactive<int>  dependentReactive    = new Reactive<int>(() => proactiveBeingTested);
            int            triggerValueUpdate   = dependentReactive.Value;

            Assert.That(proactiveBeingTested.HasDependents);
        }
    }
}