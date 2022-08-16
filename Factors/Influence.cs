using System;
using System.Collections.Generic;
using Core;
using Core.Factors;
using Core.States;
using Dextarius.Collections;
using JetBrains.Annotations;

namespace Factors
{
    public class Influence : IInfluence
    {
        #region Static Fields

        [ThreadStatic]
        private static UpdateList updateList;
        
        #endregion
        
        
        #region Instance Fields
        
        protected readonly HashSet<IFactorSubscriber> allSubscribers       = new HashSet<IFactorSubscriber>();
        protected readonly HashSet<IFactorSubscriber> necessarySubscribers = new HashSet<IFactorSubscriber>();

        #endregion
        
        #region Static Properties

        public static UpdateList UpdateList => updateList ??= new UpdateList();

        #endregion


        #region Instance Properties

        public virtual IInfluenceOwner Owner                   { get; }
        public         bool            HasSubscribers          => allSubscribers.Count > 0;
        public virtual bool            HasNecessarySubscribers => necessarySubscribers.Count > 0;
        public         int             NumberOfSubscribers     => allSubscribers.Count;

        #endregion


        #region Instance Methods

        public virtual bool Subscribe(IFactorSubscriber subscriberToAdd, bool isNecessary)
        {
            if (subscriberToAdd == null) { throw new ArgumentNullException(nameof(subscriberToAdd)); }

            if (allSubscribers.Add(subscriberToAdd))
            {
                if (isNecessary)
                {
                    AddSubscriberAsNecessary(subscriberToAdd);

                    if (NumberOfSubscribers is 1)
                    {
                        Owner.OnFirstSubscriberGained();
                    }
                }

                return true;
            }
            else return false;
        }

        public virtual bool Unsubscribe(IFactorSubscriber subscriberToRemove)
        {
            if (subscriberToRemove != null)
            {
                bool wasRemoved = allSubscribers.Remove(subscriberToRemove);
                
                if (wasRemoved)
                {
                    RemoveSubscriberFromNecessary(subscriberToRemove);

                    if (NumberOfSubscribers is 0)
                    {
                        Owner.OnLastSubscriberLost();
                    }
                }

                return wasRemoved;
            }
            else return false;
        }

        protected virtual void AddSubscriberAsNecessary(IFactorSubscriber necessarySubscriber)
        {
            bool wasAlreadyNecessary = HasNecessarySubscribers;
            
            necessarySubscribers.Add(necessarySubscriber);

            if (wasAlreadyNecessary is false)
            {
                Owner.OnNecessary();
                
                //- TODO : OnNecessary is going to update a Reactor, so do we want that to happen before or after
                //         we add the subscriber?
            }
        }
        
        protected virtual void RemoveSubscriberFromNecessary(IFactorSubscriber unnecessarySubscriber)
        {
            if (HasNecessarySubscribers)
            {
                necessarySubscribers.Remove(unnecessarySubscriber);

                if (HasNecessarySubscribers is false)
                {
                    Owner.OnNotNecessary();
                }
            }
        }
        
        public virtual void NotifyNecessary(IFactorSubscriber necessarySubscriber)
        {
            if (allSubscribers.Contains(necessarySubscriber))
            {
                AddSubscriberAsNecessary(necessarySubscriber);
            }
        }
        
        public virtual void NotifyNotNecessary(IFactorSubscriber unnecessarySubscriber) =>
            RemoveSubscriberFromNecessary(unnecessarySubscriber);
        
        public void TriggerSubscribers(IFactor triggeringFactor)
        {
            var formerSubscribers = allSubscribers;
                
            if (formerSubscribers.Count > 0)
            {
                using (UpdateList.QueueUpdates())
                {
                    formerSubscribers.RemoveWhere(TriggerSubscriberAndPotentiallyRemoveThem);
                }
            }
            //^ TODO : We might be able to skip establishing the UpdateQueue if there's only 1 subscriber.
            //         If this factor was triggered by another one, the subscriber will get picked up by their
            //         queue anyways.
            

            bool TriggerSubscriberAndPotentiallyRemoveThem(IFactorSubscriber subscriber)
            {
                subscriber.Trigger(triggeringFactor, out bool removeSubscription);

                if (removeSubscription)
                {
                    //- Working On : Do we want to use RemoveSubscriberFromNecessary() here?
                    //  We probably should, but that means we're going to check if we need
                    //  to notify that we're no longer necessary every time we remove one, instead of just
                    //  checking when we're done triggering all of them.

                    RemoveSubscriberFromNecessary(subscriber);
                    return true;
                }
                else return false;
            }
        }
        
        //- Doesn't actually destabilize all of the subscribers.
        //  Just however many as it takes to find out if we have a necessary one.
        public bool DestabilizeSubscribers(IFactor unstableFactor)
        {
            var subscribersToDestabilize = allSubscribers;

            if (subscribersToDestabilize.Count > 0)
            {
                foreach (var subscriber in subscribersToDestabilize)
                {
                    if (subscriber.Destabilize(unstableFactor))
                    {
                        AddSubscriberAsNecessary(subscriber);  
                        //^ What happens if this makes us Necessary?
                        //  If we're owned by a Reflexive Reactor will it start updating in
                        //  the middle of us destabilizing these subscribers?
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


        #region Constructors

        public Influence(IInfluenceOwner owner)
        {
            Owner = owner;
        }

        #endregion
        
    }
}