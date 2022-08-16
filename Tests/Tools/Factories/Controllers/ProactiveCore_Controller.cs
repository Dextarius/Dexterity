using Core.States;
using Tests.Tools.Interfaces;

namespace Tests.Tools.Factories.Controllers
{
    public abstract class ProactiveCore_Controller<TCore, TValue> : FactorCore_Controller<TCore>, 
        IProactiveCore_Controller<TValue>
        where TCore : IProactiveCore<TValue>
    {
        public abstract TValue ChangeValueToANonEqualValue();
        public abstract TValue SetValueToAnEqualValue();
        public abstract TValue GetRandomInstanceOfValuesType_NotEqualTo(TValue valueToAvoid);

        protected ProactiveCore_Controller(TCore controlledInstance)
        {
            ControlledInstance = controlledInstance;
        }

        IProactiveCore<TValue> IProactiveCore_Controller<TValue>.ControlledInstance => ControlledInstance;
    }
}