using System;
using System.Collections;
using System.Collections.Generic;
using Core.Causality;
using Core.Factors;
using Core.States;

namespace Factors.Outcomes.ObservedOutcomes.CollectionResults
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
                Observer.NotifyInvolved(this);
                return currentCollection;
            }
        }

        #endregion


        #region Instance Methods

        protected override bool GenerateOutcome()
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

                if (AreCollectionsEqual(newCollection, oldCollection) is false)
                {
                    currentCollection = newCollection;
                    return true;
                }
                
                return false;
            }
        }
        
        public void CopyTo(TValue[] array, int index) => Collection.CopyTo(array, index);
        public void CopyTo(Array    array, int index) => ((ICollection)Collection).CopyTo(array, index);
        public bool Contains(TValue item)             => Collection.Contains(item);
        
        public IEnumerator<TValue> GetEnumerator()
        {
            foreach (TValue element in Collection)
            {
                NotifyInvolved();
                yield return element;
            }
            
            //- TODO : Do we really want to call NotifyInvolved() on every iteration?
        }
        
        protected abstract IEnumerable<TValue> GetElements();
        protected abstract TCollection         CreateCollectionFromElements(IEnumerable<TValue> newElements);
        protected abstract bool                AreCollectionsEqual(TCollection collection1, TCollection collection2);

        #endregion
        
        
        #region Constructors

        protected ObservedCollectionResult(string name) : base(name)
        {
            
        }

        #endregion
        

        #region Explicit Implementations

        IEnumerable<TValue> IProcess<IEnumerable<TValue>>.Execute() => GetElements();

        #endregion
    }



}