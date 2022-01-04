using Core.Factors;
using Factors;
using NUnit.Framework;

namespace Tests.ObservedObjects
{
    public class ObservedOutcome_Tests
    {
        [Test]
        public void WhenInvolved_NotifiesObserver()
        {
            var outcomeBeingTested = new ObservedResult<int>()

            IFactor factorBeingTested = factory.CreateInstance();
            var     process           = CreateProcessThatCallsNotifyInvolvedOn(factorBeingTested);
            var     observedObject    = new MockInteraction(process);

            Factor.Observe(process, observedObject);
            
            Assert.That(observedObject.WasInfluenced, Is.True);
        }
    }
}