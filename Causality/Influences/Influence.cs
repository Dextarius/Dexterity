// using System;
// using System.Collections.Generic;
// using System.Diagnostics;
// using Core.Causality;
// using static Core.Tools.Collections;
//
// namespace Causality.States
// {
//     public class Influence : IInfluence //- Consequences?
//     {
//         #region Instance Fields
//
//         protected Dictionary<Factant, Convergence> convergencesByFactor;
//         protected HashSet<Convergence>             unsortedConvergences;
//         private   HashSet<Influence>               dependentInfluences = new HashSet<Influence>();
//         protected HashSet<Reactant>                necessaryDependents = new HashSet<Reactant>();
//      // private   int                              versionNumber       = 0; // int.Min?
//
//         //- dependentInfluences probably doesn't have to be a Hashset, since each Influence should only
//         //  add itself when it changes to/from being Necessary
//         
//         #endregion
//
//
//         #region Properties
//
//         public bool IsNecessary => reflexiveCallbacks.Count > 0;
//         public bool IsStable    { get; set; }
//
//         // public int  VersionNumber
//         // {
//         //               get => versionNumber;
//         //     protected set => versionNumber = value;
//         // }
//
//         #endregion
//
//         public void AddDependent(Influence influenceToAdd)
//         {
//             dependentInfluences.Add(influenceToAdd);
//         }
//
//         
//         public void AddCallback(Reactant reactant)
//         {
//             reflexiveCallbacks.Add(reactant);
//             
//             //- If a reactant recalculates and no longer uses this state, we're going to need it to remove its callback 
//         }
//         
//         public void RemoveCallback(Reactant reactant)
//         {
//             reflexiveCallbacks.Remove(reactant);
//         }
//
//         public void Invalidate()  //- Only called by users?
//         {
//             if (IsNecessary)
//             {
//                 // Recalculate? | Queue for Update and RunUpdates?
//             }
//         }
//         
//         //- TODO : Make sure we're invalidating the different categories of dependents in the right order.
//         public void Invalidate(int newVersionNumber)   
//         {
//             int oldVersionNumber = VersionNumber;
//             
//             if (newVersionNumber > oldVersionNumber)
//             {
//                 VersionNumber = newVersionNumber;
//                 
//                 if (reflexiveCallbacks != null)
//                 {
//                     foreach (var callback in reflexiveCallbacks)
//                     {
//                         UpdateList.AddOutcome(callback, callback.Priority);
//                     }
//                 }
//
//                 var formerDependents = dependentInfluences;
//                 
//                 if (formerDependents != null)
//                 {
//                     foreach (var dependent in formerDependents)  
//                     {
//                         if (dependent.Destabilize(newVersionNumber))
//                         {
//                             AddCallback(dependent);
//                             UpdateList.AddOutcome(dependent, dependent.Priority);
//                         }
//                     }
//                     
//                     formerDependents.Clear();  //- Does this break the stabilization chain?
//                     //- Maybe not, the highest unstable node will recalculate, and during that reattach itself to this one?
//                 }
//                 
//                 if (convergences != null)
//                 {
//                     foreach (var convergence in convergences)
//                     {
//                         convergence.Invalidate();
//                     }
//                 }
//             }
//         }
//         
//         //- Indicates that at least one of the owners has recalculated, and that their outcome changed when they did.
//         public virtual bool Destabilize(int newVersionNumber)
//         {
//             int oldVersionNumber = VersionNumber;
//
//             if (IsNecessary)
//             {
//                 return true;
//             }
//             else if (newVersionNumber > oldVersionNumber &&
//                      oldVersionNumber > 0)
//             {
//                 var formerDependents = dependentInfluences;
//                 
//                 VersionNumber = -oldVersionNumber; //- Are we using negatives?
//
//                 if (formerDependents != null)
//                 {
//                     foreach (var dependent in formerDependents)
//                     {
//                         if (dependent.Destabilize(newVersionNumber))
//                         {
//                             return true;
//                         }
//                     }
//                 }
//             }
//             
//             return false;
//         }
//
//         public Convergence ConvergeWith(Convergence parentState, Factant otherFactor)
//         {
//             
//         }
//
//         public void AddConvergence(Factant key, Convergence convergence)
//         {
//             
//         }
//
//         public Convergence ConvergeWith(Factant parentState, Factant otherFactor)
//         {
//             Debug.Assert(parentState != otherFactor);
//
//             if (parentState.Id > otherFactor.Id)
//             {
//                 return otherFactor.ConvergeWith(parentState);
//             }
//             else
//             {
//                 return ConvergeWith((ICausalState)parentState, otherFactor);
//             }
//         }
//         
//         protected Convergence ConvergeWith(ICausalState parentState, Factant otherFactor)
//         {
//             Convergence matchingConvergence;
//
//             if (convergencesByFactor is null)
//             {
//                 convergencesByFactor = new Dictionary<Factant, Convergence>();
//                 matchingConvergence  =  CreateConvergence(otherFactor, new[] { parentState, otherFactor });
//             }
//             else if (convergencesByFactor.TryGetValue(otherFactor, out matchingConvergence))
//             {
//                 if (matchingConvergence.Parents.Count > 2)
//                 {
//                     var intermediateConvergence = new Convergence(new[] { parentState, otherFactor });
//
//                     matchingConvergence.ReplacePrimaryParent(, intermediateConvergence);
//                     matchingConvergence = intermediateConvergence;
//                 }
//             }
//             else
//             {
//                 matchingConvergence = CreateConvergence(otherFactor, new[] { parentState, otherFactor });
//             }
//             
//             return matchingConvergence;
//         }
//
//
//         public Convergence ConvergeWith(Span<Factant> factorsToConverge)
//         {
//             Convergence matchingConvergence;
//                 
//             if (factorsToConverge.Length < 1)
//             {
//                 throw new InvalidOperationException();
//             }
//             else
//             {
//                 if (factorsToConverge.Length is 1)
//                 {
//                     matchingConvergence = ConvergeWith(factorsToConverge[0]);
//                 }
//                 else
//                 {
//                     var firstFactor = factorsToConverge[0];
//
//                     if (FindConvergence(firstFactor, out var convergenceMatchingKey))
//                     {
//                         var convergencesAdditionalParents = convergenceMatchingKey.AdditionalParents;
//                         int firstDifferingParent          = 0;
//                         
//                         if (convergencesAdditionalParents != null)
//                         {
//                             var remainingFactorsToMatch = factorsToConverge.Slice(1);
//
//                             matchingConvergence = 
//                                 TryMatchAdditionalParents(
//                                     convergenceMatchingKey, remainingFactorsToMatch, convergencesAdditionalParents);
//
//                         }
//                         else
//                         {
//                             return convergenceMatchingKey.ConvergeWith(parentState, factorsToConverge.Slice(1));
//                         }
//                     }
//                     else
//                     {
//                         var intersectingFactors = new ICausalState[factorsToConverge.Length + 1];
//
//                         intersectingFactors[0] = parentState;
//
//                         for (int i = 1; i < intersectingFactors.Length; i++)
//                         {
//                             intersectingFactors[i] = factorsToConverge[i];
//                         }
//                     
//                         return convergenceMatchingKey;
//                     }
//                 }
//             }
//         }
//
//         public Convergence TryMatchAdditionalParents(Convergence convergenceWithParents,
//             Span<Factant> factorsToMatch, IReadOnlyList<ICausalState> convergencesAdditionalParents)
//         {
//
//             int  firstDifferingParent                   = 0;
//             bool additionalParentsMatchRequestedFactors = true;
//
//             for (int i = 0; i < convergencesAdditionalParents.Count  &&  i < factorsToMatch.Length; i++)
//             {
//                 if (convergencesAdditionalParents[i] != factorsToMatch[i])
//                 {
//                     additionalParentsMatchRequestedFactors = false;
//                     firstDifferingParent = i;
//                     break;
//                 }
//             }
//
//             if (convergencesAdditionalParents.Count < factorsToMatch.Length)
//             {
//                 if (firstDifferingParent == convergencesAdditionalParents.Count)
//                 {
//                     return convergenceWithParents;
//                 }
//                 else
//                 {
//                     var unsharedAdditionalParents = factorsToMatch.Slice(firstDifferingParent);
//                     var intermediate              = convergenceWithParents.ExtractIntermediateConvergence(firstDifferingParent);
//
//                     //- Replace convergenceMatchingKey with intermediate in Dictionary 
//                     return intermediate.ConvergeWith(intermediate, unsharedAdditionalParents);
//                 }
//             }
//
//             if (firstDifferingParent > factorsToMatch.Length)
//             {
//                 if (factorsToMatch.Length == convergencesAdditionalParents.Count)
//                 {
//                     matchingConvergence = convergenceMatchingKey;
//                 }
//                 else
//                 {
//
//                 }
//             }
//
//             if (additionalParentsMatchRequestedFactors)
//             {
//                 if (convergencesAdditionalParents.Count < factorsToMatch.Length)
//                 {
//                     var unsharedAdditionalParents = factorsToMatch.Slice(firstDifferingParent);
//
//                     matchingConvergence = convergenceMatchingKey.ConvergeWith(factorsToMatch);
//                 }
//                 else if (convergencesAdditionalParents.Count == factorsToMatch.Length)
//                 {
//                     matchingConvergence = convergenceMatchingKey;
//                 }
//                 else
//                 {
//
//                 }
//             }
//             else
//             {
//                 var unsharedAdditionalParents = factorsToMatch.Slice(firstDifferingParent);
//                 var intermediate              = convergenceMatchingKey.ExtractIntermediateConvergence(firstDifferingParent);
//
//                 //- Replace convergenceMatchingKey with intermediate in Dictionary 
//                 matchingConvergence = intermediate.ConvergeWith(intermediate, unsharedAdditionalParents);
//             }
//
//         }
//
//         private int FindFirstDifferingParent(
//             Span<Factant> factorsToMatch, IReadOnlyList<ICausalState> convergencesAdditionalParents)
//         {
//             for (int i = 0; i < convergencesAdditionalParents.Count  &&  i < factorsToMatch.Length; i++)
//             {
//                 if (convergencesAdditionalParents[i] != factorsToMatch[i])
//                 {
//                     return i;
//                 }
//             }
//             
//             return 
//         }
//
//
//
//         protected Convergence SplitConvergence(
//             Convergence convergenceToSplit, int numberOfOtherParentsToExtract, Span<Factant> parentsToExtract)
//         {
//             var uncommonFactors    = otherFactors.Slice(firstDifferingParent, otherFactors.Length - firstDifferingParent);
//             var intermediate       = convergenceContainingFactor.ExtractIntermediateConvergence(commonFactors);
//             var createdConvergence = intermediate.ConvergeWith(uncommonFactors);
//
//             if (numberOfOtherParentsToExtract == 0)
//             {
//                 intermediate = convergenceToSplit.ExtractIntermediateConvergence();
//             }
//             else
//             {
//                 var commonFactors = parentsToExtract.Slice(0, numberOfOtherParentsToExtract);
//
//                 intermediate = convergenceToSplit.ExtractIntermediateConvergence(commonFactors);
//             }
//
//         }
//
//         protected Convergence ExtractCommonParentsIntoConvergence(Convergence convergenceToSplit, Span<Factant> commonFactors)
//         {
//             var intermediate = convergenceToSplit.ExtractIntermediateConvergence(commonFactors);
//         }
//         
//
//         protected Convergence Split(Span<Factant> allParents, int firstDifferingParent)
//         {
//             var commonFactors      = allParents.Slice(0,                    firstDifferingParent);
//             var uncommonFactors    = allParents.Slice(firstDifferingParent, allParents.Length - firstDifferingParent);
//             var intermediate       = .ExtractIntermediateConvergence(commonFactors);
//             var createdConvergence = intermediate.ConvergeWith(uncommonFactors);
//
//             parentsMatch = false;
//             break;
//         }
//
//         protected Convergence CreateConvergence(Factant key, ICausalState[] otherFactors)
//         {
//             var convergence = new Convergence(otherFactors);
//             
//             convergence.Initialize();
//             convergencesByFactor[key] = convergence;
//             
//
//             return convergence;
//         }
//         
//         public virtual bool FindConvergence(Factant otherFactor, out Convergence result)
//         {
//             if (convergencesByFactor is null)
//             {
//                 result = null;
//                 return false;
//             }
//             else if(convergencesByFactor.TryGetValue(otherFactor, out var match))
//             {
//                 result = match;
//                 return true;
//             }
//             else
//             {
//                 result = null;
//                 return false;
//             }
//         }
//
//         public void AddConvergence(Convergence convergenceToAdd, Factant key)
//         {
//             if (convergenceToAdd is null)
//             {
//                 convergencesByFactor = new Dictionary<Factant, Convergence>();
//             }
//             
//             convergencesByFactor[key] = convergenceToAdd;
//         }
//
//         public void AddConvergence(Convergence convergenceToAdd, Convergence primaryParent)
//         {
//             
//         }
//         
//         
//         
//         public void ResetVersionNumber()
//         {
//             //- We need some way to reset the version number when it reaches the max value.
//             //- Also, are we using negatives?
//         }
//     }
// }