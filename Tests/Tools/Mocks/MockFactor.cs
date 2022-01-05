using System;
using System.Collections.Generic;
using System.Diagnostics;
using Core.Factors;
using Core.States;
using JetBrains.Annotations;

namespace Tests.Tools.Mocks
{
    internal class MockFactor : IFactor
    {
        #region Instance Fields

        [NotNull, ItemNotNull]
        protected HashSet<WeakReference<IFactorSubscriber>> affectedResults = new();
        protected int numberOfNecessaryDependents;

        #endregion
        
        public int    Priority           => 0;
        public string Name               => nameof(MockFactor);
        public bool   IsNecessary        => numberOfNecessaryDependents > 0;
        public bool   HasSubscribers      => affectedResults.Count > 0;
        public int    NumberOfSubscribers => affectedResults.Count;
        
        
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

            //- Note : This doesn't invalidate the dependents in order of Priority
        }
    
        public virtual bool Reconcile() => true;
    }
}