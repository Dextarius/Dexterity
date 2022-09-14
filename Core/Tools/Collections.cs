using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Core.Tools
{
    public static class Collections
    {
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
        
        public static int BinarySearch<T>(IReadOnlyList<T> items, int startIndex, int length, T value, IComparer<T> comparer)
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
        
        public static Dictionary<TKey, TValue> CreateNewDictionary<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> entries, IEqualityComparer<TKey> comparerForKeys)
        {
            var createdDictionary = new Dictionary<TKey, TValue>(comparerForKeys);

            foreach (var keyValuePair in entries)
            {
                createdDictionary[keyValuePair.Key] = keyValuePair.Value;
            }
            
            return createdDictionary;
        }
    }
}