using System;
using System.Collections.Generic;
using System.Diagnostics;
using Core;
using Core.Factors;
using Core.States;
using JetBrains.Annotations;

namespace Factors.Outcomes.Influences
{ 
    public abstract class Influence : IFactor
    {
        #region Static Fields

        [ThreadStatic]
        private static UpdateList updateList;

        #endregion
        
        #region Instance Fields

        [NotNull, ItemNotNull]
        protected HashSet<WeakReference<IDependent>> affectedResults = new HashSet<WeakReference<IDependent>>();
        protected int                                numberOfNecessaryDependents;

        #endregion
        
        
        #region Static Properties

        public static UpdateList UpdateList => updateList ??= new UpdateList();

        #endregion

        #region Instance Properties

        public          string Name               { get; }
        public abstract int    Priority           { get; }
        public          bool   IsNecessary        => numberOfNecessaryDependents > 0;
        public          bool   HasDependents      => affectedResults.Count > 0;
        public          int    NumberOfDependents => affectedResults.Count;

        #endregion
        

        #region Instance Methods
        
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
                using (UpdateList.QueueUpdates())
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

        protected Influence(string name)
        {
            Name = name;
        }

        #endregion
    }
}