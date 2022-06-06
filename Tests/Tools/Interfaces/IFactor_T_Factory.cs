using Core.Factors;

namespace Tests.Tools.Interfaces
{
    public interface IFactor_T_Factory<out TFactor, TValue> : IFactory<TFactor>, IRandomGenerator<TValue>
        where TFactor : IFactor<TValue>
    {
       // TState CreateInstance_WithValue(TValue value);
    }
}