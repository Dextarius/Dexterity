using System;
using System.Collections;
using System.Collections.Generic;
using Core.Causality;
using Core.States;
using static Factors.CollectionFactor;

namespace Factors.Cores.ObservedReactorCores.CollectionResults
{
    public abstract class ObservedCollectionResult<TCollection, TValue> : 
        ObservedReactorCore, ICollectionResult<TValue>, IProcess<IEnumerable<TValue>>
            where TCollection : ICollection<TValue>
    {
        #region Instance Fields

        protected TCollection currentCollection;
        
        //- TODO : We could also make Processes that convert an IEnumerable into the target collection type.  That would
        //         allow us to change the reactionProcess to an IProcess<TCollection>.  As a result we would also be
        //         able to accept IProcess<TCollection> arguments in the constructor, and those could get collections
        //         directly, without having to make a new collection out of an IEnumerable.
        //         

        #endregion

        #region Properties

        public int Count => Collection.Count;

        protected TCollection Collection
        {
            get
            {
              //  AttemptReaction();
                NotifyInvolved();
                return currentCollection;
            }
        }

        #endregion


        #region Instance Methods

        protected override long CreateOutcome()
        {
            TCollection         oldCollection = currentCollection;
            IEnumerable<TValue> newElements   = 
                Observer.ObserveInteractions<ObservedCollectionResult<TCollection, TValue>, IEnumerable<TValue>>(this);

            using (Observer.PauseObservation())
            {
                // if (UsesRecycling)
                // {
                //     //- TODO: Implement this.
                // }

                TCollection newCollection = CreateCollectionFromElements(newElements);

                RemoveUnusedTriggers();

                if (AreCollectionsEqual(newCollection, oldCollection, out var triggerFlags) is false)
                {
                    currentCollection = newCollection;
                }
                
                return triggerFlags;
            }
        }
        
        public void CopyTo(TValue[] array, int index)
        {
            Collection.CopyTo(array, index);
            NotifyInvolved(TriggerFlags.ItemAdded | TriggerFlags.ItemRemoved);
        }
        
        public void CopyTo(Array    array, int index)
        {
            ((ICollection) Collection).CopyTo(array, index);
            NotifyInvolved(TriggerFlags.ItemAdded | TriggerFlags.ItemRemoved);
        }
        
        public bool Contains(TValue item)
        {
            if (Collection.Contains(item))
            {
                NotifyInvolved(TriggerFlags.ItemRemoved);
                return true;
            }
            else
            {
                NotifyInvolved(TriggerFlags.ItemAdded);
                return false;
            }
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            NotifyInvolved(TriggerFlags.ItemAdded | TriggerFlags.ItemRemoved);

            foreach (TValue element in Collection)
            {
                yield return element;
            }
            
            //- TODO : Do we want to call NotifyInvolved() on every iteration?
        }

        
        //- This method should never return null, because most of the operations these classes take assumes there is 
        //  a collection at all times even if it's just an empty one.
        protected abstract TCollection         CreateCollectionFromElements(IEnumerable<TValue> newElements);
        protected abstract IEnumerable<TValue> GetElements();
        protected abstract bool                AreCollectionsEqual(TCollection collection1, TCollection collection2, out long triggerFlags);

        #endregion
        
        
        #region Constructors

        protected ObservedCollectionResult() : base()
        {
            
        }

        #endregion
        

        #region Explicit Implementations

        IEnumerable<TValue> IProcess<IEnumerable<TValue>>.Execute() => GetElements();

        #endregion
    }
}