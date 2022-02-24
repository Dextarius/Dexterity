using Core.Causality;
using Core.Factors;
using Factors;
using Tests.Tools.Interfaces;
using Tests.Tools.Mocks.Processes;

namespace Tests.Tools.Factories
{
    // public class ReactionFactory : IReactorFactory<Reaction>
    // {
    //     public Reaction CreateInstance() => new Reaction(Tests.Tools.Tools.DoNothing);
    //
    //     public Reaction CreateInstance_WhoseUpdateInvolves(IFactor factorToInvolve)
    //     {
    //         var process = new InvolveFactorProcess(factorToInvolve);
    //
    //         return new Reaction(process);
    //     }
    //     
    //     public Reaction CreateInstance_WhoseUpdateCalls(IProcess processToCall) => new Reaction(processToCall);
    // }
}