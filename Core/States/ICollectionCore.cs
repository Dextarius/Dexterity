using System.Collections.Generic;
using Core.Factors;

namespace Core.States
{
    public interface ICollectionCore<TValue> : IFactorCore, IEnumerable<TValue>  
    {
        bool CollectionEquals(IEnumerable<TValue> collectionToCompare);
    }
}