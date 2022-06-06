using Factors;
using Factors.Cores.ProactiveCores;
using Tests.Tools.Interfaces;

namespace Tests.Tools.Factories
{
    public abstract class ObservedProactiveCore_T_Factory<TValue> :  IFactor_T_Factory<Proactive<TValue>, TValue>
    {
        public ObservedProactiveCore<TValue> CreateInstance_WithValue(TValue value) => 
            new ObservedProactiveCore<TValue>(value);
        
        public Proactive<TValue> CreateInstance()
        {
            var value = CreateRandomValue();
            var core = new ObservedProactiveCore<TValue>(value);
            var proactive = new Proactive<TValue>(core);

            core.SetOwner(proactive);

            return proactive;
        }

        public Proactive<TValue> CreateStableInstance() => CreateInstance();

        public abstract TValue CreateRandomValue();
        public abstract TValue CreateRandomValueNotEqualTo(TValue valueToAvoid);
    }
}