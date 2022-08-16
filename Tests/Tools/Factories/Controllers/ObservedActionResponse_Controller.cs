using Factors.Cores.ObservedReactorCores;
using Tests.Tools.Interfaces;

namespace Tests.Tools.Factories.Controllers
{
    public class ObservedActionResponse_Controller : ActionBasedResponse_Controller<ObservedActionResponse>
    {
        private readonly IFactor_T_Controller<int> inputController;

        private static void DoNothing(int input) { }

        protected override void ChangeInputsToANonEqualValue() => inputController.ChangeValueToANonEqualValue();
        protected override void ChangeInputsToAnEqualValue()   => inputController.SetValueToAnEqualValue();
        public    override void SetOffInstancesTriggers()      => ChangeInputsToANonEqualValue();


        public ObservedActionResponse_Controller(IFactor_T_Controller<int> inputSourceController) :
            base(new ObservedActionResponse(() => DoNothing(inputSourceController.ControlledInstance.Value)))
        {
            inputController = inputSourceController;
        }

        public ObservedActionResponse_Controller() : this(new Proactive_Controller<ObservedProactiveCore_Controller, int>())
        {
            
        }
    }
}