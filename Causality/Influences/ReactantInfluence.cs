// namespace Causality.States
// {
//     public class ReactantInfluence : Influence
//     {
//         #region Instance Fields
//
//         private int versionNumber = 0;
//
//         #endregion
//
//         
//         #region Properties
//
//         //public bool HasCallback => owner != null;
//         public bool IsValid      { get; protected set; }
//         public bool IsStablizing { get; set; }
//
//         
//         public int VersionNumber
//         {
//             get
//             {
//                 //if (versionSource != null)
//                 //{
//                 //    return versionSource.VersionNumber;
//                 //}
//                 //else
//                 {
//                     return versionNumber;
//                 }
//             }
//             
//             protected set => versionNumber = value;
//         }
//
//         //public void    SetCallback(Reactant reactant) => owner = reactant;
//         //public void RemoveCallback()                  => owner = null;
//
//         #endregion
//         
//
//         #region Instance Methods
//
//         public override void Invalidate()
//         {
//             VersionNumber++;
//             base.Invalidate(TODO);
//             //owner?.NotifyInvalid();
//         }
//
//         public void UpdateVersionSource(ICausalState source)
//         {
//             versionSource = source;
//         }
//         
//
//         public override bool Destabilize(int newVersionNumber)
//         {
//             if (IsStablizing) //- Do we need this? Does it make sense now that Unstable means having an old version number?
//             {
//                 return false;
//             }
//             else
//             {
//                 return base.Destabilize(newVersionNumber);
//             }
//         }
//
//         #endregion
//     }
// }