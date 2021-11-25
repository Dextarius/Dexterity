// using System;
// using Core.Causality;
// using Core.Factors;
//
// namespace Causality.States
// {
//     public class InvalidOutcome : IOutcome
//     {
//         #region Instance Fields
//
//         //- It almost seems a waste to have this, but not having it has led to bugs with classes expecting to
//         //  be able to set a callback and have HasCallback be true, so we might as well implement it properly
//         //  until we find a better solution.
//         protected WeakReference<INotifiable> callbackReference;
//
//         #endregion
//         
//         #region Properties
//
//         public bool IsBeingAffected => false;
//         public bool IsConsequential => false;
//         public bool IsInvalid       => true;
//         public bool IsValid         => false;
//         public bool HasCallback     => callbackReference != null;
//
//         #endregion
//
//
//         #region Instance Methods
//
//         public void NotifyInvolved() => Observer.NotifyInvolved(this);
//
//         public bool Invalidate() => false;
//         public OutcomeResponse Invalidate(IState invalidParentState) => OutcomeResponse.None;
//         public void InvalidateDependents() {  }
//         public void ReleaseDependent(IOutcome dependentOutcome) { }
//
//         public void SetInfluences(IState[] newInfluences)
//         {
//             //- I don't intend for this method to be used, but who knows if I'll remember.
//             //  Might as well make it behave appropriately.
//             foreach (var influence in newInfluences)
//             {
//                 influence.ReleaseDependent(this);
//             }
//         }
//
//         public bool AddDependent(IOutcome dependentOutcome)
//         {
//             //- TODO : This code is replicated in section where CasualEvent tries to add dependencies.
//             if (dependentOutcome.IsValid)
//             {
//                 dependentOutcome.Invalidate(this);
//             }
//
//             return false;
//         }
//         
//         public void SetCallback(INotifiable objectToNotify)
//         {
//             callbackReference = new WeakReference<INotifiable>(objectToNotify);  
//             objectToNotify.Notify();
//             
//             //- TODO : Consider if immediately calling the object being notified is what most people would expect to
//             //         happen when they set a callback to an already invalid outcome?
//         }
//
//         public void DisableCallback() => callbackReference = null;
//
//         #endregion
//     }
//
//     public class InvalidOutcome<T> : InvalidOutcome, IOutcome<T>
//     {
//         public T Value { get; set; } = default(T);
//         public T Peek() => Value;
//     }
//
// }