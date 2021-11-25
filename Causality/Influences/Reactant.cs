// using System;
// using System.Diagnostics;
//
// namespace Causality.States
// {
//     public abstract class Reactant : Factant
//     {
//         #region Static Fields
//
//         protected static ICausalState noParentState = ;
//
//         #endregion
//         
//         
//         #region Instance Fields
//
//         protected ReactantInfluence influence;
//         private   ICausalState      parentState;
//         private   int               lastVersionChecked;
//         private   int               versionNumber;
//         private   bool              isReflexive;
//
//         #endregion
//
//         
//         #region Properties
//
//         protected override Influence Influence     => influence;
//         public    override int       VersionNumber => versionNumber;
//
//         public bool IsValid    { get; protected set; }
//         public bool IsUpdating { get; protected set; }
//         public int  Priority   { get; protected set; }
//
//         public bool IsReflexive
//         {
//             // get => influence?.HasCallback??  false;
//             // set
//             // {
//             //     bool previousValue = IsReflexive;
//             //
//             //     if (previousValue is true)
//             //     {
//             //         if (value is false)
//             //         {
//             //             influence.RemoveCallback();
//             //         }
//             //     }
//             //     else
//             //     {
//             //         if (value is true)
//             //         {
//             //             if (influence is null)
//             //             {
//             //                 CreateInfluence();
//             //             }
//             //             
//             //             influence.SetCallback(this);
//             //         }
//             //     }
//             // }
//             get => isReflexive;
//             set
//             {
//                 bool previousValue = isReflexive;
//
//                 if (previousValue is true)
//                 {
//                     if (value is false)
//                     {
//                         isReflexive = false;
//                         parentState.RemoveCallback(this);
//                     }
//                 }
//                 else
//                 {
//                     if (value is true)
//                     {
//                         isReflexive = true;
//                         Stabilize(); //- Is this the right order to call these in?
//                         parentState.AddCallback(this);
//                         
//                     }
//                 }
//             }
//         }
//
//         #endregion
//
//
//         #region Instance Methods
//
//         public override int Stabilize()
//         {
//             if (parentState.VersionNumber != lastVersionChecked ||
//                 influence?.IsValid is false)
//             {
//                 if (parentState.Stabilize() != lastVersionChecked)
//                 {
//                     React();
//                 }
//             }
//             
//             return versionNumber;
//         }
//
//         public void React()
//         {
//             //if (influence.IsValid is false)
//             //{
//             
//             Debug.Assert(IsUpdating == false, "A reactor is in an update loop");
//             IsUpdating = true;
//             
//             bool outcomeChanged = Act();
//             
//             IsUpdating = false;
//
//             if (outcomeChanged)
//             {
//                 versionNumber++;
//                 InvalidateDependents();
//                 UpdateList.RunUpdates();
//                 //- Publish/Queue Subscribers
//             }
//             
//             //- Make sure Influence is not invalid, if we use that mechanic.
//
//             //}
//         }
//
//         protected abstract bool Act();
//
//         private bool CheckIfValid()
//         {
//             int currentVersion = parentState.VersionNumber;
//
//             if (currentVersion == 0)
//             {
//                 //?
//             }
//             
//             if (currentVersion == lastVersionChecked)
//             {
//                 return true;
//             }
//             else
//             {
//                 var stabilizedVersion = parentState.Stabilize();
//                 
//                 if (stabilizedVersion == lastVersionChecked)
//                 {
//                     return true;
//                 }
//                 else
//                 {
//                     
//                     return false;
//                 }
//             }
//         }
//         
//         public void Invalidate()
//         {
//             if (IsUpdating)
//             {
//                 throw new InvalidOperationException(
//                     "An Outcome was invalidated while it was updating, meaning either its update process created a loop, " +
//                     "or the Outcome was accessed by two different threads at the same time. \n  " +
//                     $"The invalidated outcome was '{this}' and it was invalidated by '{changedParentState}'. ");
//                 //- If this Outcome is in the update list we should know it's a loop, if it's not then it should be
//                 //  another thread accessing it.  Well actually, the parent won't add us to the list until this returns...
//             }
//
//             if (Influence?.IsNecessary is true)
//             {
//                 //- Recalculate()?
//             }
//             
//             InvalidateDependents();
//         }
//
//         public override void AddCallback(Reactant reactant)
//         {
//             var influenceWasNull = Influence is null;
//             bool wasNecessary = influenceWasNull is false && 
//                                 Influence.IsNecessary is true;
//
//             base.AddCallback(reactant);
//
//             if (wasNecessary is false)
//             {
//                 parentState.AddCallback(this);
//
//                 if (influenceWasNull is false)
//                 {
//                     parentState.RemoveDependent(Influence);
//                     //- If we had an Influence before, then it should have been added as a dependent of our parent,
//                     //  so version changes could propagate.  We no longer want that, since we're going to be recalculating.
//                 }
//             }
//         }
//         
//         
//         
//
//         protected override void CreateInfluence()
//         {
//             influence = new ReactantInfluence();
//         }
//
//         public void NotifyInvalid()
//         {
//             if (React())
//             {
//                 
//             }
//
//         }
//
//         #endregion
//     }
// }