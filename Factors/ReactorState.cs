using System;
using System.Collections.Generic;
using System.Diagnostics;
using Core;
using Core.Factors;
using Core.States;
using Dextarius.Collections;

namespace Factors
{
    // public class ReactorState : IFactorSubscriber
    // {
    //     #region Static Fields
    //
    //     [ThreadStatic]
    //     private static UpdateList updateList;
    //     
    //     #endregion
    //     
    //     
    //     #region Instance Fields
    //     
    //     protected readonly HashSet<IFactorSubscriber>       allSubscribers       = new HashSet<IFactorSubscriber>();
    //     protected readonly HashSet<IFactorSubscriber>       necessarySubscribers = new HashSet<IFactorSubscriber>();
    //     private   readonly IFactorSubscriber                subscriber;
    //     private   readonly WeakReference<IFactorSubscriber> weakReferenceToSubscriber;
    //     private            int                              interactionCount;
    //     
    //     #endregion
    //     
    //     
    //     #region Static Properties
    //
    //     public static UpdateList UpdateList => ;
    //
    //     #endregion
    //
    // //  public virtual IInfluenceOwner Owner                   { get; }
    //     public         bool            IsTriggered             { get; set; }
    //     public         bool            IsUnstable              { get; set; }
    //     public         bool            IsNecessary             { get; set; }
    //     public         bool            CallBackOnTriggered     { get; set; }
    //     public virtual bool            HasNecessarySubscribers => necessarySubscribers.Count > 0;
    //     public         int             NumberOfSubscribers     => allSubscribers.Count;
    //
    //
    //     public bool Trigger() => Trigger(null, out _);
    //
    //     public bool Trigger(IFactor triggeringFactor, out bool removeSubscription)
    //     {
    //         removeSubscription = false; //- Remove this
    //         
    //         if (IsReacting)
    //         {
    //            // Logging.Notify_ReactorTriggeredWhileUpdating(this, triggeringFactor);
    //
    //             //- If this Outcome is in the update list we should know it's a loop,
    //             //  if it's not then it should be another thread accessing it.
    //             //  Well actually, the parent won't add us to the list until this returns...
    //             //  Don't we add ourselves now?
    //         }
    //         
    //         if (IsTriggered is false)
    //         {
    //             IsTriggered = true;
    //             InvalidateOutcome(triggeringFactor);
    //             Debug.Assert(IsQueued is false);
    //             
    //             if (CallBackOnTriggered)
    //             {
    //                 Callback.ReactorTriggered(this);
    //             }
    //
    //             return true;
    //         }
    //
    //         return false;
    //     }
    //     
    //     public bool Destabilize()
    //     {
    //         if (IsNecessary || HasNecessarySubscribers)
    //         {
    //             return true;
    //             
    //             //- We shouldn't need to mark ourselves as Unstable. If we're Necessary then either
    //             //  we're going to be triggered when our parent updates, or the parent won't
    //             //  change, in which case we aren't Unstable.
    //         }
    //         else if (IsUnstable is false)
    //         {
    //             bool hasNecessaryDependents = DestabilizeSubscribers();
    //
    //             return hasNecessaryDependents;
    //         }
    //
    //         return false;
    //         
    //     //- Note : This method used to check IsStabilizing and IsTriggered, but I removed that because it
    //     //         it no longer seemed relevant.  When we have time, go through all the possible ways this might
    //     //         called and make sure I'm not missing something.
    //     }
    //     
    //     public virtual bool Subscribe(IFactorSubscriber subscriberToAdd, bool isNecessary)
    //     {
    //         if (subscriberToAdd == null) { throw new ArgumentNullException(nameof(subscriberToAdd)); }
    //
    //         if (allSubscribers.Add(subscriberToAdd))
    //         {
    //             if (isNecessary)
    //             {
    //                 AddSubscriberAsNecessary(subscriberToAdd);
    //             }
    //
    //             return true;
    //         }
    //         else return false;
    //     }
    //
    //     public virtual bool Unsubscribe(IFactorSubscriber subscriberToRemove)
    //     {
    //         if (subscriberToRemove != null)
    //         {
    //             bool wasRemoved = allSubscribers.Remove(subscriberToRemove);
    //             
    //             if (wasRemoved)
    //             {
    //                 RemoveSubscriberFromNecessary(subscriberToRemove);
    //             }
    //
    //             return wasRemoved;
    //         }
    //         else return false;
    //     }
    //     
    //     public void TriggerSubscribers(IFactor triggeringFactor)
    //     {
    //         var formerSubscribers = allSubscribers;
    //             
    //         if (formerSubscribers.Count > 0)
    //         {
    //             using (UpdateList.QueueUpdates())
    //             {
    //                 formerSubscribers.RemoveWhere(TriggerSubscriberAndPotentiallyRemoveThem);
    //             }
    //         }
    //         //^ TODO : We might be able to skip establishing the UpdateQueue if there's only 1 subscriber.
    //         //         If this factor was triggered by another one, the subscriber will get picked up by their
    //         //         queue anyways.
    //         
    //
    //         bool TriggerSubscriberAndPotentiallyRemoveThem(IFactorSubscriber subscriber)
    //         {
    //             subscriber.Trigger(triggeringFactor, out bool removeSubscription);
    //
    //             if (removeSubscription)
    //             {
    //                 //- Working On : Do we want to use RemoveSubscriberFromNecessary() here?
    //                 //  We probably should, but that means we're going to check if we need
    //                 //  to notify that we're no longer necessary every time we remove one, instead of just
    //                 //  checking when we're done triggering all of them.
    //
    //                 RemoveSubscriberFromNecessary(subscriber);
    //                 return true;
    //             }
    //             else return false;
    //         }
    //     }
    //     
    //     public bool DestabilizeSubscribers()
    //     {
    //         var subscribersToDestabilize = allSubscribers;
    //
    //         if (subscribersToDestabilize.Count > 0)
    //         {
    //             foreach (var subscriber in subscribersToDestabilize)
    //             {
    //                 if (subscriber.Destabilize())
    //                 {
    //                     AddSubscriberAsNecessary(subscriber);  
    //                     //^ What happens if this makes us Necessary?
    //                     //  If we're owned by a Reflexive Reactor will it start updating in
    //                     //  the middle of us destabilizing these subscribers?
    //                     return true;
    //                 }
    //             }
    //
    //             //- TODO : We specifically tried to avoid using foreach before when triggering subscribers because they might
    //             //         choose to remove themselves, so consider if we really want to use it here.  
    //             //-        Well we don't really want to use the RemoveWhere() method like we did in TriggerSubscribers()
    //             //         because we want to end this method as soon as we find a necessary subscriber.
    //         }
    //         
    //         return false;
    //     }
    //     
    //     public virtual void NotifyNecessary(IFactorSubscriber necessarySubscriber)
    //     {
    //         if (allSubscribers.Contains(necessarySubscriber))
    //         {
    //             AddSubscriberAsNecessary(necessarySubscriber);
    //         }
    //     }
    //     
    //     public virtual void NotifyNotNecessary(IFactorSubscriber unnecessarySubscriber) =>
    //         RemoveSubscriberFromNecessary(unnecessarySubscriber);
    //     
    //     protected virtual void AddSubscriberAsNecessary(IFactorSubscriber necessarySubscriber) => 
    //         necessarySubscribers.Add(necessarySubscriber);
    //
    //     protected virtual void RemoveSubscriberFromNecessary(IFactorSubscriber unnecessarySubscriber) => 
    //         necessarySubscribers.Remove(unnecessarySubscriber);
    //
    // }
}