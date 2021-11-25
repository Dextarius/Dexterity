using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using Core.Causality;
using Core.Factors;
using Core.States;
using JetBrains.Annotations;

namespace Causality.States
{
    public class CausalFactor : IFactor
    {
        #region Static Fields

        [ThreadStatic]
        private static UpdateList updateList;

        #endregion
        

        #region Instance Fields

        [NotNull, ItemNotNull]
        protected HashSet<WeakReference<IDependency>> affectedResults = new HashSet<WeakReference<IDependency>>();
        protected object referenceToOwner;
        protected int    numberOfNecessaryDependents;

        #endregion
        
        
        #region Static Properties

        protected static CausalObserver Observer   => CausalObserver.ForThread;
        protected static UpdateList     UpdateList => updateList??  (updateList = new UpdateList());

        #endregion


        #region Instance Properties

        public         string Name               { get; }
        public         bool   IsNecessary        => numberOfNecessaryDependents > 0;
        public         bool   HasDependents      => affectedResults.Count > 0;
        public         int    NumberOfDependents => affectedResults.Count;
        public virtual int    Priority           => 0;
        
        #endregion


        #region Static Methods

        public static void Observe(IProcess process, IInfluenceable interaction) =>
            Observer.ObserveInteractions(process, interaction);

        public static T Observe<T>(IProcess<T> process, IInfluenceable interaction) =>
            Observer.ObserveInteractions(process, interaction);
        
        public static void Update<T>(T objectToUpdate) where T : IUpdateable, IPrioritizable => UpdateList.Update(objectToUpdate);

        #endregion


        #region Instance Methods

        public void NotifyInvolved() => Observer.NotifyInvolved(this);
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
            
            //- We could probably skip establishing the UpdateQueue if there's only 1 dependent.
        }
        
        public virtual bool AddDependent(IDependency dependentToAdd)
        {
            if (dependentToAdd == null) { throw new ArgumentNullException(nameof(dependentToAdd)); }

            if (dependentToAdd != this)
            {
                if (affectedResults.Add(dependentToAdd.WeakReference))
                {
                    if (dependentToAdd.IsNecessary)
                    {
                        numberOfNecessaryDependents++;
                    }

                    return true;
                }
            }

            return false;
        }


        public void ReleaseDependent(IDependency dependentToRelease)
        {
            if (dependentToRelease != null)
            {
                if (affectedResults.Remove(dependentToRelease.WeakReference)  &&  dependentToRelease.IsNecessary)
                {
                    numberOfNecessaryDependents--;
                }
            }
        }

        public virtual bool Reconcile()
        {
            return true; 
            //- A non-reactive factor never destabilizes its dependents, so it should never be the parent that 
            //  the caller needs to reconcile with.
            //- TODO : This seems like it needs to be redesigned.  States can't be Unstable, so why can they be stabilized?
        }
        
        public virtual void OnChanged()
        {
            Observer.NotifyChanged(this);
            InvalidateDependents();
        }

        #endregion


        #region Constructors

        public CausalFactor(object ownerToReference)
        {
            referenceToOwner = ownerToReference;
        }
        
        public CausalFactor() : this(null)
        {
        }

        #endregion
    }

    //- TODO : Implement a mechanic that allows a user to set a separate 'priority' level to indicate one outcome should
    //         be calculated before/after another.  Alternatively/Additionally we should allow them to specify a list of
    //         outcomes to calculate before / after a given outcome.  
    //         Is this really a good idea though?  In what situation would you want an outcome to be updated before
    //         something that doesn't directly rely on it?
}
