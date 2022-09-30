using System.Collections;
using System.Collections.Generic;

namespace Factors.Collections
{
    public interface IDictionaryImplementer<TKey, TValue> : ICollectionImplementer<KeyValuePair<TKey, TValue>>
    {
        TValue              this[TKey key] { get; set; }
        ICollection<TKey>   Keys           { get; }
        ICollection<TValue> Values         { get; }
        bool                IsFixedSize    { get; }

        void        Add(TKey key, TValue value);
        bool        Remove(TKey key);
        bool        TryGetValue(TKey key, out TValue value);
        bool        ContainsKey(TKey keyToLookFor);
        bool        ContainsValue(TValue valueToLookFor);
        ICollection GetKeysAsICollection();
        ICollection GetValuesAsICollection();
        
        new IDictionaryEnumerator GetEnumerator();
    }
}