using System;
using Core.Factors;
using Factors.Observer;

namespace Factors.Cores.ProactiveCores
{
    public class ObservedProactorCore : ProactorCore, IInvolved
    {
        #region Instance Methods

        public void NotifyInvolved(long triggerFlags) => Observer.NotifyInvolved(Callback, triggerFlags);
        public void NotifyInvolved()                  => NotifyInvolved(TriggerFlags.Default);
        public void NotifyChanged()                   => Observer.NotifyChanged(Callback);

        #endregion
    }
}