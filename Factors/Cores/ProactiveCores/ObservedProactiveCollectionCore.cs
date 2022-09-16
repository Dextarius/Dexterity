using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.States;
using static Factors.TriggerFlags;

namespace Factors.Cores.ProactiveCores
{
    public abstract class ObservedProactiveCollectionCore<TCollection, TValue> : 
        ObservedProactorCore, IProactiveCollectionCore<TValue>
            where TCollection : ICollection<TValue>
    {
        #region Instance Fields

        protected TCollection collection;
        
        #endregion
        
        #region Properties

        public int  Count      => Collection.Count;
        public bool IsReadOnly => false;

        protected TCollection Collection
        {
            get => collection;
            set => collection = value; 
        }

        #endregion


        #region Instance Methods
        
        protected void OnItemAdded(TValue itemAdded, long triggerFlags)
        {
            OnCollectionChanged(triggerFlags);
           // ItemWasAdded.Send(itemAdded);
        }
        
        protected void OnMultipleItemsAdded(IEnumerable<TValue> itemsAdded, long triggerFlags)
        {
            OnCollectionChanged(triggerFlags);
           // ItemWasAdded.Send(itemsAdded);
        }
        
        protected void OnRangeOfItemsAdded(int startingIndex, int count, long triggerFlags)
        {
            for (int i = startingIndex; i < count; i++)
            {
                //- BroadcastItemAdded(item);

            }

            OnCollectionChanged(triggerFlags);
        }
        
        protected void OnItemRemoved(TValue itemRemoved, long triggerFlags)
        {

        }
        
        protected void OnItemReplaced(TValue oldItem, TValue newItem, long triggerFlags)
        {
            OnCollectionChanged(triggerFlags);
           // ItemWasRemoved.Send(oldItem);
           // ItemWasAdded.Send(newItem);

        }
        
        protected void OnCollectionChanged(long triggerFlags)
        {
            NotifyChanged();
            Callback?.CoreUpdated(this, triggerFlags);
        }
        
        public bool Add(TValue item)
        {
            if (AddItem(item, out long involveFlags, out long additionalChangeFlags))
            {
                OnCollectionChanged(ItemAdded | additionalChangeFlags);
                NotifyInvolved(TriggerWhenItemRemoved | involveFlags);

                return involveFlags is not TriggerFlags.None;
            }
            else return false;
        }

        protected abstract bool AddItem(TValue item, out long notifyInvolvedFlags, out long additionalChangeFlags);
        
        public void AddRange(IEnumerable<TValue> itemsToAdd)
        {
            int  numberOfItemsAdded = 0;
            long triggerFlags       = ItemAdded;
            
            foreach (var item in itemsToAdd)
            {
                if (AddItem(item, out _, out long additionalChangeFlags))
                {
                    numberOfItemsAdded++;
                    triggerFlags |= additionalChangeFlags;
                }
            }

            if (numberOfItemsAdded > 0)
            {
                OnCollectionChanged(triggerFlags);
            }
        }

        public void AddRange(params TValue[] itemsToAdd) => AddRange((IEnumerable<TValue>)itemsToAdd);

        public bool Remove(TValue itemToRemove)
        {
            bool wasSuccessful = RemoveItem(itemToRemove, out long additionalTriggerFlags);
            
            if (wasSuccessful)
            {
                OnCollectionChanged(ItemRemoved | additionalTriggerFlags);
            }

            NotifyInvolved(TriggerWhenItemAdded);
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
                collection.Clear();
                OnCollectionChanged(ItemRemoved);
            }
        }
        
        public IEnumerator<TValue> GetEnumerator()
        {
            foreach (TValue element in collection)
            {
                yield return element;
            }
            
            //- TODO : Do we want to call NotifyInvolved() on every iteration?
        }

        public bool Contains(TValue item)
        {
            bool itemIsPresent = Collection.Contains(item);
            
            NotifyInvolved_ContainsItem(itemIsPresent);

            return itemIsPresent;
        }
        
        public void CopyTo(TValue[] array, int index) => Collection.CopyTo(array, index);
        public void CopyTo(Array array,    int index) => ((ICollection)Collection).CopyTo(array, index);

        
        protected void NotifyInvolved_ContainsItem(bool collectionContainsItem)
        {
            if (collectionContainsItem)
            {
                NotifyInvolved(TriggerWhenItemRemoved | TriggerWhenItemReplaced);
            }
            else
            {
                NotifyInvolved(TriggerWhenItemAdded | TriggerWhenItemReplaced);
            }
        }

        protected void NotifyInvolved_IndexOf(int itemsIndex)
        {
            if (itemsIndex < 0)
            {
                NotifyInvolved(TriggerWhenItemAdded | TriggerWhenItemReplaced);
            }
            else
            {
                NotifyInvolved(TriggerWhenItemRemoved | TriggerWhenItemReplaced | TriggerWhenItemMoved);
            }
        }
        
        protected void NotifyInvolved_LastIndexOf(int itemsIndex)
        {
            if (itemsIndex < 0)
            {
                NotifyInvolved(TriggerWhenItemAdded | TriggerWhenItemReplaced);
            }
            else
            {
                NotifyInvolved(TriggerWhenItemAdded    | TriggerWhenItemRemoved | 
                               TriggerWhenItemReplaced | TriggerWhenItemMoved);
            }
        }
        
        protected void NotifyInvolved_IterateAll()
        {
            NotifyInvolved(TriggerWhenItemAdded | TriggerWhenItemRemoved | TriggerWhenItemReplaced);
        }
        
        protected void NotifyInvolved_Rearranged()
        {
            NotifyInvolved(TriggerWhenItemRemoved | TriggerWhenItemReplaced);
        }

        public abstract bool CollectionEquals(IEnumerable<TValue> collectionToCompare);
        
        #endregion

        
        #region Constructors

        protected ObservedProactiveCollectionCore(TCollection initialValue)
        {
            collection = initialValue;
        }

        #endregion


        #region Explicit Implementations

        void ICollection<TValue>.Add(TValue item) => Add(item);
        IEnumerator IEnumerable. GetEnumerator()  => GetEnumerator();

        #endregion

        //- TODO : We should implement a mechanic where if a collection of factors updates,
        //         the message it sends to invalidate its dependents should include what action
        //         was taken on the collection (Add, Remove, etc) an we should make Reactive 
        //         collections that depend on them handle those different cases.  This might
        //         simplify the work we have to do for enabling Recycling considerably.
    }
}