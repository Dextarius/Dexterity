using System.Collections.Generic;

namespace Core.Factors
{
    public interface ICollectionFactorSubscriber<TValue>
    {
        void   ItemAdded(IDirectCollectionFactor<TValue> collection, TValue itemAdded);
        void ItemRemoved(IDirectCollectionFactor<TValue> collection, TValue itemRemoved);
    }
    
    
    public interface IListFactorSubscriber<TValue>
    {
        void ItemAdded(IList<TValue> list, TValue itemAdded, int index);
        void ItemMoved(IList<TValue> list, TValue itemMoved, int oldIndex, int newIndex);
        void ItemRemoved(IList<TValue> list, TValue itemRemoved, int index);
        void ItemReplaced(IList<TValue> list, TValue itemReplaced, int index);
        void Rearranged(IList<TValue> list);
    }

    public interface IDirectCollectionFactor<TValue>
    {

    }

}