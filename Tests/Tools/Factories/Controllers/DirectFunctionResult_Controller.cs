using System;
using Factors.Cores.DirectReactorCores;
using Tests.Tools.Interfaces;
using static Tests.Tools.Tools;

namespace Tests.Tools.Factories.Controllers
{
    public class DirectFunctionResult_Controller : 
        DirectFunctionResult_ControllerBase<DirectFunctionResult<int, int>, int>
    {
        private static readonly Func<int, int> defaultValueFunction = ReturnArgumentValue;
        private readonly        Func<int, int> valueFunction;

        protected override int  CallValueFunction() => valueFunction(inputControllers[0].ControlledInstance.Value);

        public override int GetRandomInstanceOfValuesType_NotEqualTo(int valueToAvoid) => 
            Tools.GenerateRandomIntNotEqualTo(valueToAvoid);

        public DirectFunctionResult_Controller(IFactor_T_Controller<int> inputSourceController) : 
            base(new [] { inputSourceController }, 
                 new DirectFunctionResult<int, int>(defaultValueFunction, inputSourceController.ControlledInstance)
                )
        {
            valueFunction = defaultValueFunction;
        }

        public DirectFunctionResult_Controller() : this(new DirectProactiveCore_Controller())
        {
            
        }
    }
    

    public class DirectFunctionResult2_Controller : 
        DirectFunctionResult_ControllerBase<DirectFunctionResult<int, int, int>, int>
    {
        private static readonly Func<int, int, int> defaultValueFunction = AddValues;
        private readonly        Func<int, int, int> valueFunction;
        
        protected override int CallValueFunction() => valueFunction(inputControllers[0].ControlledInstance.Value, 
                                                                    inputControllers[1].ControlledInstance.Value);

        public override int GetRandomInstanceOfValuesType_NotEqualTo(int valueToAvoid) => 
            Tools.GenerateRandomIntNotEqualTo(valueToAvoid);


        public DirectFunctionResult2_Controller(IFactor_T_Controller<int> inputSourceController1, 
                                                IFactor_T_Controller<int> inputSourceController2) : 
            base(new [] { inputSourceController1, inputSourceController2 },
                 new DirectFunctionResult<int, int, int>(defaultValueFunction, 
                                                         inputSourceController1.ControlledInstance,
                                                         inputSourceController2.ControlledInstance))
        {
            valueFunction = defaultValueFunction;
        }

        public DirectFunctionResult2_Controller() : this(new DirectProactiveCore_Controller(), 
                                                         new DirectProactiveCore_Controller())
        {
            
        }
    }
    
    
    
    public class DirectFunctionResult3_Controller : 
        DirectFunctionResult_ControllerBase<DirectFunctionResult<int, int, int, int>, int>
    {
        private static readonly Func<int, int, int, int> defaultValueFunction = AddValues;
        private        readonly Func<int, int, int, int> valueFunction;
        
        
        protected override int CallValueFunction() => valueFunction(inputControllers[0].ControlledInstance.Value, 
                                                                    inputControllers[1].ControlledInstance.Value,
                                                                    inputControllers[2].ControlledInstance.Value);

        public override int GetRandomInstanceOfValuesType_NotEqualTo(int valueToAvoid) => 
            Tools.GenerateRandomIntNotEqualTo(valueToAvoid);


        public DirectFunctionResult3_Controller(IFactor_T_Controller<int> inputSourceController1, 
                                                IFactor_T_Controller<int> inputSourceController2, 
                                                IFactor_T_Controller<int> inputSourceController3) : 
            base(new [] { inputSourceController1, inputSourceController2, inputSourceController3 },
                 new DirectFunctionResult<int, int, int, int>(defaultValueFunction, 
                                                              inputSourceController1.ControlledInstance,
                                                              inputSourceController2.ControlledInstance,
                                                              inputSourceController2.ControlledInstance))
        {
            valueFunction = defaultValueFunction;
        }

        public DirectFunctionResult3_Controller() : this(new DirectProactiveCore_Controller(), 
                                                         new DirectProactiveCore_Controller(),
                                                         new DirectProactiveCore_Controller())
        {
            
        }
    }
}