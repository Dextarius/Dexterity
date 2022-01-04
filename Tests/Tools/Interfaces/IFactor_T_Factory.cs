using Core.Factors;

namespace Tests.Tools.Interfaces
{
    public interface IFactor_T_Factory<out TState, TValue> : IFactory<TState, TValue>
        where TState : IFactor<TValue>
    {
       // void   ObserveProcess(IProcess process, IObserved interaction);
        TState CreateInstance_WithValue(TValue value);

    }
}