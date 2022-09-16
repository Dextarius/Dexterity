using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.States;
using Core.Tools;
using JetBrains.Annotations;
using Dextarius.Collections;
using static Core.Tools.Types;
using static Factors.TriggerFlags;

namespace Factors.Cores.ProactiveCores
{
    public class ObservedProactiveListCore<T> : ObservedProactiveCollectionCore<List<T>, T>, IProactiveListCore<T>
    {
        #region Instance Fields

        [NotNull]
        protected readonly IEqualityComparer<T> itemComparer;
        
        #endregion
        
        #region Instance Properties

        public T this[int index]
        {
            get => Collection[index];
            set
            {
                var currentValue = collection[index];

                if (itemComparer.Equals(currentValue, value) is false)
                {
                    collection[index] = value;
                    OnItemReplaced(currentValue, value, ItemReplaced);
                }
            }
        }

        public int Capacity => Collection.Capacity;

        #endregion

        
        #region Instance Methods 
        
        public void Insert(int index, T itemToInsert)
        {
            long triggerFlags = ItemAdded;
            
            if (index != collection.Count)
            {
                triggerFlags |= ItemMoved | ItemReplaced;
            }
            
            collection.Insert(index, itemToInsert);
            OnCollectionChanged(triggerFlags);
        }
        
        public void InsertRange(int index, IEnumerable<T> itemsToInsert)
        {
            long triggerFlags = ItemAdded;
            
            if (index != collection.Count)
            {
                triggerFlags |= ItemMoved | ItemReplaced;
                
            }
            
            collection.InsertRange(index, itemsToInsert);
            OnCollectionChanged(triggerFlags);
        }
        
        public void RemoveAt(int index)
        {
            long triggerFlags = ItemRemoved;
            
            if (index != collection.Count - 1)
            {
                triggerFlags |= ItemMoved | ItemReplaced;
            }
            
            collection.RemoveAt(index);
            OnCollectionChanged(triggerFlags);
        }
        
        public int RemoveAll(Predicate<T> predicate)
        {
            int elementsRemoved = collection.RemoveAll(predicate);

            if (elementsRemoved > 0)
            {
                OnCollectionChanged(ItemRemoved | ItemMoved | ItemReplaced);
            }
            
            NotifyInvolved(TriggerWhenItemAdded | ItemReplaced);

            return elementsRemoved;
        }

        public void RemoveRange(int index, int count)
        {
            if (count > 0)
            {
                long triggerFlags = ItemRemoved;
            
                if (index != collection.Count - 1)
                {
                    triggerFlags |= ItemMoved | ItemReplaced;
                }
                
                collection.RemoveRange(index, count);
                OnCollectionChanged(triggerFlags);
            }
            else if (count < 0)
            {
                throw new ArgumentException($"{nameof(count)} cannot be less than 0. ");
            }
        }
        
        public int AddObject(object value)
        {
            int  index    = collection.Count;
            bool wasAdded;
            
            if (value is null && TheType<T>.IsNullable)
            {
                wasAdded = Add(default);
            }
            else if (value is T objectOfCorrectType)
            {
                wasAdded = Add(objectOfCorrectType);
            }
            else
            {
                throw new ArgumentException("A process attempted to add an object of type " +
                                            $"{value?.GetType()} to a {NameOf<ObservedProactiveListCore<T>>()}");
            }
            
            if (wasAdded)
            {
                OnCollectionChanged(ItemAdded);
                NotifyInvolved(TriggerFlags.ItemRemoved | TriggerFlags.ItemReplaced | TriggerFlags.ItemMoved);
                return index;
            }
            else
            {
                //- Not sure why Add() would return false for a list...
                return -1;
            }
        }

        protected override bool AddItem(T item, out long notifyInvolvedFlags, out long additionalTriggerFlags)
        {
            collection.Add(item);
            additionalTriggerFlags = TriggerFlags.None;
            notifyInvolvedFlags           = TriggerFlags.None;
            return true;
        }
        
