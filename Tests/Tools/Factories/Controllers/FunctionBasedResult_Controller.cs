using System.Diagnostics;
using Core.States;

namespace Tests.Tools.Factories.Controllers
{
    public abstract class FunctionBasedResult_Controller<TCore, TValue> : Result_Controller<TCore, TValue>
        where TCore : IResult<TValue>
    {
        public override TValue ExpectedValue => CallValueFunction();
        
        
        public override TValue ChangeValueToANonEqualValue()
        {
            ChangeInputsToANonEqualValue();
            Debug.Assert(ControlledInstance.IsTriggered); //- What if the core is reflexive?
            ControlledInstance.AttemptReaction();

            return ControlledInstance.Value;
        }
        
        public override TValue SetValueToAnEqualValue()
        {
            ChangeInputsToAnEqualValue();
            ControlledInstance.ForceReaction();
            
            return ControlledInstance.Value;
        }
        
        protected abstract TValue CallValueFunction();
        protected abstract void   ChangeInputsToANonEqualValue();
        protected abstract void   ChangeInputsToAnEqualValue();
        
        
        protected FunctionBasedResult_Controller(TCore core) : base(core)
        {
        }
    }
}