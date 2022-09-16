﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Factors;
using static Factors.TriggerFlags;

namespace Factors.Collections
{
    public interface ICollectionOwner<TCollection, TValue> : IInvolved
        where TCollection : ICollection<TValue>
    {
        void OnItemAdded(TValue itemAdded);
        void OnMultipleItemsAdded(IEnumerable<TValue> itemsAdded);
        void OnMultipleItemsRemoved(IEnumerable<TValue> itemsAdded);
        void OnRangeOfItemsAdded(int startingIndex, int count);
        void OnRangeOfItemsRemoved(int startingIndex, int count);
        void OnItemRemoved(TValue itemRemoved);
        void OnCollectionChanged(long triggerFlags);
    }
    
    public abstract class CollectionImplementer<TCollection, TValue> : ICollection<TValue>, ICollection, IInvolved
        where TCollection : ICollection<TValue>
    {
        #region Instance Fields

        protected TCollection collection;
        
        #endregion
        
        #region Properties

        public ICollectionOwner<TCollection, TValue> Owner { get; }

        public bool   IsSynchronized { get; }
        public object SyncRoot       { get; }
        public bool   IsReadOnly     => false;

        protected TCollection Collection
        {
            get => collection;
            set => collection = value; 
        }
        
        public int Count
        {
            get
            {
                int count = Collection.Count;
                
                NotifyInvolved_NumberOfItems();
                return count;
            }
        }

        #endregion


        #region Instance Methods

        public void NotifyInvolved(long triggerFlags) => Owner.NotifyInvolved(triggerFlags);
        
        public void NotifyInvolved()                  => NotifyInvolved(TriggerFlags.Default);

        public bool Add(TValue itemToAdd)
        {
            if (AddItem(itemToAdd, out long involveFlags, out long additionalChangeFlags))
            {
                Owner.OnItemAdded(itemToAdd);
                Owner.OnCollectionChanged(ItemAdded   | additionalChangeFlags);
                NotifyInvolved(TriggerWhenItemRemoved | TriggerWhenItemReplaced | involveFlags);

                return true;
            }
            else return false;
        }

        protected abstract bool AddItem(TValue itemToAdd, out long additionalNotifyFlags, out long additionalChangeFlags);
        
        public void AddRange(IEnumerable<TValue> itemsToAdd)
        {
            int  numberOfItemsAdded = 0;
            long triggerFlags       = ItemAdded;
            var  itemsAsList        = itemsToAdd.ToList();

            foreach (var item in itemsAsList)
            {
                if (AddItem(item, out _, out long additionalChangeFlags))
                {
                    numberOfItemsAdded++;
                    triggerFlags |= additionalChangeFlags;
                    Owner.OnItemAdded(item);
                }
            }

            if (numberOfItemsAdded > 0)
            {
                Owner.OnCollectionChanged(triggerFlags);
            }
        }

        public void AddRange(params TValue[] itemsToAdd) => AddRange((IEnumerable<TValue>)itemsToAdd);

        public bool Remove(TValue itemToRemove)
        {
            bool wasSuccessful = RemoveItem(itemToRemove, out long additionalTriggerFlags);
            
            if (wasSuccessful)
            {
               Owner.OnItemRemoved(itemToRemove);
               Owner.OnCollectionChanged(ItemRemoved | additionalTriggerFlags);
            }

            NotifyInvolved(TriggerWhenItemAdded | TriggerWhenItemReplaced);

            //^ Should we even notify here?  
            //  Do we think someone will try to bind to this?
            //  What flags do we use if they succeed in removing an item?
            //- I suppose you could use this to make a reaction that ensured
            //  a particular item wasn't in the collection by trying to remove
            //  it every time an item was added.  That strategy doesnt account
            //  for multiple additions though.

            return wasSuccessful;
        }
        
        protected abstract bool RemoveItem(TValue itemToRemove, out long additionalTriggerFlags);
        
        public void Clear()
        {
            if (collection.Count > 0)
            {
                var copyOfCollection = Collection.ToList();
                
                collection.Clear();
                Owner.OnMultipleItemsRemoved(copyOfCollection);
                Owner.OnCollectionChanged(ItemRemoved);
            }
        }

        public bool Contains(TValue item)
        {
            bool itemIsPresent = Collection.Contains(item);
            
            NotifyInvolved_ContainsItem(itemIsPresent);
            return itemIsPresent;
        }
        
        public void CopyTo(TValue[] array, int index)
        {
            Collection.CopyTo(array, index);
            NotifyInvolved_All();
        }
        
        public void CopyTo(Array array, int index)
        {
            ((ICollection) Collection).CopyTo(array, index);
            NotifyInvolved_All();
        }
        
        protected void NotifyInvolved_ContainsItem(bool collectionContainsItem)
        {
            if (collectionContainsItem) { NotifyInvolved(TriggerWhenItemReplaced | TriggerWhenItemRemoved); }
            else                        { NotifyInvolved(TriggerWhenItemReplaced | TriggerWhenItemAdded);   }
        }
        
        protected void NotifyInvolved_IndexOf(int itemsIndex)
        {
            if (itemsIndex >= 0) { NotifyInvolved(TriggerWhenItemReplaced | TriggerWhenItemRemoved | TriggerWhenItemMoved); }
            else                 { NotifyInvolved(TriggerWhenItemReplaced | TriggerWhenItemAdded);                          }
                            
        }

        protected void NotifyInvolved_LastIndexOf(int itemsIndex)
        {
            if (itemsIndex >= 0) { NotifyInvolved(TriggerWhenItemReplaced | TriggerWhenItemAdded |
                                                  TriggerWhenItemRemoved  | TriggerWhenItemMoved); }
            
            else                 { NotifyInvolved(TriggerWhenItemReplaced | TriggerWhenItemAdded); }
        }

        protected void NotifyInvolved_NumberOfItems() => NotifyInvolved(TriggerWhenItemRemoved | TriggerWhenItemAdded);
        
        
        protected void NotifyInvolved_Rearranged()    => NotifyInvolved(TriggerWhenItemRemoved | TriggerWhenItemReplaced);
        
        protected void NotifyInvolved_Iterator()      => NotifyInvolved(TriggerWhenItemRemoved | TriggerWhenItemReplaced | 
                                                                        TriggerWhenItemAdded); 
        
        protected void NotifyInvolved_All()           => NotifyInvolved(TriggerWhenItemRemoved | TriggerWhenItemReplaced | 
                                                                        TriggerWhenItemAdded   | TriggerWhenItemMoved);

        public IEnumerator<TValue> GetEnumerator()
        {
            NotifyInvolved_Iterator();
                
            foreach (TValue element in collection)
            {
                yield return element;
            }
            
            //- TODO : Do we want to call NotifyInvolved() on every iteration?
        }

        public abstract bool CollectionEquals(IEnumerable<TValue> collectionToCompare);
        
        #endregion

        
        #region Constructors

        protected CollectionImplementer(TCollection initialValue, ICollectionOwner<TCollection, TValue> owner)
        {
            collection = initialValue;
            Owner      = owner;
        }

        #endregion


        #region Explicit Implementations

        void ICollection<TValue>.Add(TValue itemToAdd) => Add(itemToAdd);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        //- TODO : We should implement a mechanic where if a collection of factors updates,
        //         the message it sends to invalidate its dependents should include what action
        //         was taken on the collection (Add, Remove, etc) an we should make Reactive 
        //         collections that depend on them handle those different cases.  This might
        //         simplify the work we have to do for enabling Recycling considerably.
    }

}