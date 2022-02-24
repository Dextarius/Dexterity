using Core.Factors;
using Tests.Tools.Interfaces;

namespace Tests.Tools.Factories.Controllers
{
    public abstract class Reactive_Controller<TReactor, TValue> : 
        Factor_T_Controller<TReactor, TValue>, IReactive_Controller<TReactor, TValue>, IFactorSubscriberController<TReactor>
        where TReactor : IReactive<TValue>
    {
        public abstract TValue ExpectedValue { get; }
        
        public bool CheckIfTriggered()         => ControlledInstance.HasBeenTriggered;
        public void MakeStableAndUntriggered() => ControlledInstance.AttemptReaction();
        public void MakeNecessary()            => ControlledInstance.IsReflexive = true;
    }
}