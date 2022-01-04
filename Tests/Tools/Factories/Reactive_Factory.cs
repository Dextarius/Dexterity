using Core.Causality;
using Core.Factors;
using Factors;
using Tests.Tools.Interfaces;
using Tests.Tools.Mocks.Processes;

namespace Tests.Tools.Factories
{
    public abstract class Reactive_Factory<TValue> :  IReactorFactory<Reactive<TValue>, TValue>, IRandomGenerator<TValue>
    {
        #region Instance Methods
        public Reactive<TValue> CreateInstance()
        {
            TValue                     valueForInstance = CreateRandomInstanceOfValuesType();
            StoredValueProcess<TValue> valueProcess     = new StoredValueProcess<TValue>(valueForInstance);
            Reactive<TValue>           createdReactive  = new Reactive<TValue>(valueProcess);

            return createdReactive;
        }

        // public void ObserveProcess(IProcess process, IInteraction interaction) => CausalFactor.Observe(process, interaction);
        
        public Reactive<TValue> CreateInstance_WhoseUpdateCalls(IProcess<TValue> processToCall) =>
            new Reactive<TValue>(processToCall);

        public Reactive<TValue> CreateInstance_WithValue(TValue value)
        {
            var valueProcess = new StoredValueProcess<TValue>(value);

            return CreateInstance_WhoseUpdateCalls(valueProcess);
        }
        
        public Reactive<TValue> CreateInstance_WhoseUpdateCalls(IProcess processToCall)
        {
            var valueProcess = new RandomValueProcess<TValue>(this, processToCall);
            return new Reactive<TValue>(valueProcess);
        }
        public Reactive<TValue> CreateInstance_WhoseUpdateInvolves(IFactor factorToInvolve)
        {
            var valueProcess = new InvolveFactorProcess<TValue>(factorToInvolve);

            return new Reactive<TValue>(valueProcess);
        }
        
        public abstract TValue CreateRandomInstanceOfValuesType();
        public abstract TValue CreateRandomInstanceOfValuesType_NotEqualTo(TValue valueToAvoid);

        #endregion

        
        #region Explicit Implementations

        TValue IRandomGenerator<TValue>.CreateRandomValue() => CreateRandomInstanceOfValuesType();
        
        TValue IRandomGenerator<TValue>.CreateRandomValueNotEqualTo(TValue valueToAvoid) => 
            CreateRandomInstanceOfValuesType_NotEqualTo(valueToAvoid);

        #endregion
    }
}