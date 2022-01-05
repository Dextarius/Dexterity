using System;
using System.Collections.Generic;
using System.Diagnostics;
using Core;
using Core.Factors;
using Core.States;
using JetBrains.Annotations;

namespace Factors.Outcomes.Influences
{ 
    public abstract class FactorCore : IFactor
    {
        #region Static Fields

        [ThreadStatic]
        private static UpdateList updateList;

        #endregion
        
        #region Instance Fields

        [NotNull, ItemNotNull]
        protected HashSet<WeakReference<IFactorSubscriber>> affectedResults = new HashSet<WeakReference<IFactorSubscriber>>();
        protected int                                numberOfNecessaryDependents;

        #endregion
        
        
        #region Static Properties

        public static UpdateList UpdateList => updateList ??= new UpdateList();

        #endregion

        #region Instance Properties

        public          string Name               { get; }
        public abstract int    Priority           { get; }
        public          bool   IsNecessary        => numberOfNecessaryDependents > 0;
        public          bool   HasSubscribers      => affectedResults.Count > 0;
        public          int    NumberOfSubscribers => affectedResults.Count;

        #endregion
        

        #region Instance Methods
        
        public virtual bool Subscribe(IFactorSubscriber subscriberToAdd)
        {
            if (subscriberToAdd == null) { throw new ArgumentNullException(nameof(subscriberToAdd)); }

            if (affectedResults.Add(subscriberToAdd.WeakReference))
            {
                if (subscriberToAdd.IsNecessary)
                {
                    numberOfNecessaryDependents++;
                }

                return true;
            }

            return false;
        }

        public virtual void Unsubscribe(IFactorSubscriber subscriberToRemove)
        {
            if (subscriberToRemove != null)
            {
                if (affectedResults.Remove(subscriberToRemove.WeakReference)  &&  
                    subscriberToRemove.IsNecessary)
                {
                    numberOfNecessaryDependents--;
                }
            }
        }

        public void TriggerSubscribers()
        {
            var formerDependents = affectedResults;

            if (formerDependents.Count > 0)
            {
                using (UpdateList.QueueUpdates())
                {
                    foreach (var outcomeReference in formerDependents)
                    {
                        if (outcomeReference.TryGetTarget(out var outcome))
                        {
                            outcome.Trigger(this);
                        }
                    }
                    
                    formerDependents.Clear();
                    numberOfNecessaryDependents = 0;
                }
            }
            
            //- TODO : We might be able to skip establishing the UpdateQueue if there's only 1 dependent.
        }
        
        
        public virtual void NotifyNecessary()
        {
            #if DEBUG
            Debug.Assert(numberOfNecessaryDependents >= 0);
            Debug.Assert(numberOfNecessaryDependents < affectedResults.Count);
            #endif
            
            numberOfNecessaryDependents++;
        }
        
        public virtual void NotifyNotNecessary()
        {
            #if DEBUG
            Debug.Assert(numberOfNecessaryDependents > 0);
            Debug.Assert(numberOfNecessaryDependents <= affectedResults.Count);
            #endif
            
            numberOfNecessaryDependents--;
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