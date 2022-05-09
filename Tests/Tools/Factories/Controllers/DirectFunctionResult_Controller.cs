using System;
using Factors.Cores.DirectReactorCores;
using Tests.Tools.Interfaces;

namespace Tests.Tools.Factories.Controllers
{
    public class DirectFunctionResult_Controller : 
        DirectFunctionResult_ControllerBase<DirectFunctionResult<int, int>, int>
    {
        private readonly Func<int, int> valueFunction;

        protected override int  CallValueFunction() => valueFunction(inputControllers[0].ControlledInstance.Value);

        public override int GetRandomInstanceOfValuesType_NotEqualTo(int valueToAvoid) => 
            Tools.GenerateRandomIntNotEqualTo(valueToAvoid);

        public DirectFunctionResult_Controller(IFactor_T_Controller<int> inputSourceController) : 
            base(new []{ inputSourceController })
        {
            valueFunction      = (input) => input;
            ControlledInstance = new DirectFunctionResult<int, int>(valueFunction, inputSourceController.ControlledInstance);
        }

        public DirectFunctionResult_Controller() : this(new DirectState_Controller())
        {
            
        }
    }


    public class DirectFunctionResult2_Controller : 
        DirectFunctionResult_ControllerBase<DirectFunctionResult<int, int, int>, int>
    {
        private readonly Func<int, int, int> valueFunction;
        
        protected override int CallValueFunction() => valueFunction(inputControllers[0].ControlledInstance.Value, 
                                                                    inputControllers[1].ControlledInstance.Value);

        public override int GetRandomInstanceOfValuesType_NotEqualTo(int valueToAvoid) => 
            Tools.GenerateRandomIntNotEqualTo(valueToAvoid);


        public DirectFunctionResult2_Controller(IFactor_T_Controller<int> inputSourceController1, 
                                                IFactor_T_Controller<int> inputSourceController2) : 
            base(new [] { inputSourceController1, inputSourceController2 })
        {
            valueFunction      = (input1, input2) => input1 + input2;
            ControlledInstance = new DirectFunctionResult<int, int, int>(valueFunction, 
                                                                         inputSourceController1.ControlledInstance,
                                                                         inputSourceController2.ControlledInstance);
        }

        public DirectFunctionResult2_Controller() : this(new DirectState_Controller(), 
                                                         new DirectState_Controller())
        {
            
        }
    }
    
    
    
    public class DirectFunctionResult3_Controller : 
        DirectFunctionResult_ControllerBase<DirectFunctionResult<int, int, int, int>, int>
    {
        private readonly Func<int, int, int, int> valueFunction;
        
        protected override int CallValueFunction() => valueFunction(inputControllers[0].ControlledInstance.Value, 
                                                                    inputControllers[1].ControlledInstance.Value,
                                                                    inputControllers[2].ControlledInstance.Value);

        public override int GetRandomInstanceOfValuesType_NotEqualTo(int valueToAvoid) => 
            Tools.GenerateRandomIntNotEqualTo(valueToAvoid);


        public DirectFunctionResult3_Controller(IFactor_T_Controller<int> inputSourceController1, 
                                                IFactor_T_Controller<int> inputSourceController2, 
                                                IFactor_T_Controller<int> inputSourceController3) : 
            base(new [] { inputSourceController1, inputSourceController2, inputSourceController3 })
        {
            valueFunction      = (input1, input2, input3) => input1 + input2 + input3;
            ControlledInstance = new DirectFunctionResult<int, int, int, int>(valueFunction, 
                                                                              inputSourceController1.ControlledInstance,
                                                                              inputSourceController2.ControlledInstance,
                                                                              inputSourceController2.ControlledInstance);
        }

        public DirectFunctionResult3_Controller() : this(new DirectState_Controller(), 
                                                         new DirectState_Controller(),
                                                         new DirectState_Controller())
        {
            
        }
    }
}