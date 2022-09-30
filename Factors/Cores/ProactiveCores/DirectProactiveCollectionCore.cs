using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.States;
using static Factors.TriggerFlags;

namespace Factors.Cores.ProactiveCores
{
    public abstract class DirectProactiveCollectionCore<TCollection, TValue>  : ProactorCore
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

        public Channel<TValue> ItemWasAdded   { get; } = new Channel<TValue>();
        public Channel<TValue> ItemWasRemoved { get; } = new Channel<TValue>();

        #endregion


        #region Instance Methods

        public void NotifyChanged() => Observer.NotifyChanged(Callback);
        
        protected void OnCollectionChanged(long triggerFlags)
        {
            NotifyChanged();
            Callback?.CoreUpdated(this, triggerFlags);
        }
        
        protected void OnItemAdded(TValue itemAdded)
        {
            ItemWasAdded.Send(itemAdded);
        }
        
        protected void OnMultipleItemsAdded(IEnumerable<TValue> itemsAdded, long triggerFlags)
        {
            OnCollectionChanged(triggerFlags);
            ItemWasAdded.Send(itemsAdded);
        }

        protected void OnItemRemoved(TValue itemRemoved)
        {
            ItemWasRemoved.Send(itemRemoved);
        }
        
        protected void OnItemReplaced(TValue oldItem, TValue newItem, long triggerFlags)
        {
            OnCollectionChanged(triggerFlags);
            ItemWasRemoved.Send(oldItem);
            ItemWasAdded.Send(newItem);
        }

        public bool Add(TValue item)
        {
            if (AddItem(item, out long involveFlags, out long additionalChangeFlags))
            {
                OnItemAdded(item);
                OnCollectionChanged(ItemAdded | additionalChangeFlags);
                
                return involveFlags is not TriggerFlags.None;
            }
            else return false;
        }

        protected abstract bool AddItem(TValue itemToAdd, out long notifyInvolvedFlags, out long additionalChangeFlags);
        
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
                }
            }

            if (numberOfItemsAdded > 0)
            {
                OnMultipleItemsAdded(itemsAsList, triggerFlags);
            }
        }
        
        public void AddRange(params TValue[] itemsToAdd) => AddRange((IEnumerable<TValue>)itemsToAdd);
        
        public bool Remove(TValue item)
        {
            bool wasSuccessful = RemoveItem(item, out long additionalTriggerFlags);
            
            if (wasSuccessful)
            {
                OnItemRemoved(item);
                OnCollectionChanged(ItemRemoved | additionalTriggerFlags);
            }

            // Callback.NotifyInvolved(TriggerWhenItemAdded);
           
            //^ Should we even notify here?  
            //  Do we think someone will try to bind to this?
            //  What flags do we use if they succeed in removing an item?
            //- I suppose you could use this to make a reaction that ensured
            //  a particular item wasn't in the collection by trying to remove
            //  it every time an item was added.  That strategy doesnt account
            //  for multiple additions though.

            return wasSuccessful;
        }

        protected abstract bool RemoveItem(TValue item, out long additionalTriggerFlags);

        public void Clear()
        {
            if (collection.Count > 0)
            {
                //?
                collection.Clear();
                throw new NotImplementedException();
            }
        }
        
        public IEnumerator<TValue> GetEnumerator()
        {
            foreach (TValue element in collection)
            {
                yield return element;
            }
        }

        public bool Contains(TValue item) => Collection.Contains(item);
        
        public void CopyTo(TValue[] array, int index) => Collection.CopyTo(array, index);
        
        public void CopyTo(Array    array, int index) => ((ICollection)Collection).CopyTo(array, index);

        #endregion

        
        #region Constructors
        
        public DirectProactiveCollectionCore(TCollection initialValue)
        {
            collection = initialValue;
        }

        #endregion
    }
}