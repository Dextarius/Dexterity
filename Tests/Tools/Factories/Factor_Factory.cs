using Factors;
using Tests.Tools.Interfaces;

namespace Tests.Tools.Factories
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