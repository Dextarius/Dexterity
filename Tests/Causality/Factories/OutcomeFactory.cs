using Causality.Processes;
using Causality.States;
using Core.Causality;
using Core.Factors;
using Tests.Causality.Interfaces;
using Tests.Causality.Mocks;

namespace Tests.Causality.Factories
{
    public class OutcomeFactory : IResultFactory<Response>
    {
        public Response CreateInstance()
        {
            var process = ActionProcess.CreateFrom(Tools.DoNothing);
            
            return new Response(process);
        }

        public Response CreateInstance_WhoseUpdateInvolves(IFactor factorToInvolve)
        {
            var process = new InvolveFactorProcess(factorToInvolve);

            return new Response(process);
        }
        
        public Response CreateInstance_WhoseUpdateCalls(IProcess processToCall) => new Response(processToCall);
    }
}