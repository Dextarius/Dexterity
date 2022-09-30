using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Core.Tools;
using Dextarius.Collections;
using JetBrains.Annotations;
using static Factors.TriggerFlags;
using static Core.Tools.Types;

namespace Factors.Collections
{
    public class ListImplementer<T> : CollectionImplementer<List<T>, T, IListOwner<T>>, IListImplementer<T>
    {
        #region Instance Fields

        [NotNull]
        protected readonly IEqualityComparer<T> itemComparer;
        
        #endregion
        
        #region Instance Properties

        public T this[int index]
        {
            get
            {
                var currentValue = Collection[index];
                
                Owner.NotifyInvolved(TriggerWhenItemRemoved | TriggerWhenItemReplaced | TriggerWhenItemMoved);
                return currentValue;
            }
            set
            {
                var currentValue = collection[index];

                if (itemComparer.Equals(currentValue, value) is false)
                {
                    collection[index] = value;
                    Owner.OnItemAdded(value);
                    Owner.OnItemRemoved(currentValue);
                    Owner.OnCollectionChanged(ItemAdded | ItemRemoved | ItemReplaced);
                }
            }
        }

        public int Capacity => Collection.Capacity;
        //- Consider if we want to try to determine when this changes and notify.

        #endregion

        
        #region Instance Methods 
        
        protected override bool AddItem(T itemToAdd, out long additionalNotifyFlags, out long additionalChangeFlags)
        {
            collection.Add(itemToAdd);
            additionalChangeFlags = TriggerFlags.None;
            additionalNotifyFlags = TriggerFlags.None;
            return true;
        }
        
        protected override bool RemoveItem(T item, out long additionalTriggerFlags)
        {
            int itemsIndex = collection.IndexOf(item);
            int lastIndex  = collection.Count - 1;
            
            additionalTriggerFlags = TriggerFlags.None;

            if (itemsIndex < 0)
            {
                return false;
            }
            else
            {
                collection.RemoveAt(itemsIndex);

                if (itemsIndex != lastIndex)
                {
                    additionalTriggerFlags = ItemMoved | ItemReplaced;
                }
                
                return true;
            }
        }
        
        public void Insert(int index, T item)
        {
            long triggerFlags = ItemAdded;
            
            if (index != collection.Count)
            {
                triggerFlags |= ItemMoved | ItemReplaced;
            }
            
            collection.Insert(index, item);
            Owner.OnItemAdded(item);
            Owner.OnCollectionChanged(triggerFlags);
        }
        
        public void InsertRange(int index, IEnumerable<T> itemsToInsert)
        {
            long triggerFlags          = ItemAdded;
            int  numberOfItemsInserted = 0;
            
            if (index != collection.Count)
            {
                triggerFlags |= ItemMoved | ItemReplaced;
            }

            foreach (var item in itemsToInsert)
            {
                Collection[index + numberOfItemsInserted] = item;
                numberOfItemsInserted++;
            }

            if (numberOfItemsInserted > 0)
            {
                Owner.OnRangeOfItemsAdded(index, numberOfItemsInserted);
                Owner.OnCollectionChanged(triggerFlags);
            }
        }
        
        public void RemoveAt(int index)
        {
            long triggerFlags = ItemRemoved;
            var  removedItem  = Collection[index];
            
            if (index != collection.Count - 1)
            {
                triggerFlags |= ItemMoved | ItemReplaced;
            }
            
            collection.RemoveAt(index);
            Owner.OnItemRemoved(removedItem);
            Owner.OnCollectionChanged(triggerFlags);
        }
        
