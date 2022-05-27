using Factors.Cores.ProactiveCores;
using Tests.Tools.Interfaces;

namespace Tests.Tools.Factories
{
    public abstract class ObservedStateCore_T_Factory<TValue> :  IState_T_Factory<ObservedProactiveCore<TValue>, TValue>
    {
        public ObservedProactiveCore<TValue> CreateInstance_WithValue(TValue value) => new ObservedProactiveCore<TValue>(value);
        
        public ObservedProactiveCore<TValue> CreateInstance()
        {
            var value = CreateRandomValue();

            return new ObservedProactiveCore<TValue>(value);
        }

        public ObservedProactiveCore<TValue> CreateStableInstance() => CreateInstance();

        public abstract TValue CreateRandomValue();
        public abstract TValue CreateRandomValueNotEqualTo(TValue valueToAvoid);
    }
}