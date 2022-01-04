// using System;
// using Core.Causality;
// using Core.Factors;
//
// namespace Tests.Causality.Factories
// {
//     public class Reactor_Int_Factory : Reactor_T_Factory<int>
//     {
//         private Random numberGenerator = new Random();
//         
//         public override int CreateRandomInstanceOfValuesType_NotEqualTo(int valueToAvoid) => 
//             Tools.GenerateRandomIntNotEqualTo(valueToAvoid);
//         
//         public override int CreateRandomInstanceOfValuesType() => Tools.GenerateRandomInt();
//
//
//
//     }
// }