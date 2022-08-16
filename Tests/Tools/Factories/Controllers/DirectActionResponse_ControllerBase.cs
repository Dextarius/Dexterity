using Core.Factors;
using Tests.Tools.Interfaces;

namespace Tests.Tools.Factories.Controllers
{
    public abstract class DirectActionResponse_ControllerBase<TCore> : ActionBasedResponse_Controller<TCore>
        where TCore : IReactorCore
    {
        protected readonly IFactor_T_Controller<int>[] inputControllers;

        
        protected override void ChangeInputsToANonEqualValue()
        {
            foreach (var inputController in inputControllers)
            {
                inputController.ChangeValueToANonEqualValue();
            }
        }
        
        protected override void ChangeInputsToAnEqualValue()
        {
            foreach (var inputController in inputControllers)
            {
                inputController.SetValueToAnEqualValue();
            }
        }
        
        public override void SetOffInstancesTriggers() => ChangeInputsToANonEqualValue();

        protected DirectActionResponse_ControllerBase(IFactor_T_Controller<int>[] controllersForInputs, TCore core) : 
            base(core)
        {
            inputControllers = controllersForInputs;
        }
    }
}