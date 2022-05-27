using Factors.Cores.ProactiveCores;
using Tests.Tools.Interfaces;

namespace Tests.Tools.Factories
{
    public abstract class DirectStateCore_T_Factory<TValue> :  IState_T_Factory<DirectProactiveCore<TValue>, TValue>
    {
        public DirectProactiveCore<TValue> CreateInstance_WithValue(TValue value) => new DirectProactiveCore<TValue>(value);
        
        public DirectProactiveCore<TValue> CreateInstance()
        {
            var value = CreateRandomValue();

            return new DirectProactiveCore<TValue>(value);
        }

        public DirectProactiveCore<TValue> CreateStableInstance() => CreateInstance();


        public abstract TValue CreateRandomValue();
        public abstract TValue CreateRandomValueNotEqualTo(TValue valueToAvoid);
    }
}