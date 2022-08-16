using System;
using System.Collections;
using System.Collections.Generic;
using Core.States;

namespace Factors.Cores.ProactiveCores
{
    public abstract class ObservedCollectionCore<TCollection, TValue> : ObservedProactorCore, ICollectionCore<TValue> 
        where TCollection : ICollection<TValue>
    {
        #region Instance Fields

        protected TCollection collection;
        
        #endregion
        
        #region Properties

        public int Count => Collection.Count;
        
        protected TCollection Collection
        {
            get => collection;
            set => collection = value; 
        }

        #endregion


        #region Instance Methods
        
        protected void OnCollectionChanged()
        {
            NotifyChanged();
            Callback.CoreUpdated(this);
        }

        public void Add(TValue item)
        {
            int oldCount = collection.Count;

            collection.Add(item);
            
            if (collection.Count > oldCount)
            {
                OnCollectionChanged();
            }
        }
        
        public void AddRange(IEnumerable<TValue> itemsToAdd)
        {
            int oldCount = collection.Count;
            
            foreach (var item in itemsToAdd)
            {
                collection.Add(item);
            }

            if (collection.Count > oldCount)
            {
                OnCollectionChanged();
            }
        }

        public void AddRange(params TValue[] itemsToAdd) => AddRange((IEnumerable<TValue>)itemsToAdd);

        public bool Remove(TValue item)
        {
            bool wasSuccessful = collection.Remove(item);
            
            if (wasSuccessful)
            {
                OnCollectionChanged();
            }
            
            NotifyInvolved();
            
            return wasSuccessful;
        }

        public void Clear()
        {
            if (collection.Count > 0)
            {
                collection.Clear();
                OnCollectionChanged();
            }
        }
        
        public IEnumerator<TValue> GetEnumerator()
        {
            foreach (TValue element in collection)
            {
                NotifyInvolved();
                yield return element;
            }
            
            //- TODO : Do we really want to call NotifyInvolved() on every iteration?
        }

        public bool Contains(TValue item)             => Collection.Contains(item);
        public void CopyTo(TValue[] array, int index) => Collection.CopyTo(array, index);
        public void CopyTo(Array array,    int index) => ((ICollection)Collection).CopyTo(array, index);

        #endregion

        
        #region Constructors
        
        public ObservedCollectionCore(TCollection initialValue) : base()
        {
            collection = initialValue;
        }

        #endregion
        

        //- TODO : We should implement a mechanic where if a collection of factors updates,
        //         the message it sends to invalidate its dependents should include what action
        //         was taken on the collection (Add, Remove, etc) an we should make Reactive 
        //         collections that depend on them handle those different cases.  This might
        //         simplify the work we have to do for enabling Recycling considerably.
    }
}