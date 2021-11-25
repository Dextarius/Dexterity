// using System;
// using System.Collections.Generic;
// using System.Diagnostics;
// using System.Runtime.InteropServices;
//
// namespace Causality.States
// {
//     public abstract class Factant : ICausalState, IComparable<Factant>
//     {
//         #region Properties
//
//         public             int       Id            { get; }
//         protected abstract Influence Influence     { get; }
//         public    abstract int       VersionNumber { get; }
//
//         #endregion
//
//         
//         #region Instance Methods
//
//         public void NotifyInvolved()
//         {
//             throw new NotImplementedException();
//         }
//
//         public abstract int Stabilize();
//
//         public void AddDependent(Influence influenceToAdd)
//         {
//             if (Influence is null)
//             {
//                 CreateInfluence();
//                 Debug.Assert(Influence != null);
//             }
//             
//             Influence.AddDependent(influenceToAdd);
//         }
//
//         public virtual void AddCallback(Reactant reactant)
//         {
//             if (Influence is null)
//             {
//                 CreateInfluence();
//             }
//             
//             Influence.AddCallback(reactant);
//         }
//
//         public void RemoveCallback(Reactant reactant)
//         {
//             if (Influence != null)
//             {
//                 Influence.RemoveCallback(reactant);
//             }
//             else
//             {
//                 //- Should we throw?
//             }
//         }
//
//         protected abstract void CreateInfluence();
//
//         public void InvalidateDependents()
//         {
//             Influence?.Invalidate(TODO);
//         }
//
//         protected void Accessed()
//         {
//             //- Notify Observer Involved
//             //- If the Observer indicates a dependency was created, we need to create an Influence so
//             //  that they can see if a destabilize call reaches us
//         }
//         
//         protected void StateChanged()
//         {
//             versionNumber++;
//             InvalidateDependents();
//             UpdateList.RunUpdates();
//         }
//         
//         public Convergence ConvergeWith(Factant otherFactor)
//         {
//             if (otherFactor == this)
//             {
//                 throw new InvalidOperationException("Cannot intersect a Factor with itself. ");
//                 //- We could probably just return this if we change the return value.
//             }
//             
//             if (otherFactor.Id < this.Id)
//             {
//                 return otherFactor.ConvergeWith(this);
//             }
//             
//             if (Influence is null)
//             {
//                 CreateInfluence();
//             }
//
//             return Influence.ConvergeWith(this, otherFactor);
//         }
//         
//         public Convergence ConvergeWith(Span<Factant> otherFactors)
//         {
//             if (Influence is null)
//             {
//                 CreateInfluence();
//             }
//
//             if (otherFactors[0] == this)
//             {
//                 throw new InvalidOperationException("Can't intersect w/ self. ");
//                 //- Or we could just Slice it
//             }
//
//             return Influence.ConvergeWith(this, otherFactors);
//         }
//
//         private bool FindConvergence(Factant otherFactor, out Convergence result)
//         {
//             if (otherFactor == this)
//             {
//                 result = this;
//                 return true;
//             }
//             else
//             {
//                 var otherStatesId = otherFactor.Id;
//
//                 if (otherStatesId < this.Id)
//                 {
//                     return otherFactor.FindConvergence(this, out result);
//                 }
//                 else if (Influence == null)
//                 {
//                     result = null;
//                     return false;
//                 }
//                 else
//                 {
//                     return Influence.FindConvergence(otherFactor, out result);
//                 }
//             }
//         }
//
//         // public static int BinarySearch(IIdComparable[] items, int id) => 
//         //     BinarySearchId(items, 0, items.Length, id);
//         //
//         // public static int BinarySearch(IIdComparable[] items, int[] id) => 
//         //     BinarySearchId(items, 0, items.Length, id);
//         //
//         // static int BinarySearchId(IIdComparable[] items, int startIndex, int length, int value)
//         // {
//         //     int start = startIndex;
//         //     int end   = (startIndex + length) - 1;
//         //     
//         //     while (start <= end)
//         //     {
//         //         int mid    = start + ((end - start) / 2);
//         //         int result = items[mid].Compare(value);
//         //         
//         //         if      (result == 0) { return  mid;     }
//         //         else if (result  < 0) { start = mid + 1; }
//         //         else                  { end   = mid - 1; }
//         //     }
//         //     
//         //     return ~start;
//         // }
//         //
//         // static int BinarySearchId(IIdComparable[] items, int startIndex, int length, int[] value)
//         // {
//         //     int start = startIndex;
//         //     int end   = (startIndex + length) - 1;
//         //     
//         //     while (start <= end)
//         //     {
//         //         int mid    = start + ((end - start) / 2);
//         //         int result = items[mid].Compare(value);
//         //         
//         //         if      (result == 0) { return  mid;     }
//         //         else if (result  < 0) { start = mid + 1; }
//         //         else                  { end   = mid - 1; }
//         //     }
//         //     
//         //     return ~start;
//         // }
//         //
//         // static int BinarySearch2Ids(IIdComparable[] items, int startIndex, int length, int otherId)
//         // {
//         //     int start = startIndex;
//         //     int end   = (startIndex + length) - 1;
//         //     
//         //     while (start <= end)
//         //     {
//         //         int mid    = start + ((end - start) / 2);
//         //         int result = items[mid].Compare(value);
//         //         
//         //         if      (result == 0) { return  mid;     }
//         //         else if (result  < 0) { start = mid + 1; }
//         //         else                  { end   = mid - 1; }
//         //     }
//         //     
//         //     return ~start;
//         // }
//
//         #endregion
//     }
//
//     public interface IIdComparable
//     {
//         int        Compare(int   otherId);
//         int        Compare(int[] otherId);
//         bool    IsLessThan(int   otherId);
//         bool IsGreaterThan(int[] otherId);
//     }
// }
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
