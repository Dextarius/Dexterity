using Core.States;
using Tests.Tools.Interfaces;

namespace Tests.Tools.Factories.Controllers
{
    public abstract class Result_Controller<TCore, TValue> : ReactorCore_Controller<TCore>, IResult_Controller<TValue>
        where TCore : IResult<TValue>
    {
        public abstract TValue ExpectedValue { get; }

        public abstract TValue ChangeValueToANonEqualValue();
        public abstract TValue SetValueToAnEqualValue();
        public abstract TValue GetRandomInstanceOfValuesType_NotEqualTo(TValue valueToAvoid);
        

        protected Result_Controller(TCore controlledInstance) : base(controlledInstance)
        {

        }

        IResult<TValue> IResult_Controller<TValue>.ControlledInstance => ControlledInstance;
    }
}