        protected override bool RemoveItem(T item, out long additionalTriggerFlags)
        {
            int itemsIndex = collection.IndexOf(item);
            int lastIndex      = collection.Count - 1;
            
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

        public void Reverse(int index, int count)
        {
            if (collection.Count > 1  &&  count > 1) //- No point in reversing 1 element.
            {
                collection.Reverse(index, count);
                OnCollectionChanged(ItemMoved);
            }
        }

        public void Reverse() => Reverse(0, Count);

        //- TODO : If we decide to write our own sorting methods we can keep track
        //         of whether anything actually changes or not. 
        public void Sort(int index, int count, IComparer<T> comparer)
        {
            if (collection.Count > 1 && count > 1)
            {
                collection.Sort(index, count, comparer);
                NotifyInvolved_Rearranged();
            }
        }

        public void Sort(Comparison<T> comparison)
        {
            if (collection.Count > 1)
            {
                Collection.Sort(comparison);
                NotifyInvolved_Rearranged();
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

            if (isTrue)
            {
                NotifyInvolved(TriggerWhenItemAdded | ItemReplaced);
            }
            else
            {
                NotifyInvolved(TriggerWhenItemRemoved | ItemReplaced);
            }

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

            if (results.Count > 0)
            {
                NotifyInvolved(TriggerWhenItemAdded | ItemRemoved | ItemReplaced);
            }
            else
            {
                NotifyInvolved(TriggerWhenItemAdded | ItemReplaced);

            }

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
            NotifyInvolved(TriggerWhenItemAdded | ItemReplaced);
            //- I didn't include the ItemRemoved flag because the purpose of this method seems 
            //  to be to take some action on each item. If we remove an item, all of the remaining items 
            //  still had the Action done to them.
            Collection.ForEach(action);
            
            //- Should this even notify? It doesnt return anything.
            //  The notification would only be useful if you wanted to
            //  make a reaction that ran this method every time a new
            //  item got into the collection.
        }

       public  List<T> GetRange(int startIndex, int numberOfItems)
        {
            var results = Collection.GetRange(startIndex, numberOfItems);

            if (results.Count > 0)
            {
                NotifyInvolved(TriggerWhenItemRemoved | ItemReplaced | ItemMoved) ;
            }
            else
            {
                NotifyInvolved(TriggerWhenItemAdded | ItemReplaced);
            }

            return results;
        }

        public T[] ToArray()
        {
            var results = Collection.ToArray();

            NotifyInvolved(TriggerFlags.Default);
            return results;
        }

        public void TrimExcess() => Collection.TrimExcess();

        public List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
        {
            var results = Collection.ConvertAll(converter);

            NotifyInvolved(TriggerWhenItemAdded | ItemReplaced);
            return results;
        }
        
        public override bool CollectionEquals(IEnumerable<T> collectionToCompare) => 
            collection.IsEquivalentTo(collectionToCompare.ToList(), itemComparer);

        public List<T> AsNormalList()
        {
            var createdList = new List<T>(Collection);
            
            NotifyInvolved(TriggerFlags.Default);
            return createdList;
        }
        //- TODO : We may want to grab the field directly if it seems like the user is trying to avoid creating dependencies
        //         when they call this.

        //- TODO : Figure out if we think that methods which modify the collection, but also return a value,
        //         should call/not call NotifyInvolved(), since people may expect Reactors to be invalidated
        //         if a change in the collection might cause the the value returned by the method to change.
        //- Note : We could try using a type of object that has an implicit conversion to the normal return value,
        //         which is designed to notify if the conversion is actually used.
        
        #endregion

        
        #region Constructors

        protected ObservedProactiveListCore(List<T> list, IEqualityComparer<T> comparerForItems) : 
            base(list)
        {
            itemComparer = comparerForItems ?? EqualityComparer<T>.Default;
        }        
        public ObservedProactiveListCore(
            IEnumerable<T> collectionToCopy, IEqualityComparer<T> comparerForItems = null) : 
            this(new List<T>(collectionToCopy), comparerForItems)
        {
        }
        
        public ObservedProactiveListCore(IEqualityComparer<T> itemComparer) : 
            this(new List<T>(), itemComparer)
        {
        }

        public ObservedProactiveListCore() : this(new List<T>(), null)
        {
        }

        #endregion


    }
}