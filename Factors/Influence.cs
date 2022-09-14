using System;
using System.Collections.Generic;
using System.Diagnostics;
using Core;
using Core.Factors;
using Core.States;
using Dextarius.Collections;
using JetBrains.Annotations;
using static Factors.Factor<Core.Factors.IFactorCore>;

namespace Factors
{
    public class Influence : IInfluence
    {
        #region Static Fields
    
        [ThreadStatic]
        private static UpdateList updateList;
        
        #endregion
        
        #region Instance Fields
        
        protected readonly Dict<IFactorSubscriber, bool> allSubscribers = new Dict<IFactorSubscriber, bool>();
        protected          int                           numberOfNecessarySubscribers;

        #endregion
        
        #region Static Properties
    
        public static UpdateList UpdateList => updateList ??= new UpdateList();
    
        #endregion
    
    
        #region Instance Properties
    
        public bool HasSubscribers               => allSubscribers.Count > 0;
        public int  NumberOfSubscribers          => allSubscribers.Count;
        public bool HasNecessarySubscribers      => numberOfNecessarySubscribers > 0;
        public int  NumberOfNecessarySubscribers => numberOfNecessarySubscribers;
    
        #endregion
    
    
        #region Instance Methods
    
        public virtual bool Subscribe(IFactorSubscriber subscriberToAdd, bool isNecessary)
        {
            if (subscriberToAdd == null) { throw new ArgumentNullException(nameof(subscriberToAdd)); }

            bool subscriberWasAlreadySubscribed = allSubscribers.TryGetValue(subscriberToAdd, out var currentIsNecessaryValue);
            
            if (subscriberWasAlreadySubscribed)
            {
                if (isNecessary != currentIsNecessaryValue)
                {
                    allSubscribers[subscriberToAdd] = isNecessary;
                    
                    if (isNecessary)
                    {
                        numberOfNecessarySubscribers++;
                    }
                    else
                    {
                        numberOfNecessarySubscribers--;
                    }
                }
            }
            else
            {
                allSubscribers[subscriberToAdd] = isNecessary;
                
                if (isNecessary)
                {
                    numberOfNecessarySubscribers++;
                }
            }

            return subscriberWasAlreadySubscribed is false;
        }
    
        public virtual bool Unsubscribe(IFactorSubscriber subscriberToRemove)
        {
            if (subscriberToRemove is null)
            {
                return false;
            }
            else
            {
                bool wasRemoved = allSubscribers.TryWithdraw(subscriberToRemove, out var wasNecessary);

                if (wasRemoved && wasNecessary)
                {
                    numberOfNecessarySubscribers--;
                }

                return wasRemoved;
            }
        }
    
        // protected virtual void AddSubscriberAsNecessary(IFactorSubscriber necessarySubscriber) => 
        //     necessarySubscribers.Add(necessarySubscriber);
        //
        // protected virtual void RemoveSubscriberFromNecessary(IFactorSubscriber unnecessarySubscriber) => 
        //     necessarySubscribers.Remove(unnecessarySubscriber);
    
        public virtual void NotifyNecessary(IFactorSubscriber necessarySubscriber)
        {
            if (allSubscribers.TryGetValue(necessarySubscriber, out var currentIsNecessaryValue))
            {
                if (currentIsNecessaryValue is false)
                {
                    allSubscribers[necessarySubscriber] = true;
                    numberOfNecessarySubscribers++;
                }
            }
        }
        
        public virtual void NotifyNotNecessary(IFactorSubscriber unnecessarySubscriber) 
        {
            if (allSubscribers.TryGetValue(unnecessarySubscriber, out var currentIsNecessaryValue))
            {
                if (currentIsNecessaryValue is true)
                {
                    allSubscribers[unnecessarySubscriber] = false;
                    numberOfNecessarySubscribers--;
                }
            }
        }
        
        public void TriggerSubscribers(IFactor triggeringFactor, long triggerFlags)
        {
            var subscribers = allSubscribers;
                
            if (subscribers.Count > 0)
            {
                using (UpdateList.QueueUpdates())
                {
                    foreach (var keyValuePair in allSubscribers.AsUnguardedEnumerable())
                    {
                        var subscriber = keyValuePair.Key;

                        subscriber.Trigger(triggeringFactor, triggerFlags, out bool removeSubscription);

                        if (removeSubscription)
                        {
                            Unsubscribe(subscriber);
                        }
                    }
                }
            }
            //^ TODO : We might be able to skip establishing the UpdateQueue if there's only 1 subscriber.
            //         If this factor was triggered by another one, the subscriber will get picked up by their
            //         queue anyways.
            //^ Note : This is not true.  The subscriber might have a higher update priority than something
            //         else triggered by whatever triggered us.
        }
        
