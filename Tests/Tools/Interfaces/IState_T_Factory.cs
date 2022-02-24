using Core.Factors;

namespace Tests.Tools.Interfaces
{
    public interface IState_T_Factory<out TState, TValue> : IFactory<TState>, IRandomGenerator<TValue>
        where TState : IFactor<TValue>
    {
       // TState CreateInstance_WithValue(TValue value);
    }
}