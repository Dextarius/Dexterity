// using System;
//
// namespace Causality.Influences
// {
//     public class Continuum
//     {
//         public enum Continuity { Stable = 0, Unstable = 1, Invalid = 2 }
//     
//         private int[] registeredIDs = new int[1]; 
//         private int   nextAvailableIndex;
//     
//         public int RegisterOutcome(int outcomeId)
//         {
//             int chosenIndex = nextAvailableIndex;
//             
//             if (chosenIndex == registeredIDs.Length)
//             {
//                 ExpandArray();
//             }
//
//             registeredIDs[chosenIndex] = outcomeId;
//             nextAvailableIndex++;
//
//             return chosenIndex;
//         }
//
//         public Continuity CheckOutcome(int slotNumber, int outcomeId)
//         {
//             var currentlyRegisteredIds = registeredIDs;
//             
//             if (slotNumber > currentlyRegisteredIds.Length - 1)
//             {
//                 return Continuity.Invalid;
//             }
//             else
//             {
//                 var valueForSlot = currentlyRegisteredIds[slotNumber];
//
//                 if (valueForSlot == outcomeId)
//                 {
//                     return Continuity.Stable;
//                 }
//                 else if (valueForSlot == -outcomeId) 
//                 {
//                     //- If the number is the negative version of an Id, it indicates the outcome is unstable
//                     return Continuity.Unstable;
//                 }
//                 else
//                 {
//                     return Continuity.Invalid;
//                 }
//             }
//
//             //- What about when there are multiple Outcomes that depend on a single parent?
//             //  Do they each get their own Continuum?
//         }
//
//         private void ExpandArray()
//         {
//             var oldArray = registeredIDs;
//             var newArray = new int[oldArray.Length * 2];
//         
//             Array.Copy(oldArray, newArray, oldArray.Length);
//             registeredIDs = newArray;
//         }
//
//
//         public void Invalidate()
//         {
//             var currentlyRegisteredIds = registeredIDs;
//
//             if (currentlyRegisteredIds.Length > 0)  //- Can this ever be false?
//             {
//                 for (int i = 0; i < currentlyRegisteredIds.Length; i++)
//                 {
//                     ref int currentSlot = ref currentlyRegisteredIds[i];
//
//                     if (currentSlot == 0)
//                     {
//                         break;
//                     }
//                     else
//                     {
//                         currentSlot = 0;
//                     }
//                 }
//                 
//                 nextAvailableIndex = 0;
//             }
//         }
//     
//         public void Destabilize()
//         {
//             var currentlyRegisteredIds = registeredIDs;
//
//             for (int i = 0; i < currentlyRegisteredIds.Length; i++)
//             {
//                 ref int currentSlot = ref currentlyRegisteredIds[i];
//                 int slotValue   = currentSlot;
//
//                 if (slotValue < 0)
//                 {
//                     break;
//                 }
//                 else
//                 {
//                     currentSlot = -slotValue;
//                 }
//             }
//         }
//     }
//     
//
//
//
//     public struct ArraySpan<T>
//     {
//         private T[] array;
//         private int startingIndex;
//         private int elementCount;
//
//         public Span<T> GetSpan()
//         {
//             return new Span<T>(array, startingIndex, startingIndex - elementCount);
//         }
//     }
// }
//
//
//
