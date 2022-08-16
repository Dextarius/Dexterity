using Core.Factors;
using Core.States;
using Tests.Tools.Interfaces;
using Tests.Tools.Mocks;

namespace Tests.Tools.Factories.Controllers
{
    public abstract class ReactorCore_Controller<TCore> : FactorCore_Controller<TCore>, 
        IReactorCore_Controller,IFactorSubscriberController<TCore>
        where TCore : IReactorCore
    {
        public bool CheckIfTriggered()         => ControlledInstance.IsTriggered;
        public void MakeStableAndUntriggered() => ControlledInstance.AttemptReaction();
        public void MakeNecessary()            => ControlledInstance.IsReflexive = true;
        
        public abstract void SetOffInstancesTriggers();


        public ReactorCore_Controller(TCore controlledInstance)
        {
            ControlledInstance = controlledInstance;
        }

        ITriggeredState ITriggeredState_Controller.ControlledInstance => ControlledInstance;
        IReactorCore       IReactorCore_Controller.ControlledInstance => ControlledInstance;
    }
}