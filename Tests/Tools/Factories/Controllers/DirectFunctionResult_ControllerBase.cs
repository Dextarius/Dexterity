using Core.Factors;
using Tests.Tools.Interfaces;

namespace Tests.Tools.Factories.Controllers
{
    public abstract class DirectFunctionResult_ControllerBase<TReactive, TValue> : 
        FunctionBasedReactive_Controller<TReactive, TValue> 
        where TReactive : IReactive<TValue>
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
                inputController.ChangeValueToAnEqualValue();
            }
        }


        protected DirectFunctionResult_ControllerBase(IFactor_T_Controller<int>[] controllersForInputs)
        {
            inputControllers = controllersForInputs;
        }
    }
}