using System;
using System.Collections.Generic;

namespace Factors
{
    public static class CollectionFactor
    {
        // #region Constants
        //
        // public const long
        //     ItemAddedFlag          =  0b_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0001L,
        //     ItemRemovedFlag        =  0b_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0010L,
        //     ItemMovedFlag          =  0b_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0100L,
        //     ItemReplacedFlag       =  0b_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_1000L,
        //     ItemsToLeftChangedFlag = -0b_1000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000L,
        //     IndexMask              =  0b_0111_1111_1111_1111_1111_1111_1111_1111_0000_0000_0000_0000_0000_0000_0000_0000L,
        //
        //     TriggerFlags.ItemAdded          = ItemAddedFlag,
        //     TriggerFlags.ItemRemoved        = ItemRemovedFlag,
        //     TriggerFlags.ItemMoved          = ItemMovedFlag,
        //     TriggerFlags.ItemReplaced       = ItemReplacedFlag,
        //     TriggerOnItemsToLeftChanged = ItemsToLeftChangedFlag;
        //     
        //
        // #endregion
        
        
        public static bool HaveSameKeysAndValues<TKey, TValue>(IDictionary<TKey, TValue> newDictionary, 
                                                               IDictionary<TKey, TValue> oldDictionary, 
                                                               IEqualityComparer<TValue> valueComparer,
                                                           out long                      triggerFlags)
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
        
        public static bool HaveSameItems<T>(ISet<T>  newSet, 
                                            ISet<T>  oldSet,
                                            out long triggerFlags)
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
                var unmatchedItems = MakeDictionary(oldList, itemComparer);

                for (int i = 0; i < newList.Count; i++)
                {
                    var newListsItem = newList[i];
                    var oldListsItem = oldList[i];

                    if (itemComparer.Equals(newListsItem, oldListsItem))
                    {

                        if (unmatchedItems.TryGetValue(oldListsItem, out var numberOfInstances))
                        {
                            if (numberOfInstances > 1)
                            {
                                unmatchedItems[oldListsItem] = numberOfInstances - 1;
                            }
                            else
                            {
                                unmatchedItems.Remove(oldListsItem);
                            }
                        }
                        else
                        {
                            //- This shouldn't happen
                            throw new InvalidOperationException();
                        }
                    }
                    else
                    {
                        if (unmatchedItems.TryGetValue(oldListsItem, out var numberOfInstances))
                        {
                            if (numberOfInstances > 1)
                            {
                                unmatchedItems[oldListsItem] = numberOfInstances - 1;
                            }
                            else
                            {
                                unmatchedItems.Remove(oldListsItem);
                            }

                            triggerFlags |= TriggerFlags.ItemMoved;
                        }
                        else
                        {
                            triggerFlags |= TriggerFlags.ItemAdded;
                        }
                        
                    }
                }

                if (unmatchedItems.Count > 0)
                {
                    triggerFlags |= TriggerFlags.ItemRemoved;
                }
            }
            
            return triggerFlags is TriggerFlags.None;
            
            //- Later : Consider if we should return false if the dictionaries use different equality
            //          comparers for their keys.
        }

        public static Dictionary<T, int> MakeDictionary<T>(IList<T> list, IEqualityComparer<T> comparer)
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