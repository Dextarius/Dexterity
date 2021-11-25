using Factors;

namespace Tests.Causality.Factories
{
    public abstract class Proactive_T_Factory<TValue> : State_T_Factory<Proactive<TValue>, TValue>
    {
        public override Proactive<TValue> CreateInstance()
        {
            var value = CreateRandomInstanceOfValuesType();

            return new Proactive<TValue>(value);
        }

        public override Proactive<TValue> CreateInstance_WithValue(TValue value) => new Proactive<TValue>(value);
    }
}