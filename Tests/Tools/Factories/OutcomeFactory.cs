// using Core.Causality;
// using Core.Factors;
// using Factors.Outcomes.ObservedOutcomes;
// using Tests.Causality.Interfaces;
// using Tests.Causality.Mocks;
//
// namespace Tests.Causality.Factories
// {
//     public class OutcomeFactory : IReactorFactory<ObservedResponse>
//     {
//         public ObservedResponse CreateInstance()
//         {
//             var process = ActionProcess.CreateFrom(Tools.DoNothing);
//             
//             return new ObservedResponse(process);
//         }
//
//         public ObservedResponse CreateInstance_WhoseUpdateInvolves(IFactor factorToInvolve)
//         {
//             var process = new InvolveFactorProcess(factorToInvolve);
//
//             return new ObservedResponse(process);
//         }
//         
//         public ObservedResponse CreateInstance_WhoseUpdateCalls(IProcess processToCall) => new ObservedResponse(processToCall);
//     }
// }