// namespace Causality.States
// {
//     public interface IInfluence
//     {
//         bool IsStable { get; set; }
//         void AddDependent(Influence influenceToAdd);
//         void AddCallback(Reactant reactant);
//         void RemoveCallback(Reactant reactant);
//         void Invalidate(); //- Only called by users?
//             
//         Convergence ConvergeWith(Factant otherFactor);
//         void        AddIntersection(Factant otherFactor, Convergence convergence);
//     }
// }