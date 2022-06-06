using Factors;
using Factors.Cores.ProactiveCores;
using Tests.Tools.Interfaces;

namespace Tests.Tools.Factories
{
    public abstract class DirectProactiveCore_T_Factory<TValue> :  IFactor_T_Factory<Proactive<TValue>, TValue>
    {
        public Proactive<TValue> CreateInstance_WithValue(TValue value)
        {
            var core            = new DirectProactiveCore<TValue>(value);
            var createdInstance = new Proactive<TValue>(core);
            
            return createdInstance;
        }

        public Proactive<TValue> CreateInstance() => CreateInstance_WithValue(CreateRandomValue());

        public Proactive<TValue> CreateStableInstance() => CreateInstance();


        public abstract TValue CreateRandomValue();
        public abstract TValue CreateRandomValueNotEqualTo(TValue valueToAvoid);
        
        
    }
}