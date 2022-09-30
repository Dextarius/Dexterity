using Core.Causality;
using Core.Factors;

namespace Factors.Cores.ObservedReactorCores
{
    public abstract class ObservedResponse : ObservedReactorCore, IProcess
    {
        #region Instance Methods

        protected override long CreateOutcome()
        {
            Observer.ObserveInteractions(this);
            RemoveUnusedTriggers();

            return TriggerFlags.Default;
        }

        protected abstract void ExecuteResponse();
        
        #endregion


        #region Explicit Implementations

        void IProcess.Execute() => ExecuteResponse();

        #endregion
    }
}