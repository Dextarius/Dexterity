using System.Collections.Generic;

namespace Core.States
{
    public interface IDictionaryResult<TKey, TValue> : 
        ICollectionResult<Dictionary<TKey, TValue>, KeyValuePair<TKey, TValue>>
    {
        bool TryGetValue(TKey key, out TValue value);
        bool ContainsKey(TKey key);
        bool ContainsValue(TValue key);
    }
}