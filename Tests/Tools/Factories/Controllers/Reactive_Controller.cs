using Core.Factors;
using Core.States;
using Factors;
using Tests.Tools.Interfaces;

namespace Tests.Tools.Factories.Controllers
{
    public class Reactive_Controller<TCoreController, TValue> : Factor_T_Controller<Reactive<TValue>, TValue>, 
        IReactive_Controller<Reactive<TValue> , TValue>, IFactorSubscriberController<Reactive<TValue>>
        where TCoreController : IResult_Controller<TValue>, new()
    {
        #region Properties

        public TCoreController CoreController { get; }
        public IResult<TValue> Core           => CoreController.ControlledInstance;
        public TValue          ExpectedValue  => CoreController.ExpectedValue;

        #endregion

        
        #region Instance Methods

        public          bool   CheckIfTriggered()            => ControlledInstance.IsTriggered;
        public          void   MakeStableAndUntriggered()    => ControlledInstance.AttemptReaction();
        public          void   MakeNecessary()               => ControlledInstance.IsReflexive = true;
        public override TValue ChangeValueToANonEqualValue() => CoreController.ChangeValueToANonEqualValue();
        public override TValue SetValueToAnEqualValue()      => CoreController.SetValueToAnEqualValue();
        public override TValue GetRandomInstanceOfValuesType_NotEqualTo(TValue valueToAvoid) =>
            CoreController.GetRandomInstanceOfValuesType_NotEqualTo(valueToAvoid);

        #endregion

        
        #region Constructors

        public Reactive_Controller()
        {
            CoreController     = new TCoreController();
            ControlledInstance = new Reactive<TValue>(Core);
        }

        #endregion
    }
}