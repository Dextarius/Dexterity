using System;
using System.Collections;
using System.Collections.Generic;
using Core.Factors;

namespace Core.States
{
    public interface ICollectionCore<TValue> : IDeterminant
    {
        int Count { get; }
        
        void Add(TValue item);
        void AddRange(IEnumerable<TValue> itemsToAdd);
        void AddRange(params TValue[] itemsToAdd);
        bool Remove(TValue item);
        void Clear();
        bool Contains(TValue item);
        void CopyTo(TValue[] array, int index);
        void CopyTo(Array array, int index);
        
        IEnumerator<TValue> GetEnumerator();
    }
}