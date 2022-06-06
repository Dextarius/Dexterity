using Core.Factors;
using Core.States;
using Factors;
using Tests.Tools.Interfaces;

namespace Tests.Tools.Factories.Controllers
{
    public abstract class Reactive_Controller<TCore, TValue> : Factor_T_Controller<Reactive<TValue>, TValue>, 
        IReactive_Controller<Reactive<TValue> , TValue>, IFactorSubscriberController<Reactive<TValue>>
        where TCore : IResult<TValue>
    {
        public abstract TValue ExpectedValue { get; }
        public          TCore  Core          { get; }
        
        public bool CheckIfTriggered()         => ControlledInstance.HasBeenTriggered;
        public void MakeStableAndUntriggered() => ControlledInstance.AttemptReaction();
        public void MakeNecessary()            => ControlledInstance.IsReflexive = true;

        protected Reactive_Controller(TCore core)
        {
            Core = core;
            ControlledInstance = new Reactive<TValue>(core);
        }
    }
}