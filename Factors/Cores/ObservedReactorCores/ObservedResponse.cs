using Core.Causality;

namespace Factors.Cores.ObservedReactorCores
{
    public abstract class ObservedResponse : ObservedReactorCore, IProcess
    {
        #region Instance Methods

        protected override bool GenerateOutcome()
        {
            Observer.ObserveInteractions(this);

            return true;
        }

        protected abstract void ExecuteResponse();

        
        #endregion

        
        #region Constructors

        protected ObservedResponse(string name) : base(name)
        {
        }

        #endregion

        void IProcess.Execute() => ExecuteResponse();
    }
}