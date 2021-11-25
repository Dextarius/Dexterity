using Core.Causality;
using Core.Factors;
using Core.States;

namespace Tests.Causality.Interfaces
{
    public interface IResultFactory<out TResult, TValue> : IResultFactory<TResult>, IState_T_Factory<TResult, TValue>
        where TResult : IResult<TValue>
    {
        TResult CreateInstance_WhoseUpdateCalls(IProcess<TValue> processToCall);
    }
    
    
    public interface IResultFactory<out TResult> : IFactory<TResult>
    {
        TResult CreateInstance_WhoseUpdateCalls(IProcess processToCall);
        TResult CreateInstance_WhoseUpdateInvolves(IFactor factorToInvolve);
    }
}