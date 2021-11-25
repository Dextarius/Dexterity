using Causality.Processes;
using Causality.States;
using Core.Causality;
using Core.Factors;
using Tests.Causality.Mocks;

namespace Tests.Causality.Factories
{
    public class Response_Factory : Result_Factory<Response>
    {
        public override Response CreateInstance()
        {
            var process = ActionProcess.CreateFrom(Tools.DoNothing);
            
            return new Response(process);
        }
        
        public override Response CreateInstance_WhoseUpdateCalls(IProcess processToCall) => new Response(processToCall);

        public override Response CreateInstance_WhoseUpdateInvolves(IFactor factorToInvolve)
        {
            var process = new InvolveFactorProcess(factorToInvolve);
            
            return new Response(process);
        }
    }
}