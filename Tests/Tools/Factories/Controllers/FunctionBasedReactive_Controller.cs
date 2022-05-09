using System.Diagnostics;
using Core.Factors;
using Tests.Tools.Interfaces;

namespace Tests.Tools.Factories.Controllers
{
    public abstract class FunctionBasedReactive_Controller<TReactive, TValue> : 
        Reactive_Controller<TReactive, TValue>
        where TReactive : IReactive<TValue>
    {
        public override TValue ExpectedValue => CallValueFunction();

        protected abstract TValue CallValueFunction();
        protected abstract void   ChangeInputsToANonEqualValue();
        protected abstract void   ChangeInputsToAnEqualValue();

        public override TValue ChangeValueToANonEqualValue()
        {
            ChangeInputsToANonEqualValue();
            Debug.Assert(ControlledInstance.HasBeenTriggered);

            return ControlledInstance.Value;
        }
        
        public override TValue SetValueToAnEqualValue()
        {
            ChangeInputsToAnEqualValue();
            ControlledInstance.ForceReaction();
            
            return ControlledInstance.Value;
        }
    }
}