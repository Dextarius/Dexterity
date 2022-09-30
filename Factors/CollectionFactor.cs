using System;
using System.Collections.Generic;

namespace Factors
{
    public static class CollectionFactor
    {
        public static bool HaveSameKeysAndValues<TKey, TValue>(IDictionary<TKey, TValue> newDictionary, 
                                                               IDictionary<TKey, TValue> oldDictionary, 
                                                               IEqualityComparer<TValue> valueComparer,
                                                               out long                  triggerFlags)
        {
            triggerFlags = TriggerFlags.None;
        
            if (newDictionary != oldDictionary)
            {
                int numberOfMatchingKeys = 0;
                
                foreach (KeyValuePair<TKey, TValue> newDictionaryKeyValuePair in newDictionary)
                {
                    TKey newDictionarysKey = newDictionaryKeyValuePair.Key;
        
                    if (oldDictionary.TryGetValue(newDictionarysKey, out var oldDictionarysValue))
                    {
                        TValue newDictionarysValue = newDictionaryKeyValuePair.Value;
        
                        numberOfMatchingKeys++;
        
                        if (valueComparer.Equals(newDictionarysValue, oldDictionarysValue) is false)
                        {
                            triggerFlags |= TriggerFlags.ItemReplaced;
                        }
                    }
                    else
                    {
                        triggerFlags |= TriggerFlags.ItemAdded;
                    }
                }
        
                if (numberOfMatchingKeys != newDictionary.Count)
                {
                    triggerFlags |= TriggerFlags.ItemRemoved;
                }
        
            }
            
            return triggerFlags is TriggerFlags.None;
            
            //- Later : Consider if we should return false if the dictionaries use different equality
            //          comparers for their keys.
        }
        
        public static bool HaveSameItems<T>(ISet<T>  newSet, ISet<T>  oldSet, out long triggerFlags)
        {
            triggerFlags = TriggerFlags.None;
        
            if (newSet != oldSet)
            {
                int numberOfMatchingItems = 0;
                
                foreach (var item in newSet)
                {
                    if (oldSet.Contains(item))
                    {
                        numberOfMatchingItems++;
                    }
                    else
                    {
                        triggerFlags |= TriggerFlags.ItemAdded;
                    }
                }
        
                if (numberOfMatchingItems != newSet.Count)
                {
                    triggerFlags |= TriggerFlags.ItemRemoved;
                }
        
            }
            
            return triggerFlags is TriggerFlags.None;
            
            //- Later : Consider if we should return false if the dictionaries use different equality
            //          comparers for their keys.
        }

        public static bool ListsAreEquivalent<T>(IList<T>             newList, 
                                                 IList<T>             oldList,
                                                 IEqualityComparer<T> itemComparer,
                                                 out long             triggerFlags)
        {
            triggerFlags = TriggerFlags.None;
        
            if (newList != oldList)
            {
                var unmatchedItems = GetNumberOfInstancesForEachItemIn(oldList, itemComparer);

                for (int i = 0;  i < newList.Count;  i++)
                {
                    var  newListsItem = newList[i];
                    bool newListsItemWasAlsoInOldList = unmatchedItems.TryGetValue(newListsItem, out var numberOfInstances);
                    
                    if (newListsItemWasAlsoInOldList)
                    {
                        bool currentIndexExistsInOldList = i < oldList.Count;

                        if (currentIndexExistsInOldList)
                        {
                            var  oldListsItem       = oldList[i];
                            bool itemWasAtSameIndex = itemComparer.Equals(newListsItem, oldListsItem);
                        
                            if (itemWasAtSameIndex is false)
                            {
                                triggerFlags |= TriggerFlags.ItemMoved;
                            }
                        
                            if (numberOfInstances > 1) { unmatchedItems[oldListsItem] = numberOfInstances - 1; }
                            else                       { unmatchedItems.Remove(oldListsItem);                  }
                        }
                        else
                        {
                            triggerFlags |= TriggerFlags.ItemMoved;
                        }
                    }
                    else
                    {
                        triggerFlags |= TriggerFlags.ItemAdded;
                    }
                }

                if (unmatchedItems.Count > 0)
                {
                    triggerFlags |= TriggerFlags.ItemRemoved;
                }
            }
            
            return triggerFlags is TriggerFlags.None;
        }

        public static Dictionary<T, int> GetNumberOfInstancesForEachItemIn<T>(IList<T> list, IEqualityComparer<T> comparer)
        {
            var createdDictionary = new Dictionary<T, int>(comparer);
                
            for (int i = 0; i < list.Count; i++)
            {
                var currentItem = list[i];

                if (createdDictionary.TryGetValue(currentItem, out var numberOfInstances))
                {
                    createdDictionary[currentItem] = numberOfInstances + 1;
                }
                else
                {
                    createdDictionary[currentItem] = 1;
                }
            }

            return createdDictionary;
        }
        

    }
    
    
    
}