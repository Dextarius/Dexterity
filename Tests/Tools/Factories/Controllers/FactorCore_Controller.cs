using Core.Factors;
using Factors;
using Tests.Tools.Interfaces;

namespace Tests.Tools.Factories.Controllers
{
    public abstract class FactorCore_Controller<TCore> : IFactorCore_Controller
        where TCore : IFactorCore
    {
        #region Properties

        public TCore ControlledInstance { get; protected init; }
        //- The property is init only because some of the Observed Factors can't
        //  create an instance of their Factor type without using non-static data.

        #endregion
        

        #region Explicit Implementations

        IFactorCore IFactorCore_Controller.ControlledInstance => ControlledInstance;

        #endregion
    }
}