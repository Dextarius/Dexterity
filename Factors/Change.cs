using Core.Factors;

namespace Factors
{
    //- Potential other names: Outcome, Result, Effect, 
    public abstract class Change<T, TCore> : Factor<TCore> where TCore : IFactorCore
    {
        //- Essentially just an Event that passes an argument, like the normal Reactives used by other libraries.
        //  The point is that this one can track IsNecessary chains and such.

        protected Change(TCore factorCore, string factorsName = "Factor") : base(factorCore, factorsName)
        {
        }
    }
}