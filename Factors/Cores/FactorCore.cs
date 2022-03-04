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
        private Dictionary<IFactorSubscriber, SubscriptionState> subscriptionStateBySubscriber = new Dictionary<IFactorSubscriber, SubscriptionState>();
        protected uint numberOfNecessarySubscribers;

        #endregion
        
        
        #region Static Properties

        public static UpdateList UpdateList => updateList ??= new UpdateList();

        #endregion

        #region Instance Properties

        public          string Name                { get; }
        public abstract int    Priority            { get; }
        public virtual  bool   IsNecessary         => numberOfNecessarySubscribers > 0;
        public          bool   HasSubscribers      => subscriptionStateBySubscriber.Count > 0;
        public          int    NumberOfSubscribers => subscriptionStateBySubscriber.Count;

        #endregion
        

        #region Instance Methods
        
        public virtual bool Subscribe(IFactorSubscriber subscriberToAdd)
        {
            if (subscriberToAdd == null) { throw new ArgumentNullException(nameof(subscriberToAdd)); }

            SubscriptionState subscriptionState = new SubscriptionState(true, subscriberToAdd.IsNecessary);
            
            if (subscriptionStateBySubscriber.TryAdd(subscriberToAdd, subscriptionState))
            {
                if (subscriberToAdd.IsNecessary)
                {
                   // numberOfNecessarySubscribers++;
                    NotifyNecessary();
                }

                return true;
            }

            return false;
        }

        public virtual void Unsubscribe(IFactorSubscriber subscriberToRemove)
        {
            if (subscriberToRemove != null)
            {
                subscribers.Remove(subscriberToRemove);
                
            }
        }

        public void TriggerSubscribers()
        {
            var formerSubscribers = subscriptionStateBySubscriber;

            if (formerSubscribers.Count > 0)
            {
                using (UpdateList.QueueUpdates())
                {
                    formerSubscribers.RemoveWhere(TriggerSubscriberAndPotentiallyRemoveThem);
                }
            }
            
            //- TODO : We might be able to skip establishing the UpdateQueue if there's only 1 dependent.
        }

        private bool TriggerSubscriberAndPotentiallyRemoveThem(IFactorSubscriber subscriber)
        {
            subscriber.Trigger(this, out bool removeSubscription);

            if (removeSubscription)
            {
                if (subscriber.IsNecessary)
                {
                    numberOfNecessarySubscribers--;
                }
                
                return true;
            }
            else
            {
                return false;
            }
        }


        public virtual void NotifyNecessary()
        {
            // #if DEBUG
            // Debug.Assert(numberOfNecessarySubscribers >= 0);
            // Debug.Assert(numberOfNecessarySubscribers < subscribers.Count);
            // #endif

            if (numberOfNecessarySubscribers < uint.MaxValue)
            {
                numberOfNecessarySubscribers++;
            }
        }
        
        public virtual void NotifyNotNecessary()
        {
            if (numberOfNecessarySubscribers > 0)
            {
                numberOfNecessarySubscribers--;
            }
            
            // #if DEBUG
            // Debug.Assert(numberOfNecessarySubscribers >= 0);
            // Debug.Assert(numberOfNecessarySubscribers <= subscribers.Count);
            // #endif
        }
        
        public virtual bool Reconcile()
        {
            return true; 
            //^ A non-reactive factor never destabilizes its dependents, so unless this is a reactive
            //  it should never be the parent that the caller needs to reconcile with.
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