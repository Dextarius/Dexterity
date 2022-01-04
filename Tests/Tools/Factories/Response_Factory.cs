// using Core.Causality;
// using Core.Factors;
// using Factors.Outcomes.ObservedOutcomes;
// using Tests.Causality.Mocks;
//
// namespace Tests.Causality.Factories
// {
//     public class Response_Factory : Reactor_Factory<ObservedResponse>
//     {
//         public override ObservedResponse CreateInstance()
//         {
//             var process = ActionProcess.CreateFrom(Tools.DoNothing);
//             
//             return new ObservedResponse(process);
//         }
//         
//         public override ObservedResponse CreateInstance_WhoseUpdateCalls(IProcess processToCall) => new ObservedResponse(processToCall);
//
//         public override ObservedResponse CreateInstance_WhoseUpdateInvolves(IFactor factorToInvolve)
//         {
//             var process = new InvolveFactorProcess(factorToInvolve);
//             
//             return new ObservedResponse(process);
//         }
//     }
// }