        public int RemoveAll(Predicate<T> shouldRemoveItem)
        {
            var  removedItems = new List<T>();

            for (int i = Count - 1; i >= 0; i--)
            {
                var currentItem = collection[i];
                
                if (shouldRemoveItem(currentItem))
                {
                    removedItems.Add(collection[i]);
                    collection.RemoveAt(i);
                }
            }
            
            if (removedItems.Count > 0)
            {
                long triggerFlags = ItemRemoved;

                if (this.Count > 0)
                {
                    triggerFlags |= ItemMoved | ItemReplaced;
                }
                
                Owner.OnMultipleItemsRemoved(removedItems); 
                Owner.OnCollectionChanged(triggerFlags);
            }
            
            Owner.NotifyInvolved(TriggerWhenItemAdded | TriggerWhenItemReplaced);

            return removedItems.Count;
        }

        public void RemoveRange(int startingIndex, int numberOfItemsToRemove)
        {
            if (startingIndex < 0) { throw new ArgumentOutOfRangeException(nameof(startingIndex)); }
            if (startingIndex + numberOfItemsToRemove > Count)
            {
                Debug.WriteLine($"A process called {nameof(ListImplementer<T>)}.{nameof(RemoveRange)}() attempting to " +
                                $"start at index {startingIndex} and remove {numberOfItemsToRemove} items, but the " +
                                $"collection's index only goes up to {Count - 1}. ");
            }
            
            var removedItems = new List<T>();

            if (numberOfItemsToRemove > 0)
            {
                long triggerFlags = ItemRemoved;
            
                if (startingIndex != collection.Count - 1)
                {
                    triggerFlags |= ItemMoved | ItemReplaced;
                }

                for (int i = 0; i < Count; i++)
                {
                    removedItems.Add(collection[i]);
                    collection.RemoveAt(i);
                }
                
                collection.RemoveRange(startingIndex, numberOfItemsToRemove);
                Owner.OnMultipleItemsRemoved(removedItems);
                Owner.OnCollectionChanged(triggerFlags);
            }
            else if (numberOfItemsToRemove < 0)
            {
                throw new ArgumentException($"{nameof(numberOfItemsToRemove)} cannot be less than 0. ");
            }
        }

        public int AddObject(object value)
        {
            int  index = collection.Count;
            bool wasAdded;

            if (value is T valueOfCorrectType)
            {
                wasAdded = Add(valueOfCorrectType);
            }
            else if (value is null && TheType<T>.IsNullable)
            {
                valueOfCorrectType = default;
                wasAdded = Add(default);
            }
            else
            {
                throw new ArgumentException($"A process attempted to add an object of type {value?.GetType()} to " +
                                            $"a {NameOf<ListImplementer<T>>()}");
            }
            
            if (wasAdded)
            {
                Owner.OnItemAdded(valueOfCorrectType);
                Owner.OnCollectionChanged(ItemAdded);
                Owner.NotifyInvolved(TriggerWhenItemRemoved | TriggerWhenItemReplaced | TriggerWhenItemMoved);
                return index;
            }
            else
            {
                //- Not sure why Add() would return false for a list but who knows...
                return -1;
            }
        }

        public void Reverse(int index, int count)
        {
            if (collection.Count > 1  &&  count > 1) //- No point in reversing 1 element.
            {
                collection.Reverse(index, count);
                Owner.OnCollectionChanged(ItemMoved);
            }
        }
        
        public void Reverse() => Reverse(0, Count);

        //- TODO : If we decide to write our own sorting methods we can keep track
        //         of whether anything actually changes or not. Or we could make a 
        //         List<T> class that has a Sort() method which returns a bool,
        //         indicating whether it actually moved anything.
        public void Sort(int index, int count, IComparer<T> comparer)
        {
            if (collection.Count > 1 && count > 1)
            {
                collection.Sort(index, count, comparer);
                Owner.OnCollectionChanged(ItemMoved);
            }
        }

        public void Sort(Comparison<T> comparison)
        {
            if (collection.Count > 1)
            {
                Collection.Sort(comparison);
                Owner.OnCollectionChanged(ItemMoved);
            }
        }

        public void Sort(IComparer<T> comparer) => Sort(0, Count, comparer);
        public void Sort()                      => Sort(0, Count, Comparer<T>.Default);

