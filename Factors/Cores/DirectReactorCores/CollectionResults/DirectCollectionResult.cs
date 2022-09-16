using System;
using System.Collections;
using System.Collections.Generic;
using Core.Causality;
using Core.States;
using Factors.Cores.ObservedReactorCores;
using Factors.Cores.ObservedReactorCores.CollectionResults;

namespace Factors.Cores.DirectReactorCores.CollectionResults
{
    public abstract class DirectCollectionResult<TCollection, TValue> : 
        DirectReactorCore, ICollectionResult<TValue>, IProcess<IEnumerable<TValue>>
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

        protected TCollection Collection => currentCollection;

        #endregion


        #region Instance Methods

        protected override long CreateOutcome()
        {
            TCollection         oldCollection = currentCollection;
            IEnumerable<TValue> newElements   = GetElements();

            // if (UsesRecycling)
            // {
            //     //- TODO: Implement this.
            // }

            TCollection newCollection = CreateCollectionFromElements(newElements);

            if (AreCollectionsEqual(newCollection, oldCollection, out var triggerFlags))
            {
                currentCollection = newCollection;
            }
                
            return triggerFlags;
        }

        public void CopyTo(TValue[] array, int index) => Collection.CopyTo(array, index);
        public void CopyTo(Array    array, int index) => ((ICollection)Collection).CopyTo(array, index);
        public bool Contains(TValue item)             => Collection.Contains(item);

        public IEnumerator<TValue> GetEnumerator()
        {
            foreach (TValue element in Collection)
            {
             // NotifyInvolved();
                yield return element;
            }
            
            //- TODO : Do we really want to call NotifyInvolved() on every iteration?
        }
        
        public bool CollectionEquals(IEnumerable<TValue> collectionToCompare)
        {
            var collectionX = CreateCollectionFromElements(collectionToCompare);
            
            return AreCollectionsEqual(collectionX , collectionX, out _);
        }

        //- This method should never return null, because most of the operations these classes take assumes there is 
        //  a collection at all times even if it's just an empty one.
        protected abstract TCollection         CreateCollectionFromElements(IEnumerable<TValue> newElements);
        public abstract    bool                CollectionEquals(IEnumerable collectionToCompare);
        protected abstract IEnumerable<TValue> GetElements();
        protected abstract bool                AreCollectionsEqual(TCollection collection1, 
                                                                   TCollection collection2, 
                                                               out long        triggerFlags);

        #endregion


        #region Explicit Implementations

        IEnumerable<TValue> IProcess<IEnumerable<TValue>>.Execute() => GetElements();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

    }
}