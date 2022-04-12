using Core.Factors;
using Core.States;

namespace Factors
{
    public class DeadTrigger : IXXX
    {
        public bool Reconcile() => true;
        
        public bool Subscribe(IFactorSubscriber subscriberToAdd, bool isNecessary) => false;

        public void Unsubscribe(IFactorSubscriber subscriberToRemove)           { }
        public void NotifyNecessary(IFactorSubscriber necessarySubscriber)      { }
        public void NotifyNotNecessary(IFactorSubscriber unnecessarySubscriber) { }
    }
}