// using System.Runtime.InteropServices;
// using Core.Factors;
//
// namespace Factors.Modifiers
// {
//     public class ReactiveNumericModifier<T> : Reactor<IReactiveNumericModifierCore>, INumericMod<T>
//     {
//         private bool isEnabled;
//
//         #region Properties
//
//         public bool IsEnabled
//         {
//             get => isEnabled;
//             set
//             {
//                 if (isEnabled != value)
//                 {
//                     if (value is true)
//                     {
//                         if (IsNecessary)
//                         {
//                             OnNecessary();
//                         }
//                     }
//                     else
//                     {
//                         if (IsNecessary)
//                         {
//                             OnNotNecessary(); //- Will this work now that we reworked IsNecessary and IsReflexive?
//                         }
//                         
//                         core.IsEnabled = false; //- Do we need this? 
//                     }
//                     
//                     isEnabled = value;
//                     TriggerSubscribers();
//                 }
//             }
//         }
//
//         public NumericModType ModType
//         {
//             get
//             {
//                 if (IsEnabled is false)
//                 {
//                      return NumericModType.Ignore;
//                 }
//                 else return core.ModType;
//             }
//             set
//             {
//                 if (core.ModType != value)
//                 {
//                     core.ModType = value;
//                     TriggerSubscribers();
//                 }
//             }
//         }
//
//         public int ModPriority
//         {
//             get => core.ModPriority;
//             set
//             {
//                 if (core.ModPriority != value)
//                 {
//                     core.ModPriority = value;
//                     TriggerSubscribers();
//                 }
//             }
//         }
//
//         public double Amount
//         {
//             get
//             {
//                 AttemptReaction();
//                 return core.Amount;
//             }
//         }
//
//         public override bool IsNecessary => base.IsNecessary && IsEnabled;
//
//         #endregion
//         
//
//         #region Constructors
//
//         public ReactiveNumericModifier(IReactiveNumericModifierCore factorCore, string modifiersName = nameof(ProactiveNumericModifier)) : 
//             base(factorCore, modifiersName)
//         {
//             
//         }
//
//         #endregion
//     }
// }