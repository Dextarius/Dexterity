using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Core;
using Core.Factors;
using Core.States;
using JetBrains.Annotations;

namespace Factors.Cores
{ 
    public abstract class FactorCore : IFactor
    {
        #region Static Fields

        [ThreadStatic]
        private static UpdateList updateList;

        #endregion
        
        #region Instance Fields

        [NotNull, ItemNotNull]
        protected readonly HashSet<IFactorSubscriber> allSubscribers       = new HashSet<IFactorSubscriber>();
        protected readonly HashSet<IFactorSubscriber> necessarySubscribers = new HashSet<IFactorSubscriber>();

        #endregion
        
        
        #region Static Properties

        public static UpdateList UpdateList => updateList ??= new UpdateList();

        #endregion

        #region Instance Properties

        public          string Name                { get; }
        public abstract int    UpdatePriority      { get; }
        public virtual  bool   IsNecessary         => necessarySubscribers.Count > 0;
        public          bool   HasSubscribers      => allSubscribers.Count > 0;
        public          int    NumberOfSubscribers => allSubscribers.Count;

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
                }

                return true;
            }
            else return false;
        }

        public virtual void Unsubscribe(IFactorSubscriber subscriberToRemove)
        {
            if (subscriberToRemove != null)
            {
                if (allSubscribers.Remove(subscriberToRemove))
                {
                    RemoveSubscriberFromNecessary(subscriberToRemove);
                }
            }
        }

        public void TriggerSubscribers()
        {
            var formerSubscribers = allSubscribers;

            if (formerSubscribers.Count > 0)
            {
                using (UpdateList.QueueUpdates())
                {
                    formerSubscribers.RemoveWhere(TriggerSubscriberAndPotentiallyRemoveThem);
                }
            }
            
            //- TODO : We might be able to skip establishing the UpdateQueue if there's only 1 subscriber.
            //         If this factor was triggered by another one, the subscriber will get picked up by their
            //         queue anyways.
        }

        private bool TriggerSubscriberAndPotentiallyRemoveThem(IFactorSubscriber subscriber)
        {
            subscriber.Trigger(this, out bool removeSubscription);

            if (removeSubscription)
            {
                //- Working On : Do we want to use RemoveSubscriberFromNecessary() here?
                //  We probably should, but that means we're going to check if we need
                //  to notify we're not necessary every time we remove one instead of just
                //  checking when we're done triggering all of them.

                RemoveSubscriberFromNecessary(subscriber);
                return true;
            }
            else return false;
        }
        
        //- Working On : Changing the subscription and trigger processes for observed reactors so that
        //               parents are only notified that they are necessary during destabilizing, not
        //               when they subscribe.

        public virtual void NotifyNecessary(IFactorSubscriber necessarySubscriber)
        {
            if (allSubscribers.Contains(necessarySubscriber))
            {
                AddSubscriberAsNecessary(necessarySubscriber);
            }
        }

        protected virtual void AddSubscriberAsNecessary(IFactorSubscriber necessarySubscriber) => 
            necessarySubscribers.Add(necessarySubscriber);

        public virtual void NotifyNotNecessary(IFactorSubscriber unnecessarySubscriber) => 
            RemoveSubscriberFromNecessary(unnecessarySubscriber);

        protected virtual void RemoveSubscriberFromNecessary(IFactorSubscriber necessarySubscriber) => 
            necessarySubscribers.Remove(necessarySubscriber);
        
        public virtual bool Reconcile()
        {
            return true; 
            //^ Reconcile is used when a subscriber is destabilized, and since only reactors destabilize their dependents,
            //  a basic factor should never be the parent that the caller needs to reconcile with.
        }
        
        public override string ToString() => Name;

        #endregion


        #region Constructors

        protected FactorCore(string name)
        {
            Name = name;
        }

        #endregion
    }
}