using Causality;
using Causality.States;
using Core.Causality;
using Core.States;
using Tests.Causality.Interfaces;

namespace Tests.Causality.Factories
{
    public abstract class State_T_Factory<TState, TValue> : IState_T_Factory<TState, TValue>
        where TState : IState<TValue>
    {
        protected TState manipulatedInstance;
        
        public abstract TValue CreateRandomInstanceOfValuesType();
        public abstract TValue CreateRandomInstanceOfValuesType_NotEqualTo(TValue valueToAvoid);
        
        public void ObserveProcess(IProcess process, IInteraction interaction)
        {
            CausalObserver.ForThread.ObserveInteractions(process, interaction);
        }
        
        public abstract TState CreateInstance_WithValue(TValue value);
        public abstract void   ChangeValueTo(TValue newValue);
        public abstract TState CreateInstance();
    }
    
    
    
    public abstract class State_T_Factory<TValue> : State_T_Factory<State<TValue>, TValue>
    {
        public override State<TValue> CreateInstance()
        {
            var value = CreateRandomInstanceOfValuesType();

            return new State<TValue>(value);
        }

        public override State<TValue> CreateInstance_WithValue(TValue value) => new State<TValue>(null, value);
    }
}