        //- Doesn't actually destabilize all of the subscribers.
        //  Just however many as it takes to find out if we have a necessary one.
        public bool DestabilizeSubscribers(IFactor unstableFactor)
        {
            var subscribersToDestabilize = allSubscribers;
    
            if (subscribersToDestabilize.Count > 0)
            {
                foreach (var keyValuePair in subscribersToDestabilize)
                {
                    var subscriber = keyValuePair.Key;
                    
                    if (subscriber.Destabilize())
                    {
                        allSubscribers[subscriber] = true;
                        numberOfNecessarySubscribers++;                        
                        return true;
                    }
                }
    
                //- TODO : We specifically tried to avoid using foreach before when triggering subscribers because they might
                //         choose to remove themselves, so consider if we really want to use it here.  
                //-        Well we don't really want to use the RemoveWhere() method like we did in TriggerSubscribers()
                //         because we want to end this method as soon as we find a necessary subscriber.
            }
            
            return false;
        }
    
        #endregion
    }
    
    

    // public class ListInfluence<TValue>
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
    //     protected readonly HashSet<IListFactorSubscriber<TValue>> subscribers = new HashSet<IListFactorSubscriber<TValue>>();
    //
    //     #endregion
    //     
    //     #region Static Properties
    //
    //     public static UpdateList UpdateList => updateList ??= new UpdateList();
    //
    //     #endregion
    //
    //
    //     #region Instance Properties
    //
    //     public bool HasSubscribers      => subscribers.Count > 0;
    //     public int  NumberOfSubscribers => subscribers.Count;
    //
    //     #endregion
    //
    //
    //     #region Instance Methods
    //
    //     public virtual bool Subscribe(IListFactorSubscriber<TValue> subscriberToAdd, bool isNecessary)
    //     {
    //         if (subscriberToAdd == null) { throw new ArgumentNullException(nameof(subscriberToAdd)); }
    //
    //         if (subscribers.Add(subscriberToAdd))
    //         {
    //             return true;
    //         }
    //         else return false;
    //     }
    //
    //     public virtual bool Unsubscribe(IListFactorSubscriber<TValue> subscriberToRemove)
    //     {
    //         if (subscriberToRemove != null)
    //         {
    //             bool wasRemoved = subscribers.Remove(subscriberToRemove);
    //
    //             return wasRemoved;
    //         }
    //         else return false;
    //     }
    //
    //
    //     
    //     void ItemAdded(IList<TValue> list, TValue itemAdded, int index)
    //     {
    //         foreach (var subscriber in subscribers)
    //         {
    //             subscriber.ItemAdded(list, itemAdded, index);
    //         }
    //     }
    //
    //     void ItemMoved(IList<TValue> list, TValue itemMoved, int oldIndex, int newIndex)
    //     {
    //         foreach (var subscriber in subscribers)
    //         {
    //             subscriber.ItemMoved(list, itemMoved, oldIndex, newIndex);
    //         }
    //     }
    //
    //     void ItemRemoved(IList<TValue> list, TValue itemRemoved, int index)
    //     {
    //         foreach (var subscriber in subscribers)
    //         {
    //             subscriber.ItemRemoved(list, itemRemoved, index);
    //         }
    //     }
    //
    //     void ItemReplaced(IList<TValue> list, TValue itemReplaced, int index)
    //     {
    //         foreach (var subscriber in subscribers)
    //         {
    //             subscriber.ItemReplaced(list, itemReplaced, index);
    //         }
    //     }
    //
    //     void Rearranged(IList<TValue> list)
    //     {
    //         foreach (var subscriber in subscribers)
    //         {
    //             subscriber.Rearranged(list);
    //         }
    //     }
    //     
    //     public void TriggerSubscribers(IFactor triggeringFactor)
    //     {
    //         var subscribers = this.subscribers;
    //             
    //         if (subscribers.Count > 0)
    //         {
    //             using (UpdateList.QueueUpdates())
    //             {
    //                 subscribers.RemoveWhere(TriggerSubscriberAndPotentiallyRemoveThem);
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
    //     //- Doesn't actually destabilize all of the subscribers.
    //     //  Just however many as it takes to find out if we have a necessary one.
    //     public bool DestabilizeSubscribers(IFactor unstableFactor)
    //     {
    //         var subscribersToDestabilize = subscribers;
    //
    //         if (subscribersToDestabilize.Count > 0)
    //         {
    //             foreach (var subscriber in subscribersToDestabilize)
    //             {
    //                 if (subscriber.Destabilize())
    //                 {
    //                     AddSubscriberAsNecessary(subscriber);
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
    //     #endregion
    // }
}