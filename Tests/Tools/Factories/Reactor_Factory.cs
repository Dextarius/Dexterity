using Core.Causality;
using Core.Factors;
using Tests.Tools.Interfaces;

namespace Tests.Tools.Factories
{
    public abstract class Reactor_Factory<TReactor> : IReactorFactory<TReactor>  where TReactor : IReactor
    {
        public abstract TReactor CreateInstance();
        public abstract TReactor CreateInstance_WhoseUpdateCalls(IProcess processToCall);
        public abstract TReactor CreateInstance_WhoseUpdateInvolves(IFactor factorToInvolve);
    }
}