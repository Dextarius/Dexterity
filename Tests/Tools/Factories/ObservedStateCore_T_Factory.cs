using Factors.Cores.ProactiveCores;
using Tests.Tools.Interfaces;

namespace Tests.Tools.Factories
{
    public abstract class ObservedStateCore_T_Factory<TValue> :  IState_T_Factory<ObservedStateCore<TValue>, TValue>
    {
        public ObservedStateCore<TValue> CreateInstance_WithValue(TValue value) => new ObservedStateCore<TValue>(value);
        
        public ObservedStateCore<TValue> CreateInstance()
        {
            var value = CreateRandomValue();

            return new ObservedStateCore<TValue>(value);
        }

        public ObservedStateCore<TValue> CreateStableInstance() => CreateInstance();

        public abstract TValue CreateRandomValue();
        public abstract TValue CreateRandomValueNotEqualTo(TValue valueToAvoid);
    }
}