// using System.Collections.Generic;
// using Core.Factors;
// using Factors.Cores;
// using Factors.Cores.DirectReactorCores;
// using static Core.Tools.Numerics;
//
// namespace Factors.Modifiers
// {
//     public abstract class ReactiveModifierCore : DirectReactorCore, IReactiveNumericModifierCore
//     {
//         public            bool           IsEnabled   { get; set; }
//         public            NumericModType ModType     { get; set; }
//         public            int            ModPriority { get; set; }
//         public   abstract double         Amount      { get; }
//     }
//
//     public class CopyModifierCore : ReactiveModifierCore
//     {
//         public  IFactor<double> valueSource;
//         private double          currentAmount;
//
//
//         public override double Amount
//         {
//             get
//             {
//                 if (IsEnabled)
//                 {
//                     return currentAmount;
//                 }
//                 else return 0;
//             }
//         }
//
//         protected override IEnumerable<IFactor> Triggers         { get; }
//         public override    int                  NumberOfTriggers => 1;
//         
//         protected override bool CreateOutcome()
//         {
//             throw new System.NotImplementedException();
//         }
//         
//         
//         
//     }
// }