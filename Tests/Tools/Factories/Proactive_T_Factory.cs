using Factors;
using Tests.Tools.Interfaces;

namespace Tests.Tools.Factories
{
    public abstract class Proactive_T_Factory<TValue> :  IFactor_T_Factory<Proactive<TValue>, TValue>
    {
        public Proactive<TValue> CreateInstance_WithValue(TValue value) => new Proactive<TValue>(value);
        
        public Proactive<TValue> CreateInstance()
        {
            var value = CreateRandomInstanceOfValuesType();

            return new Proactive<TValue>(value);
        }

        public abstract TValue CreateRandomInstanceOfValuesType();
        public abstract TValue CreateRandomInstanceOfValuesType_NotEqualTo(TValue valueToAvoid);
    }
}