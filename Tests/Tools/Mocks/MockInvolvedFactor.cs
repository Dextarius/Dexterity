using Core.Factors;
using Factors;
using Factors.Observer;

namespace Tests.Tools.Mocks
{
    internal class MockInvolvedFactor : MockFactor, IInvolved
    {
        protected readonly CausalObserver observer;

        public void NotifyInvolved(long triggerFlags) => observer.NotifyInvolved(this, TriggerFlags.Default);
        public void NotifyInvolved()                  => NotifyInvolved(TriggerFlags.Default);
        
        public MockInvolvedFactor(CausalObserver observerToCall)
        {
            observer = observerToCall;
        }
    }
}