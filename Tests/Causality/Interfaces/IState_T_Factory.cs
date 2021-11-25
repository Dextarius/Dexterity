using Core.Causality;
using Core.States;

namespace Tests.Causality.Interfaces
{
    public interface IState_T_Factory<out TState, TValue> : IFactory<TState, TValue>
        where TState : IState<TValue>
    {
        void   ObserveProcess(IProcess process, IInteraction interaction);
        TState CreateInstance_WithValue(TValue value);

    }
}