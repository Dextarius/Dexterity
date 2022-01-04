using Factors.Outcomes.Influences;
using Tests.Tools.Interfaces;

namespace Tests.Tools.Factories
{
    public abstract class Factor_T_Factory<TState, TValue> : IFactor_T_Factory<TState, TValue>
        where TState : ObservedState<TValue>
    {
        protected TState manipulatedInstance;
        
        public abstract TState CreateInstance();
        public abstract TState CreateInstance_WithValue(TValue value);
        public abstract TValue CreateRandomInstanceOfValuesType();
        public abstract TValue CreateRandomInstanceOfValuesType_NotEqualTo(TValue valueToAvoid);
        
        
        // public void ObserveProcess(IProcess process, IObserved interaction)
        // {
        //     CausalObserver.ForThread.ObserveInteractions(process, interaction);
        // }

        // public abstract void ChangeValueTo(TValue newValue);
    }
    
    
    public abstract class Factor_T_Factory<TValue> : Factor_T_Factory<ObservedState<TValue>, TValue>
    {
        public override ObservedState<TValue> CreateInstance()
        {
            var value = CreateRandomInstanceOfValuesType();

            return new ObservedState<TValue>(value);
        }

        public override ObservedState<TValue> CreateInstance_WithValue(TValue value) => 
            new ObservedState<TValue>(value);
    }
}