using Core.Factors;
using Tests.Tools.Interfaces;

namespace Tests.Tools.Factories.Controllers
{
    public abstract class Factor_T_Controller<TFactor, TValue> : IFactor_T_Controller<TFactor, TValue> 
        where TFactor : IFactor<TValue>
    {
        #region Properties

        public TFactor ControlledInstance { get; protected init; }
        //- The property is init only because some of the Observed Factors can't
        //  create an instance of their Factor type without using non-static data

        #endregion
        
        
        #region Instance Methods

        public abstract TValue ChangeValueToANonEqualValue();
        public abstract TValue SetValueToAnEqualValue();
        public abstract TValue GetRandomInstanceOfValuesType_NotEqualTo(TValue valueToAvoid);

        #endregion


        #region Explicit Implementations

        IFactor<TValue> IFactor_T_Controller<TValue>.ControlledInstance => ControlledInstance;

        #endregion
    }
}