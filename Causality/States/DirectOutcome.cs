// using System;
// using Core.Causality;
//
// namespace Causality.States
// {
//     public class DirectOutcome<TOutcome, TArg, TResult> : State, IOutcome<TResult>
//     {
//         protected IState<TOutcome>        source;
//         protected IProcess<TResult, TArg> process;
//         protected TResult                 currentValue;
//
//
//         public WeakReference<IOutcome> WeakReference             { get; }
//         public bool                    IsStable                  { get; }
//         public bool                    IsBeingAffected           { get; }
//         public bool                    IsUpdating                { get; }
//         public bool                    RecalculatesAutomatically { get; set; }
//         public TResult                 Value { get; }
//
//         public TResult Peek() => currentValue;
//
//         public bool Recalculate()
//         {
//             throw new NotImplementedException();
//         }
//         
//         public void AddInfluence(IState newInfluences)
//         {
//             throw new NotImplementedException();
//         }
//         
//         public OutcomeResponse Invalidate(IState changedParentState)
//         {
//             throw new NotImplementedException();
//         }
//         
//         public OutcomeResponse Destabilize()
//         {
//             throw new NotImplementedException();
//         }
//         
//         #region Constructors
//
//         public DirectOutcome(object ownerToReference) : base(ownerToReference)
//         {
//         }
//
//         #endregion
//     }
// }