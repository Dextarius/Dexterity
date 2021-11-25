using System;
using System.Collections.Generic;
using System.Diagnostics;
using Core.Causality;
using Core.Factors;
using Core.States;
using JetBrains.Annotations;

namespace Causality.States
{
    public class Influence
    {
        #region Static Fields

        [ThreadStatic]
        private static UpdateList updateList;

        #endregion
        

        #region Instance Fields

        [NotNull, ItemNotNull]
        protected HashSet<WeakReference<IDependency>> affectedResults = new HashSet<WeakReference<IDependency>>();
        //protected object referenceToOwner;
        protected int    numberOfNecessaryDependents;

        #endregion
        
        
        // #region Static Properties
        //
        // protected static CausalObserver Observer   => CausalObserver.ForThread;
        // protected static UpdateList     UpdateList => updateList??  (updateList = new UpdateList());
        //
        // #endregion


        #region Instance Properties

        public bool   IsNecessary        => numberOfNecessaryDependents > 0;
        public bool   HasDependents      => affectedResults.Count > 0;
        public int    NumberOfDependents => affectedResults.Count;

        #endregion


        // #region Static Methods
        //
        // public static void Observe(IProcess process, IInteraction interaction) =>
        //     Observer.ObserveInteractions(process, interaction);
        //
        // public static T Observe<T>(IProcess<T> process, IInteraction interaction) =>
        //     Observer.ObserveInteractions(process, interaction);
        //
        // public static void Update<T>(T objectToUpdate) where T : IUpdateable, IPrioritizable => UpdateList.Update(objectToUpdate);
        //
        // #endregion


        #region Instance Methods

        //public void NotifyInvolved() => Observer.NotifyInvolved(this);
        
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
        
        public virtual bool AddDependent(IInteraction dependentToAdd)
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


        public void ReleaseDependent(IInteraction dependentToRelease)
        {
            if (dependentToRelease != null)
            {
                if (affectedResults.Remove(dependentToRelease.WeakReference)  &&  dependentToRelease.IsNecessary)
                {
                    numberOfNecessaryDependents--;
                }
            }
        }
        
        protected bool DestabilizeDependents()
        {
            var formerDependents = affectedProducts;

            if (formerDependents.Count > 0)
            {
                foreach (var dependentReference in formerDependents)
                {
                    if (dependentReference.TryGetTarget(out var dependent))
                    {
                        if (dependent.Destabilize())
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        // public virtual bool Stabilize()
        // {
        //     return false; 
        //     //- TODO : This seems like it needs to be redesigned.  States can't be Unstable, so why can they be stabilized?
        // }

        #endregion


        #region Constructors

        public Influence(object ownerToReference)
        {
            //referenceToOwner = ownerToReference;
        }
        
        public Influence() : this(null)
        {
        }

        #endregion
    }
}