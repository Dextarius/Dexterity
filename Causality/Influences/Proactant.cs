// namespace Causality.States
// {
//     public class Proactant : Factant
//     {
//         #region Instance Fields
//
//         protected Influence influence;
//         protected int       versionNumber = 1;
//
//         #endregion
//
//         
//         #region Properties
//
//         protected override Influence Influence     => influence;
//         public    override int       VersionNumber => versionNumber;
//
//         #endregion
//
//
//
//         #region Instance Methods
//
//         public void NotifyInvolved() => Observer.NotifyInvolved(this);
//         
//
//         protected override void CreateInfluence()
//         {
//             influence = new Influence();
//         }
//
//         public override void InvalidateDependents()
//         {
//             influence?.Invalidate(TODO);
//             //^ If these rely only on this Outcome, none of them can have a different Priority can they? 
//         }
//
//         public override int Stabilize() => VersionNumber;
//         
//
//         #endregion
//         
//     }
// }