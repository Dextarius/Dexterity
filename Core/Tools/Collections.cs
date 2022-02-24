using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

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

        public static T[] ExpandArray<T>(T[] originalArray)
        {
            if (originalArray is null) { throw new ArgumentNullException(nameof(originalArray)); }

            return ExpandArray(originalArray, originalArray.Length * 2);
        }

        public static T[] ExpandArray<T>(T[] originalArray, int newSize)
        {
            if (originalArray is null) { throw new ArgumentNullException(nameof(originalArray)); }
            
            var newArray = new T[newSize];
        
            Array.Copy(originalArray, newArray, originalArray.Length);
            return newArray;
        }

        public static T[] ExpandArrayToAtLeast<T>(T[] originalArray, int newSize)
        {
            if (originalArray is null) { throw new ArgumentNullException(nameof(originalArray)); }

            var doubleCurrentLength = originalArray.Length * 2;
            
            if (newSize < doubleCurrentLength)
            {
                newSize = doubleCurrentLength;
            }

            return ExpandArray(originalArray, newSize);
        }
        
        public static void Add<T>(ref T[] arrayReference, T item, int index)
        {
            if (index < 0) { throw new ArgumentException(
                $"A process attempted to add an element to an array, but provided a negative index.  Index Given => {index} "); }
            
            var  modifiedArray = arrayReference;
            bool arrayWasReplaced = false;

            if (modifiedArray is null  ||  modifiedArray.Length == 0)
            {
                modifiedArray = new T[index + 1];
                arrayWasReplaced = true;
            }
            else if (index >= modifiedArray.Length)
            {
                int newSize = modifiedArray.Length * 2;
                
                if (index > newSize - 1)
                {
                    newSize = index;
                }

                modifiedArray = ExpandArray(modifiedArray, newSize);
                arrayWasReplaced = true;
            }

            modifiedArray[index] = item;

            if (arrayWasReplaced)
            {
                arrayReference = modifiedArray;
            }
        }
        
        static int BinarySearch<T>(T[] items, int startIndex, int length, T value, IComparer<T> comparer)
        {
            int start = startIndex;
            int end   = (startIndex + length) - 1;
            
            while (start <= end)
            {
                int mid    = start + ((end - start) / 2);
                int result = comparer.Compare(items[mid], value);
                
                if      (result == 0) { return  mid;     }
                else if (result  < 0) { start = mid + 1; }
                else                  { end   = mid - 1; }
            }
            
            return ~start;
        }

        public static bool HasSameKeysAndValuesAs<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary1, 
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
        
    }
}