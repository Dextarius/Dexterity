using Factors.Cores.ProactiveCores;
using Tests.Tools.Interfaces;

namespace Tests.Tools.Factories
{
    public abstract class DirectStateCore_T_Factory<TValue> :  IState_T_Factory<DirectStateCore<TValue>, TValue>
    {
        public DirectStateCore<TValue> CreateInstance_WithValue(TValue value) => new DirectStateCore<TValue>(value);
        
        public DirectStateCore<TValue> CreateInstance()
        {
            var value = CreateRandomValue();

            return new DirectStateCore<TValue>(value);
        }

        public abstract TValue CreateRandomValue();
        public abstract TValue CreateRandomValueNotEqualTo(TValue valueToAvoid);
    }
}