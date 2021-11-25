// using System;
// using System.Collections.Generic;
// using Core.Causality;
//
// namespace Causality.States
// {
//     public class Intersection : ICausalNode, IMultiCausalNode
//     {
//         #region Instance Fields
//
//         private Dictionary<ICausalNode, Intersection> childNodesByPartnerState = new Dictionary<ICausalNode, Intersection>();
//         private HashSet<INotifiable>                  callbacks;
//         private ICausalNode                           leftParent;
//         private ICausalNode                           rightParent;
//         private int                                   versionNumber;
//         private ICausalNode[]                         parentIds;   //- We could just do a loop through our parents getting the Id of the right parent. 
//         //- Maybe use a Span<int> for parentIds?
//
//         #endregion
//
//
//         #region Properties
//
//         public int Id { get; }
//
//         #endregion
//
//
//         #region Static Methods
//
//         protected static int GetNewID() => ;
//
//         #endregion
//         
//
//         #region Instance Methods
//
//         public ICausalNode IntersectWith(ICausalNode partnerState)
//         {
//             if (rightParent.Id < partnerState.Id)
//             {
//                 if (childNodesByPartnerState.ContainsKey(partnerState))
//                 {
//                     return childNodesByPartnerState[partnerState];
//                 }
//
//                 Intersection newNode = new Intersection(this, partnerState);
//
//                 childNodesByPartnerState.Add(partnerState, newNode);
//                 
//                 return newNode;
//             }
//             else if (partnerState.Id == rightParent.Id)
//             {
//                 return this;
//             }
//             else
//             {
//                return leftParent.IntersectWith(partnerState);
//             }
//         }
//
//         public Intersection IntersectWith(IMultiCausalNode partnerState)
//         {
//             if (rightParent.Id < partnerState.Id)
//             {
//                 if (childNodesByPartnerState.ContainsKey(partnerState))
//                 {
//                     return childNodesByPartnerState[partnerState];
//                 }
//
//                 Intersection newNode = new Intersection(this, partnerState);
//
//                 childNodesByPartnerState.Add(partnerState, newNode);
//                 
//                 return newNode;
//             }
//             else
//             {
//
//                 //- traverse up parents to find the Intersection for the intersection where
//                 //  partnerState.Id > than the Id for both parents 
//                 //- have that Intersection create a child connecting to partnerState.
//                 //- return that child.
//             }
//         }
//
//
//         public void FindClosestNode(ICausalNode leftState, ICausalNode rightState)
//         {
//             if (leftState == this)
//             {
//                 return this;
//             }
//             else 
//             {
//                 var leftParents = leftState.
//             }
//             
//             
//         }
//
//         #endregion
//         
//
//         #region Constructors
//
//         public Intersection(ICausalNode parent1, ICausalNode parent2)
//         {
//             Id = GetNewID();
//
//             if (parent1.Id < parent2.Id)
//             {
//                 
//             }
//         }
//
//         #endregion
//     }
//
//
//     public class Conjunction : CausalBase, ICausalNode //- Nexus?
//     {
//         private ICausalOrigin leftParent;
//         private ICausalOrigin rightParent;
//         
//         public int Id { get; }
//
//         public ICausalNode IntersectWith(ICausalNode partnerState)
//         {
//             if (partnerState.Id < this.Id)
//             {
//                 if (childNodesByPartnerState.ContainsKey(partnerState))
//                 {
//                     return childNodesByPartnerState[partnerState];
//                 }
//                 else
//                 {
//                     return CreateIntersectionWith(partnerState);
//                 }
//             }
//             else if (partnerState.Id > this.Id)
//             {
//                 return partnerState.IntersectWith(this);
//             }
//             else
//             {
//                 throw new ArgumentException("Node cannot Intersect with itself. ");
//             }
//
//         }
//
//         protected ICausalNode CreateIntersectionWith(ICausalNode partnerState)
//         {
//             Intersection newNode = new Intersection(this, partnerState);
//
//             childNodesByPartnerState.Add(partnerState, newNode);
//                 
//             return newNode;
//         }
//
//         public Conjunction(IState state1, IState state2)
//         {
//             
//         }
//     }
//
//
//     public class CausalBase
//     {
//         protected Dictionary<ICausalNode, IMultiCausalNode> childNodesByPartnerState = new Dictionary<ICausalNode, IMultiCausalNode>();
//
//     }
//     
//     //- Try to pair up existing intersections?  i.e. 1,2,3,4,5 could have 1,2,3 and 4,5 as parents. 
//     //- Can we fanagle it so that Intersections only track Proactives, and clump all the Reactive dependents together?
//
//     public interface ICausalNode
//     {
//         int   Id  { get; }
//         int[] ParentIds { get; }
//
//         bool    AddCallback(INotifiable objectToCall);
//         bool RemoveCallback(INotifiable objectToRemove);
//         ICausalNode IntersectWith(ICausalNode partnerState);
//     }
//     
//     
//     
//     
//     public interface IMultiCausalNode : ICausalNode
//     {
//         int[] ParentIds { get; }
//         
//     }
//
//     
//     
//     
//     public interface ICausalOrigin : ICausalNode
//     {
//         
//     }
// }