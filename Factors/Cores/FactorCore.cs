using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        protected readonly HashSet<WeakReference<IFactorSubscriber>> subscribers = new HashSet<WeakReference<IFactorSubscriber>>();
        protected int numberOfNecessarySubscribers;

        #endregion
        
        
        #region Static Properties

        public static UpdateList UpdateList => updateList ??= new UpdateList();

        #endregion

        #region Instance Properties

        public          string Name               { get; }
        public abstract int    Priority           { get; }
        public virtual  bool   IsNecessary        => numberOfNecessarySubscribers > 0;
        public          bool   HasSubscribers      => subscribers.Count > 0;
        public          int    NumberOfSubscribers => subscribers.Count;

        #endregion
        

        #region Instance Methods
        
        public virtual bool Subscribe(IFactorSubscriber subscriberToAdd)
        {
            if (subscriberToAdd == null) { throw new ArgumentNullException(nameof(subscriberToAdd)); }

            if (subscribers.Add(subscriberToAdd.WeakReference))
            {
                if (subscriberToAdd.IsNecessary)
                {
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
                if (subscribers.Remove(subscriberToRemove.WeakReference)  &&  
                    subscriberToRemove.IsNecessary)
                {
                    NotifyNotNecessary();
                }
            }
        }

        public void TriggerSubscribers()
        {
            var formerSubscribers = subscribers;

            if (formerSubscribers.Count > 0)
            {
                numberOfNecessarySubscribers = 0;
                
                using (UpdateList.QueueUpdates())
                {
                    formerSubscribers.RemoveWhere(TriggerSubscriberAndPotentiallyRemoveThem);
                }
            }
            
            //- TODO : We might be able to skip establishing the UpdateQueue if there's only 1 dependent.
        }
        
        private bool TriggerSubscriberAndPotentiallyRemoveThem(WeakReference<IFactorSubscriber> subscriberReference)
        {
            if (subscriberReference.TryGetTarget(out var subscriber))
            {
                subscriber.Trigger(this, out bool removeSubscription);

                if (removeSubscription)
                { 
                    return true;
                }
                else
                {
                    if (subscriber.IsNecessary)
                    {
                        numberOfNecessarySubscribers++;
                    }
                    
                    return false;
                }
            }
            else return true;
        }
        
        
        public virtual void NotifyNecessary()
        {
            #if DEBUG
            Debug.Assert(numberOfNecessarySubscribers >= 0);
            Debug.Assert(numberOfNecessarySubscribers < subscribers.Count);
            #endif
            
            numberOfNecessarySubscribers++;
        }
        
        public virtual void NotifyNotNecessary()
        {
            numberOfNecessarySubscribers--;
            
            #if DEBUG
            Debug.Assert(numberOfNecessarySubscribers >= 0);
            Debug.Assert(numberOfNecessarySubscribers <= subscribers.Count);
            #endif
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