using System;
using System.Collections.Generic;
using Core.Causality;
using Core.States;

namespace Causality.States.CollectionStates
{
    public abstract class CollectionResult<TCollection, TValue> : Result, ICollectionResult<TCollection, TValue> where TCollection : ICollection<TValue>
    {
        #region Instance Fields

        protected readonly IProcess<IEnumerable<TValue>> collectionProcess;
        protected          TCollection                   currentCollection;
        
        //- TODO : We could make Processes that convert an IEnumerable into the target collection type.  That would
        //         allow us to change the reactionProcess to an IProcess<TCollection>.  As a result we would also be
        //         able to accept IProcess<TCollection> arguments in the constructor, and those could get collections
        //         directly, ithout having to make a new collection out of an IEnumerable.
        //         

        #endregion

        #region Properties

        public TCollection Collection
        {
            get
            {
                Reconcile();
                Observer.NotifyInvolved(this);
                return currentCollection;
            }
        }

        #endregion


        #region Instance Methods

        
        protected override bool ExecuteProcess()
        {
            TCollection         oldCollection = currentCollection;
            IEnumerable<TValue> newElements   = Observer.ObserveInteractions(collectionProcess, this);

            using (Observer.PauseObservation())
            {
                // if (UsesRecycling)
                // {
                //     //- TODO: Implement this.
                // }

                TCollection newCollection = CreateCollectionFromElements(newElements);

                if (AreCollectionsEqual(newCollection, oldCollection) is false)
                {
                    currentCollection = newCollection;
                    return true;
                }
                
                return false;
            }
        }
        
        protected abstract TCollection CreateCollectionFromElements(IEnumerable<TValue> newElements);

        protected abstract bool AreCollectionsEqual(TCollection collection1, TCollection collection2);

        public void CopyTo(TValue[] array, int arrayIndex) => Collection.CopyTo(array, arrayIndex);

        public bool Contains(TValue item) => Collection.Contains(item);
        
        public IEnumerator<TValue> GetEnumerator()
        {
            foreach (TValue element in Collection)
            {
                NotifyInvolved();
                yield return element;
            }
            
            //- TODO : Do we really want to call NotifyInvolved() on every iteration?
        }
        
        public TCollection Peek() => currentCollection;


        #endregion


        #region Constructors
        
        public CollectionResult(object owner, IProcess<IEnumerable<TValue>> processToDetermineValue): base(owner)
        {
            collectionProcess = processToDetermineValue ?? throw new ArgumentNullException(nameof(processToDetermineValue));
        }

        #endregion
    }



}