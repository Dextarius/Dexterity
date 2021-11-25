using Core.Causality;
using Core.Factors;
using Factors;
using Tests.Causality.Interfaces;
using Tests.Causality.Mocks;

namespace Tests.Causality.Factories
{
    public class ReactionFactory : IResultFactory<Reaction>
    {
        public Reaction CreateInstance() => new Reaction(Tools.DoNothing);

        public Reaction CreateInstance_WhoseUpdateInvolves(IFactor factorToInvolve)
        {
            var process = new InvolveFactorProcess(factorToInvolve);

            return new Reaction(process);
        }
        
        public Reaction CreateInstance_WhoseUpdateCalls(IProcess processToCall) => new Reaction(processToCall);
    }
}