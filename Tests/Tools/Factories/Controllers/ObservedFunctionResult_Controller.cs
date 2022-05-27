using System;
using Factors.Cores.ObservedReactorCores;
using Tests.Tools.Interfaces;

namespace Tests.Tools.Factories.Controllers
{
    public class ObservedFunctionResult_Controller : 
        FunctionBasedReactive_Controller<ObservedFunctionResult<int>, int>
    {
        private readonly Func<int>                 valueFunction;
        private readonly IFactor_T_Controller<int> inputController;
    
        protected override void ChangeInputsToANonEqualValue() => inputController.ChangeValueToANonEqualValue();
        protected override void ChangeInputsToAnEqualValue()   => inputController.SetValueToAnEqualValue();
        protected override int  CallValueFunction()            => valueFunction();
        
        public override int GetRandomInstanceOfValuesType_NotEqualTo(int valueToAvoid) => 
            Tools.GenerateRandomIntNotEqualTo(valueToAvoid);


        public ObservedFunctionResult_Controller(IFactor_T_Controller<int> inputSourceController)
        {
            inputController    = inputSourceController;
            valueFunction      = () => inputSourceController.ControlledInstance.Value;
            ControlledInstance = new ObservedFunctionResult<int>(valueFunction, TODO);
        }

        public ObservedFunctionResult_Controller() : this(new ObservedState_Controller())
        {
            
        }
    }
    
    
    // public class ObservedFunctionResult_Controller : 
    //     Reactive_Controller<ObservedFunctionResult<int>, int>
    // {
    //     private readonly Func<int> valueFunction;
    //     private          int       storedValue;
    //     
    //     public override int ExpectedValue => valueFunction();
    //     
    //     public override int ChangeValueToANonEqualValue()
    //     {
    //         storedValue = Tools.GenerateRandomIntNotEqualTo(storedValue);
    //         reactive.Trigger();
    //
    //         return reactive.Value;
    //     }
    //     public override int SetValueToAnEqualValue()
    //     {
    //         //- This does essentially nothing, except prevent odd situations from unintentionally
    //         //  changing the value to something new, such as this method being called before the 
    //         //  Reactive has done it's initial update, when the internal default value may be
    //         //  different than whatever the default value for storedValue was. 
    //         
    //         storedValue = reactive.Value;
    //         reactive.ForceReaction();
    //         
    //         return reactive.Value;
    //     }
    //
    //     public ObservedFunctionResult_Controller()
    //     {
    //         valueFunction = () => storedValue;
    //         reactive      = new ObservedFunctionResult<int>(valueFunction);
    //     }
    // }
}