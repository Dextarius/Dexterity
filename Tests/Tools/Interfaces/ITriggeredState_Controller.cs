using Core.Factors;
using Core.States;

namespace Tests.Tools.Interfaces
{
    public interface ITriggeredState_Controller
    {
        ITriggeredState ControlledInstance { get; }
       // int             UpdateCount        { get; }

       void SetOffInstancesTriggers();
    }

    public interface IReactorCore_Controller : ITriggeredState_Controller
    {
        new IReactorCore ControlledInstance { get; }

    }
    
    public interface IReactor_Controller : ITriggeredState_Controller
    {
        new IReactor ControlledInstance { get; }

    }
    
    public interface IResult_Controller<TValue> : IReactorCore_Controller
    {
        new IResult<TValue> ControlledInstance { get; }
        TValue              ExpectedValue      { get; }
        
        TValue ChangeValueToANonEqualValue();
        TValue SetValueToAnEqualValue();
        TValue GetRandomInstanceOfValuesType_NotEqualTo(TValue valueToAvoid);
    }
    
    public interface IFactorCore_Controller
    {
        IFactorCore ControlledInstance { get; }
    }
    
    public interface IProactiveCore_Controller<TValue> : IFactorCore_Controller
    {
        new IProactiveCore<TValue> ControlledInstance { get; }
        
        TValue ChangeValueToANonEqualValue();
        TValue SetValueToAnEqualValue();
        TValue GetRandomInstanceOfValuesType_NotEqualTo(TValue valueToAvoid);
    }

}