        public int BinarySearch(T item, IComparer<T> comparer, int startIndex, int count)
        {
            int indexOfItem = Collection.BinarySearch(startIndex, count, item, comparer);
            
            NotifyInvolved_IndexOf(indexOfItem);

            return indexOfItem;
        }
        
        public int BinarySearch(T item, IComparer<T> comparer)
        {
            int indexOfItem = Collection.BinarySearch(item, comparer);
            
            NotifyInvolved_IndexOf(indexOfItem);
            return indexOfItem;
        }
        public int BinarySearch(T item)
        {
            int indexOfItem = Collection.BinarySearch(item);
            
            NotifyInvolved_IndexOf(indexOfItem);
            return indexOfItem;
        }

        public int FindIndex(Predicate<T> predicate, int startIndex, int count)
        {
            int indexOfItem = Collection.FindIndex(startIndex, count, predicate);

            NotifyInvolved_IndexOf(indexOfItem);
            return indexOfItem;
        }
        
        public int FindIndex(Predicate<T> predicate, int startIndex)
        {
            int indexOfItem = Collection.FindIndex(startIndex, predicate);
            
            NotifyInvolved_IndexOf(indexOfItem);
            return indexOfItem;
        }
        
        public int FindIndex(Predicate<T> predicate)
        {
            int indexOfItem = Collection.FindIndex(predicate);
            
            NotifyInvolved_IndexOf(indexOfItem);
            return indexOfItem;
        }

        public int FindLastIndex(Predicate<T> predicate, int startIndex, int count)
        {
            int indexOfItem = Collection.FindLastIndex(startIndex, count, predicate);
            
            NotifyInvolved_LastIndexOf(indexOfItem);
            return indexOfItem;
        }
        public int FindLastIndex(Predicate<T> predicate, int startIndex)
        {
            int indexOfItem = Collection.FindLastIndex(startIndex, predicate);
            
            NotifyInvolved_LastIndexOf(indexOfItem);
            return indexOfItem;
        }
        
        public int FindLastIndex(Predicate<T> predicate)
        {
            int indexOfItem = Collection.FindLastIndex(predicate);
            
            NotifyInvolved_LastIndexOf(indexOfItem);
            return indexOfItem;
        }

        public int LastIndexOf(T item, int startIndex, int count)
        {
            int indexOfItem = Collection.LastIndexOf(item, startIndex, count);
            
            NotifyInvolved_LastIndexOf(indexOfItem);
            return indexOfItem;
        }
        public int LastIndexOf(T item, int startIndex)
        {
            int indexOfItem = Collection.LastIndexOf(item, startIndex);
            
            NotifyInvolved_LastIndexOf(indexOfItem);
            return indexOfItem;
        }
        public int LastIndexOf(T item)
        {
            int indexOfItem = Collection.LastIndexOf(item);
            
            NotifyInvolved_LastIndexOf(indexOfItem);
            return indexOfItem;
        }

        public int IndexOf(T item)
        {
            int indexOfItem = Collection.IndexOf(item);
            
            NotifyInvolved_IndexOf(indexOfItem);
            return indexOfItem;
        }
        
        public bool TrueForAll(Predicate<T> predicate)
        {
            bool isTrue = Collection.TrueForAll(predicate);

            if (isTrue) { Owner.NotifyInvolved(TriggerWhenItemAdded   | TriggerWhenItemReplaced); }
            else        { Owner.NotifyInvolved(TriggerWhenItemRemoved | TriggerWhenItemReplaced); }

            return isTrue;
        }

        public bool Exists(Predicate<T> predicate)
        {
            bool isTrue = Collection.Exists(predicate);

            NotifyInvolved_ContainsItem(isTrue);
            return isTrue;
        }

        public T Find(Predicate<T> predicate)
        {
            int indexOfItem = Collection.FindIndex(predicate);

            NotifyInvolved_IndexOf(indexOfItem);
            
            if (indexOfItem < 0) { return default; }
            else                 { return collection[indexOfItem]; }
        }

