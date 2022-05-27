using Core.Causality;
using Core.Factors;

namespace Factors.Cores.ObservedReactorCores
{
    public abstract class ObservedResponse : ObservedReactorCore, IProcess
    {
        #region Instance Methods

        protected override bool CreateOutcome()
        {
            Observer.ObserveInteractions(this);
            RemoveUnusedTriggers();

            return true;
        }

        protected abstract void ExecuteResponse();

        
        #endregion

        
        #region Constructors

        protected ObservedResponse() : base()
        {
        }

        #endregion

        void IProcess.Execute() => ExecuteResponse();
    }
}