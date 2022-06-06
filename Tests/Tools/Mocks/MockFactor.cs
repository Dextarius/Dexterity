using System;
using System.Collections.Generic;
using System.Diagnostics;
using Core.Factors;
using Core.States;
using Factors;
using JetBrains.Annotations;

namespace Tests.Tools.Mocks
{
    internal class MockFactor : Factor<IFactorCore>
    {
        
        
        public MockFactor() : base(new MockFactorCore())
        {
            
        }
    }



    // internal class MockFactor : IFactor
    // {
    //     #region Instance Fields
    //
    //     [NotNull, ItemNotNull]
    //     protected HashSet<WeakReference<IFactorSubscriber>> subscribers = new();
    //     protected int numberOfNecessarySubscribers;
    //
    //     #endregion
    //     
    //     public int    UpdatePriority           => 0;
    //     public string Name               => nameof(MockFactor);
    //     public bool   IsNecessary        => numberOfNecessarySubscribers > 0;
    //     public bool   HasSubscribers      => subscribers.Count > 0;
    //     public int    NumberOfSubscribers => subscribers.Count;
    //     
    //     
    //     public virtual void NotifyNecessary()
    //     {
    //         #if DEBUG
    //         Debug.Assert(numberOfNecessarySubscribers >= 0);
    //         Debug.Assert(numberOfNecessarySubscribers < subscribers.Count);
    //         #endif
    //         
    //         numberOfNecessarySubscribers++;
    //     }
    //     
    //     public virtual void NotifyNotNecessary()
    //     {
    //         #if DEBUG
    //         Debug.Assert(numberOfNecessarySubscribers > 0);
    //         Debug.Assert(numberOfNecessarySubscribers <= subscribers.Count);
    //         #endif
    //         
    //         numberOfNecessarySubscribers--;
    //     }
    //     
    //     public virtual bool Subscribe(IFactorSubscriber subscriberToAdd)
    //     {
    //         if (subscriberToAdd == null) { throw new ArgumentNullException(nameof(subscriberToAdd)); }
    //
    //         if (subscribers.Add(subscriberToAdd.WeakReference))
    //         {
    //             if (subscriberToAdd.IsNecessary)
    //             {
    //                 numberOfNecessarySubscribers++;
    //             }
    //
    //             return true;
    //         }
    //
    //         return false;
    //     }
    //
    //     public virtual void Unsubscribe(IFactorSubscriber subscriberToRemove)
    //     {
    //         if (subscriberToRemove != null)
    //         {
    //             if (subscribers.Remove(subscriberToRemove.WeakReference)  &&  
    //                 subscriberToRemove.IsNecessary)
    //             {
    //                 numberOfNecessarySubscribers--;
    //             }
    //         }
    //     }
    //     
    //     public void TriggerSubscribers()
    //     {
    //         var currenSubscribers = subscribers;
    //
    //         if (currenSubscribers.Count > 0)
    //         {
    //             currenSubscribers.RemoveWhere(TriggerSubscriberAndPotentiallyRemoveThem);
    //         }
    //         
    //         //- Note : This doesn't invalidate the dependents in order of UpdatePriority
    //     }
    //     
    //     private bool TriggerSubscriberAndPotentiallyRemoveThem(WeakReference<IFactorSubscriber> subscriberReference)
    //     {
    //         if (subscriberReference.TryGetTarget(out var subscriber))
    //         {
    //             subscriber.Trigger(this, out bool removeSubscription);
    //
    //             if (removeSubscription)
    //             {
    //                 if (subscriber.IsNecessary)
    //                 {
    //                     numberOfNecessarySubscribers--;
    //                 }
    //
    //                 return true;
    //             }
    //             else return false;
    //         }
    //         else return true;
    //     }
    //
    //     public virtual bool Reconcile() => true;
    // }
}