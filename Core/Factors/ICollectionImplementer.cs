using System.Collections;
using System.Collections.Generic;
using Core.Factors;

namespace Factors.Collections
{
    public interface ICollectionImplementer<TValue> : ICollection<TValue>, ICollection, IInvolved 
    {
        new int Count { get; }

        new bool Add(TValue item);
            void AddRange(IEnumerable<TValue> itemsToAdd);
            void AddRange(params TValue[] itemsToAdd);
            bool CollectionEquals(IEnumerable<TValue> collectionToCompare); 
    }
}