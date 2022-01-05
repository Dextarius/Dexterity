using Core.Causality;
using Core.States;

namespace Factors.Outcomes.ObservedOutcomes
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