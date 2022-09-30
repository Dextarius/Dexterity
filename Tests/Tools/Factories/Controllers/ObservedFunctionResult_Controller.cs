using System;
using Factors.Cores.ObservedReactorCores;
using Tests.Tools.Interfaces;

namespace Tests.Tools.Factories.Controllers
{
    public class ObservedFunctionResult_Controller : 
        FunctionBasedResult_Controller<ObservedFunctionResult<int>, int>
    {
        private readonly IFactor_T_Controller<int> inputController;
        private readonly Func<int>                 valueFunction;

        protected override void ChangeInputsToANonEqualValue() => inputController.ChangeValueToANonEqualValue();
        protected override void ChangeInputsToAnEqualValue()   => inputController.SetValueToAnEqualValue();
        protected override int  CallValueFunction()            => valueFunction();
        public    override void SetOffInstancesTriggers()      => ChangeInputsToANonEqualValue();

        public override int GetRandomInstanceOfValuesType_NotEqualTo(int valueToAvoid) => 
            Tools.GenerateRandomIntNotEqualTo(valueToAvoid);


        public ObservedFunctionResult_Controller(IFactor_T_Controller<int> inputSourceController) : 
            base(new ObservedFunctionResult<int>(() => inputSourceController.ControlledInstance.Value))
        {
            inputController = inputSourceController;
            valueFunction   = () => inputSourceController.ControlledInstance.Value;
        }

        public ObservedFunctionResult_Controller() : 
            this(new Proactive_Controller<ObservedProactiveCore_Controller, int>())
        {
        }
    }
}