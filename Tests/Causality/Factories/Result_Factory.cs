using Causality;
using Core.Causality;
using Core.Factors;
using Core.States;
using Tests.Causality.Interfaces;

namespace Tests.Causality.Factories
{
    public abstract class Result_Factory<TResult> : IResultFactory<TResult>  where TResult : IResult
    {
        public void ObserveProcess(IProcess process, IInteraction interaction)
        {
            CausalObserver.ForThread.ObserveInteractions(process, interaction);
        }
        
        public abstract TResult CreateInstance();
        public abstract TResult CreateInstance_WhoseUpdateCalls(IProcess processToCall);
        public abstract TResult CreateInstance_WhoseUpdateInvolves(IFactor factorToInvolve);
    }
}