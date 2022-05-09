using Core.Factors;

namespace Tests.Tools.Interfaces
{
    public interface IFactor_T_Controller<TValue> 
    {
        IFactor<TValue> ControlledInstance { get; }

        TValue ChangeValueToANonEqualValue();
        TValue SetValueToAnEqualValue();
        TValue GetRandomInstanceOfValuesType_NotEqualTo(TValue valueToAvoid);
    }

    
    public interface IFactor_T_Controller<out TFactor, TValue> : IFactor_T_Controller<TValue>
        where TFactor : IFactor<TValue>
    {
       new TFactor ControlledInstance { get; }
    }
}