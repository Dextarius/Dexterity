// using Core.Factors;
// using Core.States;
//
// namespace Factors
// {
//     public class FactorShell<TFactor> : IFactor
//         where TFactor : IFactor
//     {
//         protected TFactor factor;
//
//         
//         public string Name           => factor?.Name                ??
//         public int    UpdatePriority => factor?.UpdatePriority      ?? 0;
//         public uint   VersionNumber  => factor?.VersionNumber       ?? 0;
//
//         public bool Subscribe(IFactorSubscriber subscriberToAdd, bool isNecessary)
//         {
//             throw new System.NotImplementedException();
//         }
//
//         public void Unsubscribe(IFactorSubscriber subscriberToRemove)
//         {
//             throw new System.NotImplementedException();
//         }
//
//         public void NotifyNecessary(IFactorSubscriber necessarySubscriber)
//         {
//             throw new System.NotImplementedException();
//         }
//
//         public void NotifyNotNecessary(IFactorSubscriber unnecessarySubscriber)
//         {
//             throw new System.NotImplementedException();
//         }
//
//         public bool Reconcile()
//         {
//             throw new System.NotImplementedException();
//         }
//     }
//     
//     public class ReactorShell<TReactor> : IReactor
//         where TReactor : IReactor
//     {
//         protected TReactor reactor;
//         
//         public string Name                => reactor?.Name                ??
//         public int    UpdatePriority      => reactor?.UpdatePriority      ?? 0;
//         public uint   VersionNumber       => reactor?.VersionNumber       ?? 0;
//         public bool   IsNecessary         => reactor?.IsNecessary         ?? false;
//         public bool   HasSubscribers      => reactor?.HasSubscribers      ?? false;
//         public int    NumberOfSubscribers => reactor?.NumberOfSubscribers ?? 0;
//         public bool   IsUnstable          => reactor?.IsUnstable          ?? false;
//         public bool   IsReacting          => reactor?.IsReacting          ?? false;
//         public bool   IsStabilizing       => reactor?.IsStabilizing       ?? false;
//         public bool   HasReacted          => reactor?.HasReacted          ?? false;
//         public bool   HasTriggers         => reactor?.HasTriggers         ?? false;
//         public bool   IsTriggered         => reactor?.IsTriggered         ?? false;
//         public int    NumberOfTriggers    => reactor?.NumberOfTriggers    ?? 0;
//         public bool   IsReflexive         { get; set; }
//         public bool   AutomaticallyReacts { get; set; }
//
//         public bool Subscribe(IFactorSubscriber subscriberToAdd, bool isNecessary)
//         {
//             throw new System.NotImplementedException();
//         }
//         
//         public void Unsubscribe(IFactorSubscriber subscriberToRemove)
//         {
//             throw new System.NotImplementedException();
//         }
//
//         public void NotifyNecessary(IFactorSubscriber necessarySubscriber)
//         {
//             throw new System.NotImplementedException();
//         }
//
//         public void NotifyNotNecessary(IFactorSubscriber unnecessarySubscriber)
//         {
//             throw new System.NotImplementedException();
//         }
//
//         public bool Reconcile()
//         {
//             throw new System.NotImplementedException();
//         }
//
//
//
//         public void TriggerSubscribers()
//         {
//             throw new System.NotImplementedException();
//         }
//         public bool Trigger()
//         {
//             throw new System.NotImplementedException();
//         }
//         public bool Trigger(IFactor triggeringFactor, out bool removeSubscription)
//         {
//             throw new System.NotImplementedException();
//         }
//         public bool Destabilize()
//         {
//             throw new System.NotImplementedException();
//         }
//
//         public bool AttemptReaction()
//         {
//             throw new System.NotImplementedException();
//         }
//         public bool ForceReaction()
//         {
//             throw new System.NotImplementedException();
//         }
//     }
//     
//     public class ReactiveShell<T> : ReactorShell<Reactive<T>>, IReactive<T>
//     {
//         private T value;
//         private T
//     }
// }