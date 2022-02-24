using Core.Factors;

namespace Tests.Tools.Interfaces
{
    public interface IFactor_T_Controller<out TValue> 
    {
        IFactor<TValue> ControlledInstance { get; }

        TValue ChangeValueToANonEqualValue();
        TValue ChangeValueToAnEqualValue();
    }

    
    public interface IFactor_T_Controller<out TFactor, out TValue> : IFactor_T_Controller<TValue>
        where TFactor : IFactor<TValue>
    {
       new TFactor ControlledInstance { get; }
    }
}