using Core.Causality;
using Core.Factors;
using Core.States;

namespace Tests.Tools.Interfaces
{
    public interface IReactorFactory<out TResult> : IFactory<TResult>
    {
        TResult CreateInstance_WhoseUpdateCalls(IProcess processToCall);
        TResult CreateInstance_WhoseUpdateInvolves(IFactor factorToInvolve);
    }

    public interface IReactorFactory<out TResult, TValue> : IReactorFactory<TResult>, IFactor_T_Factory<TResult, TValue>
        where TResult : IResult<TValue>
    {
        TResult CreateInstance_WhoseUpdateCalls(IProcess<TValue> processToCall);
    }
}