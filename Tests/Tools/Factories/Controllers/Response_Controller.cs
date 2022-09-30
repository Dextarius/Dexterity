using Core.Factors;
using Tests.Tools.Interfaces;

namespace Tests.Tools.Factories.Controllers
{
    public abstract class Response_Controller<TCore> : ReactorCore_Controller<TCore>
        where TCore : IReactorCore
    {
        protected Response_Controller(TCore controlledInstance) : base(controlledInstance)
        {
        }
    }
}