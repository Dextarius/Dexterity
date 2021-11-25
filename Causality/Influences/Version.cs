// using Core.Causality;
// using Core.States;
//
// namespace Causality.States
// {
//     public readonly struct StateVersion
//     {
//         public readonly IState State;
//         public readonly long   VersionNumber;
//
//         public StateVersion(IState state, long versionNumber)
//         {
//             State = state;
//             VersionNumber = versionNumber;
//         }
//     }
//     
//     public readonly struct CausalVersion
//     {
//         //public readonly ICausalState State;
//         public readonly long VersionNumber;
//
//         public CausalVersion(IState state, long versionNumber)
//         {
//             //State = state;
//             VersionNumber = versionNumber;
//         }
//     }
//     
//     
//     public class Version
//     {
//         private Version alternateSource;
//         private int     number = 0;
//
//         public int Number
//         {
//             get
//             {
//                 if (alternateSource is null)
//                 {
//                     return number;
//                 }
//                 else
//                 {
//                     return alternateSource.Number;
//                 }
//             }
//         }
//
//         public void SetSource(Version source) => alternateSource = source;
//         public int  Increment()               => number++;
//     }
//
// }