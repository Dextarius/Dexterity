using Core.Factors;

namespace Factors
{
    //- We may want to name this one Outcome, Result, Effect, 
    public class Change<T, TCore> : Factor<TCore> where TCore : IFactorCore
    {
        //- Essentially just an Event that passes an argument, like the normal Reactives used by other libraries.
        //  The point is that this one can track IsNecessary chains and such.
        
        public Change(TCore factorCore, string factorsName = "Factor") : base(factorCore, factorsName)
        {
        }
    }
}