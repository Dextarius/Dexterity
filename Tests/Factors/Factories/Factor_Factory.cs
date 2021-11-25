using Causality.States;
using Factors;
using Tests.Causality.Interfaces;

namespace Tests.Factors.Factories
{
    public abstract class Factor_Factory<TFactor> : IFactory<TFactor>  where TFactor : Factor
    { 
        public abstract TFactor CreateInstance();
        public abstract TFactor CreateInstance_WithName();
    }
    
    public abstract class Proactor_Factory<TFactor> : IFactory<TFactor>  where TFactor : Factor
    { 
        public abstract TFactor CreateInstance();
    }
}