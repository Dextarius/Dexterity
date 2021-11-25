// using System;
// using System.Collections.Generic;
// using System.Diagnostics;
// using Core.Causality;
// using JetBrains.Annotations;
//
// namespace Causality.States
// {
//     // public class Convergence : INexus  //- Convergence?
//     // {
//     //     #region Instance Fields
//     //
//     //     private          HashSet<IOutcome> necessaryDependents;
//     //     private          INexus            leftParent;
//     //     private          INexus            rightParent;
//     //     private readonly int               hashcode;
//     //     private          int               numberOfAffectedOutcomes;
//     //
//     //     #endregion
//     //
//     //
//     //     #region Properties
//     //
//     //     public int[] Id            { get; }
//     //     public int   VersionNumber { get; protected set; } = 1;
//     //     public bool  IsNecessary   => HasNecessaryDependents();
//     //     public bool  IsUnstable    => VersionNumber < 0;
//     //
//     //     //- If both of an Convergence's parents are regular States, we could make the Version number a long consisting of 
//     //     //  both of the version numbers belonging to its parents.
//     //     #endregion
//     //
//     //
//     //     #region Instance Methods
//     //
//     //     
//     //     private bool HasNecessaryDependents()
//     //     {
//     //         if (necessaryDependents is null)
//     //         {
//     //             return false;
//     //         }
//     //         else
//     //         {
//     //             return necessaryDependents.Count > 0;
//     //         }
//     //     }
//     //
//     //     public void NotifyNecessary(IOutcome necessaryOutcome)
//     //     {
//     //         if (necessaryDependents is null)
//     //         {
//     //             necessaryDependents = new HashSet<IOutcome>();
//     //         }
//     //
//     //         necessaryDependents.Add(necessaryOutcome);
//     //     }
//     //     
//     //     public void NotifyNotNecessary(IOutcome unnecessaryOutcome)
//     //     {
//     //         necessaryDependents?.Remove(unnecessaryOutcome);
//     //     }
//     //
//     //     private int HashIdNumbers()
//     //     {
//     //         //- Make a hash of all of the numbers in the Id so we can compare IDs and quickly
//     //         //  identify if they are not a match.
//     //     }
//     //
//     //     public void Invalidate()
//     //     {
//     //         if (VersionNumber < 0)
//     //         {
//     //             int currentVersion = VersionNumber;
//     //
//     //             VersionNumber = -currentVersion + 1;
//     //         }
//     //         
//     //         VersionNumber++;
//     //     }
//     //
//     //     public virtual bool Stabilize()
//     //     {
//     //         bool wasStabilized = true;
//     //
//     //         if (leftParent.IsUnstable)
//     //         {
//     //             if (leftParent.Stabilize() is false)
//     //             {
//     //                 wasStabilized = false;
//     //             }
//     //         }
//     //
//     //         if (rightParent.IsUnstable)
//     //         {
//     //             if (rightParent.Stabilize() is false)
//     //             {
//     //                 wasStabilized = false;
//     //             }
//     //         }
//     //
//     //         if (wasStabilized is false)
//     //         {
//     //             //- Invalidate?
//     //         }
//     //
//     //         return wasStabilized;
//     //     }
//     //     
//     //     public void Destabilize()
//     //     {
//     //         VersionNumber = -VersionNumber;
//     //     }
//     //     
//     //     public void InvalidateDependents()
//     //     {
//     //         var formerDependents = necessaryDependents;
//     //
//     //         foreach (var outcome in formerDependents)
//     //         {
//     //             outcome.Invalidate();
//     //             UpdateList.AddOutcome(outcome, outcome.Priority);
//     //         }
//     //     }
//     //
//     //     protected void ReplaceLeftParent()
//     //     {
//     //         
//     //     }
//     //
//     //     protected void ReplaceRightParent()
//     //     {
//     //         
//     //     }
//     //     
//     //     #endregion
//     //
//     //     public Convergence(int[] parentsIds, INexus leftParent, INexus rightParent)
//     //     {
//     //         Id               = parentsIds;
//     //         this.hashcode    = HashIdNumbers();
//     //         this.leftParent  = leftParent;
//     //         this.rightParent = rightParent;
//     //     }
//     // }
//
//
//     public abstract class Convergence : Influence, IConvergence //- Entanglement, Implication, Interaction, Intersection?
//     {
//         #region Properties
//
//                   public virtual  IReadOnlyList<ICausalState> AdditionalParents => null;
//         [NotNull] public abstract ICausalState                LeftParent   { get; }
//         [NotNull] public abstract Factant                     RightParent  { get; }
//
//         public bool  IsUnstable => VersionNumber < 0;
//         public int   VersionNumber { get; protected set; } = 1;
//
//
//         //- If both of an Convergence's parents are regular States, we could make the Version number a long consisting of 
//         //  both of the version numbers belonging to its parents.
//
//         #endregion
//
//
//         #region Instance Methods
//
//         
//         private bool HasNecessaryDependents()
//         {
//             if (necessaryDependents is null) { return false; }
//             else                             { return necessaryDependents.Count > 0; }
//         }
//
//         public void NotifyNecessary(Reactant necessaryOutcome)
//         {
//             if (necessaryDependents is null)
//             {
//                 necessaryDependents = new HashSet<Reactant>();
//             }
//
//             necessaryDependents.Add(necessaryOutcome);
//         }
//         
//         public void NotifyNotNecessary(Reactant unnecessaryOutcome)
//         {
//             necessaryDependents?.Remove(unnecessaryOutcome);
//         }
//
//         public void Invalidate()
//         {
//             if (VersionNumber < 0)
//             {
//                 int currentVersion = VersionNumber;
//
//                 VersionNumber = -currentVersion + 1;
//             }
//             
//             VersionNumber++;
//         }
//
//         public virtual bool Stabilize()
//         {
//             bool wasStabilized = true;
//
//             if (leftParent.IsUnstable)
//             {
//                 if (leftParent.Stabilize() is false)
//                 {
//                     wasStabilized = false;
//                 }
//             }
//
//             if (rightParent.IsUnstable)
//             {
//                 if (rightParent.Stabilize() is false)
//                 {
//                     wasStabilized = false;
//                 }
//             }
//
//             if (wasStabilized is false)
//             {
//                 //- Invalidate?
//             }
//
//             return wasStabilized;
//         }
//         
//         public void Destabilize()
//         {
//             VersionNumber = -VersionNumber;
//         }
//         
//         public void InvalidateDependents()
//         {
//             var formerDependents = necessaryDependents;
//
//             foreach (var convergence in convergences)
//             {
//                 convergence.Invalidate();
//             }
//
//             foreach (var continuum in continua)
//             {
//                 continuum.Invalidate();
//             }
//             
//             foreach (var outcome in formerDependents)
//             {
//                 outcome.Invalidate();
//                 UpdateList.AddOutcome(outcome, outcome.Priority);
//             }
//         }
//         
//         int ICausalState.Stabilize()
//         {
//             throw new NotImplementedException();
//         }
//         public void RemoveDependent(Influence influence)
//         {
//             throw new NotImplementedException();
//         }
//
//         public abstract void Initialize();
//         
//
//         #endregion
//
//         
//         #region Constructors
//
//         public Convergence([NotNull] ICausalState leftParent, Factant rightParent)
//         {
//             LeftParent  = leftParent;
//             RightParent = rightParent;
//         }
//
//         #endregion
//
//         public abstract Convergence ExtractIntermediateConvergence(int numberOfAdditionalParentsToExtract);
//         public abstract Convergence ExtractIntermediateConvergence();
//     }
//
//     
//     
//     class BinaryConvergence : Convergence
//     {
//         #region Instance Fields
//
//         private Factant rightParent;
//         private Factant leftParent;
//
//         #endregion
//
//         #region Properties
//
//         public override ICausalState LeftParent  => leftParent;
//         public override Factant      RightParent => rightParent;
//
//         #endregion
//
//
//         #region Instance Methods
//
//         public override void Initialize()
//         {
//             leftParent.AddConvergence(rightParent);
//             rightParent.AddConvergence(leftParent);
//         }
//         
//         public override Convergence ExtractIntermediateConvergence(Span<Factant> otherStatesToExtract) =>
//             throw new InvalidOperationException("Can't replace if the Convergence only has 2 parents. ");
//
//
//         #endregion
//         
//         
//         #region Constructors
//
//         public BinaryConvergence([NotNull] ICausalState leftParent, Factant rightParent) : base(leftParent, rightParent)
//         {
//         }
//
//         #endregion
//     }
//
//
//     
//     class MultiConvergence : Convergence
//     {
//         #region Instance Fields
//
//         private ICausalState leftParent;
//         private Factant      rightParent;
//         private Factant[]    additionalParents;
//
//         #endregion
//
//
//         #region Properties
//
//         public override IReadOnlyList<ICausalState> AdditionalParents => additionalParents;
//         public override ICausalState                LeftParent   => leftParent;
//         public override Factant                     RightParent  => rightParent;
//
//         #endregion
//
//
//         #region Instance Methods
//         
//         public override Convergence ExtractIntermediateConvergence() => ExtractIntermediateConvergence(0);
//         
//         public override Convergence ExtractIntermediateConvergence(int numberOfAdditionalParentsToExtract)
//         {
//             var former_AdditionalParents = additionalParents;
//
//             if (former_AdditionalParents is null)
//             {
//                 throw new InvalidOperationException("Can't replace if the Convergence only has 2 parents. ");
//             }
//             else
//             {
//                 Convergence intermediateConvergence;
//                 int         originalNumberOfAdditionalParents = former_AdditionalParents.Length;
//
//                 if (numberOfAdditionalParentsToExtract is 0)
//                 {
//                     intermediateConvergence = new Convergence(LeftParent, RightParent);
//                     this.rightParent = former_AdditionalParents[0];
//                 }
//                 else if (numberOfAdditionalParentsToExtract >= former_AdditionalParents.Length)
//                 {
//                     throw new ArgumentException();
//                     //- Can't take all of our parents
//                 }
//                 else
//                 {
//                     var extractedAdditionalParents = new Factant[numberOfAdditionalParentsToExtract];
//
//                     for (int i = 0; i < numberOfAdditionalParentsToExtract; i++)
//                     {
//                         extractedAdditionalParents[i] = former_AdditionalParents[i];
//                     }
//                     
//                     intermediateConvergence = new MultiConvergence(LeftParent, RightParent, extractedAdditionalParents);
//                     this.rightParent = former_AdditionalParents[numberOfAdditionalParentsToExtract];
//                 }
//
//                 this.leftParent  = intermediateConvergence;
//                 TrimAdditionalParents(former_AdditionalParents, numberOfAdditionalParentsToExtract);
//                 //- intermediateConvergence.Initialize();
//
//                 return intermediateConvergence;
//             }
//         }
//         
//         protected void TrimAdditionalParents(Factant[] oldOtherParents, int numberOfFactorsToRemove)
//         {
//             int originalNumberOfOtherParents = oldOtherParents.Length;
//             var numberOfOtherParentsLeft     = originalNumberOfOtherParents - numberOfFactorsToRemove;
//             
//             if (numberOfOtherParentsLeft > 0)
//             {
//                 var newOtherParents = new Factant[numberOfOtherParentsLeft];
//                     
//                 Array.Copy(oldOtherParents, numberOfFactorsToRemove, newOtherParents, 0, numberOfOtherParentsLeft);
//                 additionalParents = newOtherParents;
//             }
//             else
//             {
//                 additionalParents = null;
//             }
//         }
//         
//         #endregion
//
//
//         #region Constructors
//
//         public MultiConvergence([NotNull] ICausalState leftParent, Factant rightParent, Factant[] additionalParents) : 
//             base(leftParent, rightParent)
//         {
//             this.additionalParents = additionalParents;
//         }
//
//         #endregion
//     }
//
//     public class ConvergenceManager
//     {
//         private INexus          owner;
//         private IConvergence[] convergences;
//         private int             centerIndex;
//
//         public void AddConvergenceWith(INexus otherState)
//         {
//
//         }
//
//         public IConvergence FindConvergenceWith(INexus otherState)
//         {
//             if (otherState == owner)
//             {
//                 //- return owner?
//                 //- throw?
//             }
//             
//             int startingIndex;
//             int endingIndex;
//             int otherStatesId = otherState.Id;
//
//             if (otherStatesId < owner.Id)
//             {
//                 return otherState.FindConvergenceWith(owner);
//                 // startingIndex = 0;
//                 // endingIndex = centerIndex;
//             }
//             else
//             {
//                 startingIndex = centerIndex;
//                 endingIndex   = convergences.Length;
//                 
//                 for (int i = startingIndex; i < endingIndex; i++)
//                 {
//                     var currentConvergence = convergences[i];
//                     var convergencesParents = currentConvergence.Path;
//
//                     for (int j = 0; j < convergencesParents.Length; j++)
//                     {
//                         
//                     }
//                 }
//             }
//             
//             
//
//         }
//
//         public void StoreConvergence(IConvergence convergence)
//         {
//             int ownerIndex = -1;
//
//             for (int i = 0; i < convergence.Path.Length; i++)
//             {
//                 if (convergence.Path[i] == owner.Id)
//                 {
//                     ownerIndex = i;
//                     break;
//                 }
//             }
//
//             Span<int> relevantPath = new Span<int>(convergence.Path, ownerIndex, convergence.Path.Length - ownerIndex);
//
//
//             for (int i = centerIndex; i < convergences.Length; i++)
//             {
//
//             }
//         }
//
//         public int ComparePaths(Span<int> first, Span<int> second)
//         {
//             for (int i = 0; i < first.Length; i++)
//             {
//                 if (first[i] > second[i])
//                 {
//                     return -1;
//                 }
//                 else if (second[i] > first[i])
//                 {
//                     return 1;
//                 }
//             }
//
//             return 0;
//         }
//
//     }
// }
