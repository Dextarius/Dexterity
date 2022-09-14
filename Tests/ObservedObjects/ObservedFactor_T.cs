using Core.Factors;
using Factors;
using Factors.Observer;
using NUnit.Framework;
using Tests.Tools.Interfaces;
using Tests.Tools.Mocks;
using static Tests.Tools.Tools;

namespace Tests.ObservedObjects
{
    public class ObservedFactor_T<TFactor, TFactory, TValue>  
        where TFactor  : IFactor<TValue>, IInvolved
        where TFactory : IFactory<TFactor>, new()
    {
        #region Instance Fields

        private TFactory factory = new TFactory();

        #endregion

        
        #region Tests
        
        public void WhenValueRetrieved_NotifiesObserverItsInvolved()
        {
            TFactor stateBeingTested = factory.CreateInstance();
            var     observedObject   = new MockObserved();
            var     process          = CreateProcessThatRetrievesValueOf(stateBeingTested);

            CausalObserver.ForThread.ObserveInteractions(process, observedObject);
            Assert.That(observedObject.WasInfluenced, Is.True);
        }
        
        public void WhenValueRetrievedUsingPeek_DoesNotNotifyObserverItsInvolved()
        {
            TFactor stateBeingTested = factory.CreateInstance();
            var     observedObject   = new MockObserved();
            var     process          = CreateProcessThatPeeksAtTheValueOf(stateBeingTested);
            
            CausalObserver.ForThread.ObserveInteractions(process, observedObject);
            Assert.That(observedObject.WasInfluenced, Is.False);
        }

        #endregion
    }
}