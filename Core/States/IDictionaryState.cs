using System;
using System.Collections;
using System.Collections.Generic;

namespace Core.States
{
    public interface IDictionaryState<TKey, TValue> : ICollectionState<KeyValuePair<TKey, TValue>>
    {
        ICollection<TKey>   Keys           { get; }
        ICollection<TValue> Values         { get; }
        TValue              this[TKey key] { get; set; }
        
        void        Add(TKey key, TValue value);
        bool        Remove(TKey key);
        bool        ContainsKey(TKey key);
        bool        ContainsValue(TValue key);
        bool        TryGetValue(TKey key, out TValue value);
        ICollection GetKeysAsICollection();
        ICollection GetValuesAsICollection();
        
        new IDictionaryEnumerator GetEnumerator();

    }
}