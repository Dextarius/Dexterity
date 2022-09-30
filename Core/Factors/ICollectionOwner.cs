using System.Collections.Generic;
using Core.Factors;

namespace Factors.Collections
{
    public interface ICollectionOwner<in TValue> : IInvolved
    {
        void OnItemAdded(TValue itemAdded);
        void OnMultipleItemsAdded(IEnumerable<TValue> itemsAdded);
        void OnMultipleItemsRemoved(IEnumerable<TValue> itemsAdded);
        void OnItemRemoved(TValue itemRemoved);
        void OnCollectionChanged(long triggerFlags);
    }
    
    public interface IListOwner<in TValue> : ICollectionOwner<TValue>
    {
        void OnRangeOfItemsAdded(int startingIndex, int count);
    }

    public interface IDictionaryOwner<TKey, TValue> : ICollectionOwner<KeyValuePair<TKey, TValue>>
    {
        //- This interface is just here for readability/clarity.
    }
}