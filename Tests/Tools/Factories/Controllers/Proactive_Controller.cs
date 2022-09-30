using Core.States;
using Factors;
using Tests.Tools.Interfaces;

namespace Tests.Tools.Factories.Controllers
{
    public class Proactive_Controller<TCoreController, TValue> : Factor_T_Controller<Proactive<TValue>, TValue>
        where TCoreController : IProactiveCore_Controller<TValue>, new()

    {
        #region Properties

        public TCoreController CoreController { get; }

        public IProactiveCore<TValue> Core => CoreController.ControlledInstance;

        #endregion

        
        #region Instance Methods

        public override TValue ChangeValueToANonEqualValue() => CoreController.ChangeValueToANonEqualValue();
        public override TValue SetValueToAnEqualValue()      => CoreController.SetValueToAnEqualValue();
        public override TValue GetRandomInstanceOfValuesType_NotEqualTo(TValue valueToAvoid) =>
            CoreController.GetRandomInstanceOfValuesType_NotEqualTo(valueToAvoid);

        #endregion
        
                
        #region Constructors

        public Proactive_Controller()
        {
            CoreController     = new TCoreController();
            ControlledInstance = new Proactive<TValue>(Core);
        }

        #endregion
    }
}