using Core.Factors;
using Factors;
using Factors.Cores.ObservedReactorCores;
using Factors.Observer;
using NUnit.Framework;
using Tests.Tools.Interfaces;
using Tests.Tools.Mocks;
using static Tests.Tools.Tools;

namespace Tests.ObservedObjects
{
    public class ObservedFactorTests<TFactor, TFactory, TValue>  
        where TFactor  : IFactor, IInvolved
        where TFactory : IFactory<TFactor>, new()
    {
        #region Instance Fields

        private TFactory factory = new TFactory();

        #endregion
        
        [Test]
        public void WhenNotifyInvolvedIsCalled_NotifiesObserver()
        {
            TFactor stateBeingTested = factory.CreateInstance();
            var     observedObject   = new MockObserved();
            var     process          = CreateProcessThatCallsNotifyInvolvedOn(stateBeingTested);

            CausalObserver.ForThread.ObserveInteractions(process, observedObject);
            Assert.That(observedObject.WasInfluenced, Is.True);
        }
    }
}