        public List<T> FindAll(Predicate<T> predicate)
        {
            var results = Collection.FindAll(predicate);

            if (results.Count > 0) { Owner.NotifyInvolved(TriggerWhenItemAdded    | 
                                                          TriggerWhenItemReplaced | 
                                                          TriggerWhenItemRemoved); }
            
            else                   { Owner.NotifyInvolved(TriggerWhenItemAdded | 
                                                          TriggerWhenItemReplaced); }

            return results;
        }

        public T FindLast(Predicate<T> predicate)
        {
            int indexOfItem = Collection.FindLastIndex(predicate);

            NotifyInvolved_LastIndexOf(indexOfItem);
            
            if (indexOfItem < 0) { return default; }
            else                 { return collection[indexOfItem]; }
        }

        public void ForEach(Action<T> action)
        {

            Collection.ForEach(action);
            Owner.NotifyInvolved(TriggerWhenItemAdded | TriggerWhenItemReplaced);
            //- I didn't include the ItemRemoved flag because the purpose of this method seems 
            //  to be to take some action on each item. If we remove an item, all of the remaining items 
            //  still had the Action done to them.
            
            //- Should this even notify? It doesnt return anything.
            //  The notification would only be useful if you wanted to
            //  make a reaction that ran this method every time a new
            //  item got into the collection, and you could just use
            //  Count to keep track of that.
        }

       public List<T> GetRange(int startIndex, int numberOfItems)
        {
            var results = Collection.GetRange(startIndex, numberOfItems);

            if (results.Count > 0) { Owner.NotifyInvolved(TriggerWhenItemRemoved | ItemReplaced | ItemMoved) ; }
            else                   { Owner.NotifyInvolved(TriggerWhenItemAdded   | ItemReplaced);              }

            return results;
        }

        public T[] ToArray()
        {
            var results = Collection.ToArray();

            Owner.NotifyInvolved(TriggerFlags.Default);
            return results;
        }

        public void TrimExcess() => Collection.TrimExcess();

        public List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
        {
            var results = Collection.ConvertAll(converter);

            Owner.NotifyInvolved(TriggerWhenItemAdded | TriggerWhenItemReplaced);
            return results;
        }
        
        public override bool CollectionEquals(IEnumerable<T> collectionToCompare)
        {
            var listToCompare = collectionToCompare.ToList();

            return collection.IsEquivalentTo(listToCompare, itemComparer);
        }
        
        public List<T> AsNormalList()
        {
            var createdList = new List<T>(Collection);
            
            Owner.NotifyInvolved(TriggerFlags.Default);
            return createdList;
        }
        //- TODO : We may want to grab the collection field directly if it seems like the user is trying
        //        to avoid creating dependencies when they call this.

        //- TODO : Figure out whether we think that methods which modify the collection, but also return a value,
        //         should call/not call Owner.NotifyInvolved(), since people may expect Reactors to be invalidated
        //         if a change in the collection might cause the the value returned by the method to change.
        //- Note : We could try using a type of object that has an implicit conversion to the normal return value,
        //         which is designed to notify if the conversion is actually used.
        
        #endregion

        
        #region Constructors

        protected ListImplementer(IListOwner<T> owner, List<T> list, IEqualityComparer<T> comparerForItems) : 
            base(list, owner)
        {
            itemComparer = comparerForItems ?? EqualityComparer<T>.Default;
        }        
        public ListImplementer(IListOwner<T>        owner,
                               IEnumerable<T>       collectionToCopy, 
                               IEqualityComparer<T> comparerForItems = null) : 
            this(owner, new List<T>(collectionToCopy), comparerForItems)
        {
        }
        
        public ListImplementer(IListOwner<T> owner, IEqualityComparer<T> itemComparer) : 
            this(owner, new List<T>(), itemComparer)
        {
        }

        public ListImplementer(IListOwner<T> owner) : this(owner, new List<T>(), null)
        {
        }

        #endregion


    }
}