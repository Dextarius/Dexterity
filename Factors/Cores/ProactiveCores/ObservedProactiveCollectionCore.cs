using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.States;
using Factors.Collections;
using static Factors.TriggerFlags;

namespace Factors.Cores.ProactiveCores
{
    public abstract class ObservedProactiveCollectionCore<TCollection, TValue, TImplementer> : 
        ObservedProactorCore, IProactiveCollectionCore<TValue>, ICollectionOwner<TValue>
            where TCollection  : ICollection<TValue>
            where TImplementer : ICollectionImplementer<TValue>
    {
        #region Instance Fields
        
        protected TImplementer implementer;
        
        #endregion
        
        #region Properties

        public int             Count          => implementer.Count;
        public bool            IsReadOnly     => implementer.IsReadOnly;
        public Channel<TValue> ItemWasAdded   { get; } = new Channel<TValue>();
        public Channel<TValue> ItemWasRemoved { get; } = new Channel<TValue>();

        #endregion


        #region Instance Methods

        public bool                Add(TValue item)                         => implementer.Add(item);
        public void                AddRange(IEnumerable<TValue> itemsToAdd) => implementer.AddRange(itemsToAdd);
        public void                AddRange(params TValue[] itemsToAdd)     => AddRange((IEnumerable<TValue>)itemsToAdd);
        public bool                Remove(TValue itemToRemove)              => implementer.Remove(itemToRemove);
        public void                Clear()                                  => implementer.Clear();
        public bool                Contains(TValue item)                    => implementer.Contains(item);
        public void                CopyTo(TValue[] array, int index)        => implementer.CopyTo(array, index);
        public void                CopyTo(Array array,    int index)        => implementer.CopyTo(array, index);
        public IEnumerator<TValue> GetEnumerator()                          => implementer.GetEnumerator();

        public bool CollectionEquals(IEnumerable<TValue> collectionToCompare) => 
            implementer.CollectionEquals(collectionToCompare);

        #endregion


        #region Explicit Implementations

        void ICollectionOwner<TValue>.OnItemAdded(TValue addedItem)
        {
            ItemWasAdded.Send(addedItem);
        }

        void ICollectionOwner<TValue>.OnMultipleItemsAdded(IEnumerable<TValue> addedItems)
        {
            ItemWasAdded.Send(addedItems);
        }

        void ICollectionOwner<TValue>.OnItemRemoved(TValue removedItem)
        {
            ItemWasRemoved.Send(removedItem);
        }

        void ICollectionOwner<TValue>.OnMultipleItemsRemoved(IEnumerable<TValue> removedItems)
        {
            ItemWasRemoved.Send(removedItems);
        }

        void ICollectionOwner<TValue>.OnCollectionChanged(long triggerFlags)
        {
            NotifyChanged();
            Callback?.CoreUpdated(this, triggerFlags);
        }
        
        void ICollection<TValue>.Add(TValue item) => Add(item);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }
}