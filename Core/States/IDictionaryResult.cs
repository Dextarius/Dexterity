using System;
using System.Collections;
using System.Collections.Generic;
using Core.Factors;

namespace Core.States
{
    public interface IDictionaryResult<TKey, TValue> : ICollectionResult<KeyValuePair<TKey, TValue>>
    {
        ICollection<TKey>   Keys           { get; }
        ICollection<TValue> Values         { get; }
        TValue              this[TKey key] { get; }

        new IDictionaryEnumerator    GetEnumerator();
            Dictionary<TKey, TValue> AsNormalDictionary();
            bool                     TryGetValue(TKey key, out TValue value);
            bool                     ContainsKey(TKey key);
            bool                     ContainsValue(TValue key);
            ICollection              GetKeysAsICollection();
            ICollection              GetValuesAsICollection();
    }
}