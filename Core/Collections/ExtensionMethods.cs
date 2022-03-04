using System.Collections.Generic;

namespace Core.Collections
{
    public static class ExtensionMethods
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
        
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(
            this IEnumerable< KeyValuePair<TKey, TValue>> keyValuePairs, IEqualityComparer<TKey> keyComparer = null)
        {
            Dictionary<TKey, TValue> createdDictionary;
            
            if (keyValuePairs != null)
            {
                createdDictionary = new Dictionary<TKey, TValue>(keyComparer);
                
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
        
        public static bool IsEquivalentTo<T>(this IList<T> list1, IList<T> list2, IEqualityComparer<T> elementComparer)
        {
            if (list1 == list2)
            {
                return true;
            }
            else if (list1 is null || list2 is null)
            {
                return false;
            }
            else if (list1.Count != list2.Count)
            {
                return false;
            }
            else
            {
                for (int i = 0; i < list1.Count; i++)
                {
                    if (elementComparer.Equals(list1[i], list2[i]) is false)
                    {
                        return false;
                    }
                }
                
                return true;
            }
        }
        
        public static bool HasSameKeysAndValuesAs<TKey, TValue>(this IDictionary<TKey, TValue> dictionary1, 
                                                                     IDictionary<TKey, TValue> dictionary2, 
                                                                     IEqualityComparer<TValue> valueComparer)
        {
            if (dictionary1 == dictionary2)
            {
                return true;
            }
            else if (dictionary1.Count != dictionary2.Count)
            {
                return false;
            }
            else
            {
                foreach (KeyValuePair<TKey, TValue> dictionary1KeyValuePair in dictionary1)
                {
                    TKey keyInDictionary1 = dictionary1KeyValuePair.Key;
                    
                    if (dictionary2.TryGetValue(keyInDictionary1, out var valueInCollection2) is false)
                    {
                        return false;
                    }
                    else
                    {
                        TValue valueInDictionary1 = dictionary1KeyValuePair.Value;

                        if (valueComparer.Equals(valueInDictionary1, valueInCollection2) is false)
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
            
            //- Later : Consider if we should return false if the dictionaries use different equality
            //          comparers for their keys.
        }
    }
    
    
    
    
}