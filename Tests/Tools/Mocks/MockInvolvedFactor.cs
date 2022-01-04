using Core.Factors;
using Factors.Observer;

namespace Tests.Tools.Mocks
{
    internal class MockInvolvedFactor : MockFactor, IInvolved
    {
        protected readonly CausalObserver observer;

        public void NotifyInvolved() => observer.NotifyInvolved(this);
        
        public MockInvolvedFactor(CausalObserver observerToCall)
        {
            observer = observerToCall;
        }
    }
}