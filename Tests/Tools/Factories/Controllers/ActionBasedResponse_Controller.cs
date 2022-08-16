using Core.Factors;

namespace Tests.Tools.Factories.Controllers
{
    public abstract class ActionBasedResponse_Controller<TCore> : Response_Controller<TCore>
        where TCore : IReactorCore
    {
        protected abstract void ChangeInputsToANonEqualValue();
        protected abstract void ChangeInputsToAnEqualValue();
        
        protected ActionBasedResponse_Controller(TCore controlledInstance) : base(controlledInstance)
        {
        }
    }
}