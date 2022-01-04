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
        protected HashSet<WeakReference<IDependent>> affectedResults = new();
        protected int numberOfNecessaryDependents;

        #endregion
        
        public int    Priority           => 0;
        public string Name               => nameof(MockFactor);
        public bool   IsNecessary        => numberOfNecessaryDependents > 0;
        public bool   HasDependents      => affectedResults.Count > 0;
        public int    NumberOfDependents => affectedResults.Count;
        
        
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
        
        public virtual bool AddDependent(IDependent dependentToAdd)
        {
            if (dependentToAdd == null) { throw new ArgumentNullException(nameof(dependentToAdd)); }

            if (affectedResults.Add(dependentToAdd.WeakReference))
            {
                if (dependentToAdd.IsNecessary)
                {
                    numberOfNecessaryDependents++;
                }

                return true;
            }

            return false;
        }

        public virtual void RemoveDependent(IDependent dependentToRemove)
        {
            if (dependentToRemove != null)
            {
                if (affectedResults.Remove(dependentToRemove.WeakReference)  &&  
                    dependentToRemove.IsNecessary)
                {
                    numberOfNecessaryDependents--;
                }
            }
        }

        public void InvalidateDependents()
        {
            var formerDependents = affectedResults;

            if (formerDependents.Count > 0)
            {
                foreach (var outcomeReference in formerDependents)
                {
                    if (outcomeReference.TryGetTarget(out var outcome))
                    {
                        outcome.Invalidate(this);
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