using Core.Factors;
using Core.States;
using Tests.Tools.Interfaces;

namespace Tests.Tools.Factories.Controllers
{
    public abstract class DirectFunctionResult_ControllerBase<TCore, TValue> : 
        FunctionBasedReactive_Controller<TCore, TValue> 
        where TCore : IResult<TValue>
    {
        protected readonly IFactor_T_Controller<int>[] inputControllers;
        //^ The controllers for each of the Factors used in the Reactive's function

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
        
        protected DirectFunctionResult_ControllerBase(IFactor_T_Controller<int>[] controllersForInputs, TCore core) : 
            base(core)
        {
            inputControllers = controllersForInputs;
        }
    }
}