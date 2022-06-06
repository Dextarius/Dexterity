using System;
using Factors;
using Factors.Cores.ObservedReactorCores;
using Tests.Tools.Interfaces;

namespace Tests.Tools.Factories
{
    public abstract class Proactive_T_Factory<TValue> :  IFactor_T_Factory<Proactive<TValue>, TValue>
    {
        public Proactive<TValue> CreateInstance_WithValue(TValue value) => new Proactive<TValue>(value);
        
        public Proactive<TValue> CreateInstance()
        {
            var value = CreateRandomValue();

            return new Proactive<TValue>(value);
        }
        public Proactive<TValue> CreateStableInstance() => CreateInstance();

        public abstract TValue CreateRandomValue();
        public abstract TValue CreateRandomValueNotEqualTo(TValue valueToAvoid);
    }
    

}