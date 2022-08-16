using Core.Factors;
using Core.States;
using Tests.Tools.Interfaces;

namespace Tests.Tools.Factories.Controllers
{
    public class Reaction_Controller<TCoreController> : IReaction_Controller
        where TCoreController : IReactorCore_Controller, new()
    {
        public IReactor        ControlledInstance { get; protected init; }
        public TCoreController CoreController     { get; }
        public IReactorCore    Core               => CoreController.ControlledInstance;

        public bool CheckIfTriggered()         => ControlledInstance.IsTriggered;
        public void MakeStableAndUntriggered() => ControlledInstance.AttemptReaction();
        public void MakeNecessary()            => ControlledInstance.IsReflexive = true;
        public void SetOffInstancesTriggers()  => CoreController.SetOffInstancesTriggers();

        ITriggeredState ITriggeredState_Controller.ControlledInstance => ControlledInstance;
    }
}