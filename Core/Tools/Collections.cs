using System.Collections.Generic;

namespace Core.Tools
{
    public static class Collections
    {
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> valuesToPutInSet)
        {
            HashSet<T> createdSet;
            
            

            if (valuesToPutInSet != null)
            {
                createdSet = new HashSet<T>();
                
                foreach (T value in valuesToPutInSet)
                {
                    createdSet.Add((value));
                }
            }
            else
            {
                createdSet = null;
            }

            return createdSet;
        }
        
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable< KeyValuePair<TKey, TValue>> keyValuePairs)
        {
            Dictionary<TKey, TValue> createdDictionary;
            
            if (keyValuePairs != null)
            {
                createdDictionary = new Dictionary<TKey, TValue>();
                
                foreach (KeyValuePair<TKey, TValue> pair in keyValuePairs)
                {
                    createdDictionary[pair.Key] = pair.Value;
                }
            }
            else
            {
                createdDictionary = null;
            }

            return createdDictionary;
        }
    }
}