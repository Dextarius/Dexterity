using System;
using Core.Factors;
using Factors.Observer;

namespace Factors.Cores.ProactiveCores
{
    public class ObservedProactorCore : ProactorCore, IInvolved
    {
        #region Static Properties

        protected static CausalObserver Observer => CausalObserver.ForThread;

        #endregion


        #region Instance Methods

        public void NotifyInvolved() => Observer.NotifyInvolved(Callback);
        public void NotifyChanged()  => Observer.NotifyChanged(Callback);

        #endregion
    }
}