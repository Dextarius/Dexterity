using Core.Factors;
using Tests.Tools.Interfaces;

namespace Tests.Tools.Factories.Controllers
{
    public abstract class Factor_T_Controller<TFactor, TValue> : IFactor_T_Controller<TFactor, TValue> 
        where TFactor : IFactor<TValue>
    {
        #region Instance Fields


        
        #endregion


        #region Properties

        public TFactor ControlledInstance { get; protected init; }

        #endregion
        
        
        #region Instance Methods

        public abstract TValue ChangeValueToANonEqualValue();
        public abstract TValue ChangeValueToAnEqualValue();

        #endregion


        #region Explicit Implementations

        IFactor<TValue> IFactor_T_Controller<TValue>.ControlledInstance => ControlledInstance;

        #endregion
    }
}