using Factors.Cores.ProactiveCores;
using Tests.Tools.Interfaces;

namespace Tests.Tools.Factories
{
    public abstract class State_T_Factory<TState, TValue> : IState_T_Factory<TState, TValue>
        where TState : ObservedStateCore<TValue>
    {
        public abstract TState CreateInstance();
        public abstract TState CreateInstance_WithValue(TValue value);
        public abstract TValue CreateRandomValue();
        public abstract TValue CreateRandomValueNotEqualTo(TValue valueToAvoid);
        
        
        // public void ObserveProcess(IProcess process, IObserved interaction)
        // {
        //     CausalObserver.ForThread.ObserveInteractions(process, interaction);
        // }

        // public abstract void ChangeValueTo(TValue newValue);
    }
    
    
    public abstract class State_T_Factory<TValue> : State_T_Factory<ObservedStateCore<TValue>, TValue>
    {
        public override ObservedStateCore<TValue> CreateInstance()
        {
            var value = CreateRandomValue();

            return new ObservedStateCore<TValue>(value);
        }

        public override ObservedStateCore<TValue> CreateInstance_WithValue(TValue value) => 
            new ObservedStateCore<TValue>(value);
    }
}