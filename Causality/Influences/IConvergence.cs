// using System.Collections.Generic;
//
// namespace Causality.States
// {
//     public interface IConvergence : ICausalState
//     {
//         IReadOnlyList<ICausalState> Parents { get; }
//         
//         void Invalidate();
//         void Invalidate(int newVersionNumber);
//         void Destabilize();
//         bool Destabilize(int newVersionNumber);
//     }
// }