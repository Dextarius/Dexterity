using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Core.States;
using Core.Tools;
using JetBrains.Annotations;
using static Core.Tools.Types;
using static Factors.TriggerFlags;

namespace Factors.Cores.ProactiveCores
{
    public class DirectProactiveListCore<T> : DirectProactiveCollectionCore<List<T>, T>
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
        
        protected void OnRangeOfItemsAdded(int startingIndex, int count)
        {
            for (int i = startingIndex; i < startingIndex + count; i++)
            {
                ItemWasAdded.Send(collection[i]);
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
            OnItemAdded(item);
            OnCollectionChanged(triggerFlags);
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
                OnRangeOfItemsAdded(index, numberOfItemsInserted);
                OnCollectionChanged(triggerFlags);
            }
        }

        public void RemoveAt(int index)
        {
            if (index >= collection.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            else
            {
                var valueToRemove = collection[index];
                
                collection.RemoveAt(index);
                OnItemRemoved(valueToRemove);
            }
        }
        
        public int RemoveAll(Predicate<T> predicate)
        {
            if (predicate is null) { throw new ArgumentNullException(nameof(predicate)); }

            int numberOfItemsRemoved = 0;
            
            for (int i = collection.Count - 1; i >= 0; i--)
            {
                var currentItem = collection[i];

                if (predicate(currentItem))
                {
                    collection.RemoveAt(i);
                    OnItemRemoved(currentItem);
                    numberOfItemsRemoved++;
                }
            }

            return numberOfItemsRemoved;
        }

        public void RemoveRange(int index, int count)
        {
            if (count < 0) { throw new ArgumentOutOfRangeException($"{nameof(count)} cannot be less than 0. "); }
            if (index + count > collection.Count) { throw new ArgumentOutOfRangeException($"{nameof(count)}. "); }

            for (int i = index + count - 1; i >= index; i--)
            {
                var valueToRemove = collection[i];

                collection.RemoveAt(index);
                OnItemRemoved(valueToRemove);
            }
        }

        public int AddObject(object value)
        {
            int indexOfItem = collection.Count;

            if (value is T valueOfCorrectType)
            {
                Add(valueOfCorrectType);
            }
            else if (value is null  &&  default(T) is null)
            {
                Add(default);
            }
            else
            {
                throw new ArgumentException("A process attempted to add an object of type " +
                                           $"{value?.GetType()} to a {NameOf<ObservedProactiveListCore<T>>()}");
            }
            
            return indexOfItem;
        }
        
        public void Reverse(int index, int count)
        {
            if (collection.Count > 1  &&  count > 1) //- No point in reversing 1 element.
            {
                collection.Reverse(index, count);
            }
        }
        
        public void Reverse() => Reverse(0, Count);

        public void Sort(int index, int count, IComparer<T> comparer)
        {
            if (collection.Count > 1 && count > 1)
            {
                collection.Sort(index, count, comparer);
            }
        }

        public void Sort(Comparison<T> comparison)
        {
            if (collection.Count > 1)
            {
                Collection.Sort(comparison);
            }
        }

        public void Sort(IComparer<T> comparer) => Sort(0, Count, comparer);
        public void Sort()                      => Sort(0, Count, Comparer<T>.Default);

        public int BinarySearch(T item, IComparer<T> comparer, int startIndex, int count) => Collection.BinarySearch(startIndex, count, item, comparer);
        public int BinarySearch(T item, IComparer<T> comparer)                            => Collection.BinarySearch(item, comparer);  //- Should we use the Comparer we already have?
        public int BinarySearch(T item)                                                   => Collection.BinarySearch(item);
        
        public int FindIndex(Predicate<T> predicate, int startIndex, int count) => Collection.FindIndex(startIndex, count, predicate);
        public int FindIndex(Predicate<T> predicate, int startIndex)            => Collection.FindIndex(startIndex, predicate);
        public int FindIndex(Predicate<T> predicate)                            => Collection.FindIndex(predicate);
        
        public int FindLastIndex(Predicate<T> predicate, int startIndex, int count) => Collection.FindLastIndex(startIndex, count, predicate);
        public int FindLastIndex(Predicate<T> predicate, int startIndex)            => Collection.FindLastIndex(startIndex, predicate);
        public int FindLastIndex(Predicate<T> predicate)                            => Collection.FindLastIndex(predicate);

        public int LastIndexOf(T item, int startIndex, int count) => Collection.LastIndexOf(item, startIndex, count);
        public int LastIndexOf(T item, int startIndex)            => Collection.LastIndexOf(item, startIndex);
        public int LastIndexOf(T item)                            => Collection.LastIndexOf(item);

        public int     IndexOf(T item)                     => Collection.IndexOf(item);
        public bool    TrueForAll(Predicate<T> predicate)  => Collection.TrueForAll(predicate);
        public bool    Exists(Predicate<T> predicate)      => Collection.Exists(predicate);
        public T       Find(Predicate<T> predicate)        => Collection.Find(predicate);
        public List<T> FindAll(Predicate<T> predicate)     => Collection.FindAll(predicate);
        public T       FindLast(Predicate<T> predicate)    => Collection.FindLast(predicate);
        public void    ForEach(Action<T>     action)       => Collection.ForEach(action);
        public List<T> GetRange(int startIndex, int count) => Collection.GetRange(startIndex, count);
        public T[]     ToArray()                           => Collection.ToArray();
        public void    TrimExcess()                        => Collection.TrimExcess();
        public List<T> AsNormalList()                      => new List<T>(Collection);

        public List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter) => Collection.ConvertAll(converter);
        
        #endregion

        
        #region Constructors

        protected DirectProactiveListCore(List<T> list, IEqualityComparer<T> comparerForItems) : base(list)
        {
            itemComparer = comparerForItems ?? EqualityComparer<T>.Default;
        }        
        public DirectProactiveListCore(IEnumerable<T> collectionToCopy, IEqualityComparer<T> comparerForItems = null) : 
            this(new List<T>(collectionToCopy), comparerForItems)
        {
        }
        
        public DirectProactiveListCore(IEqualityComparer<T> itemComparer) : this(new List<T>(), itemComparer)
        {
        }

        public DirectProactiveListCore() : this(new List<T>(), null)
        {
        }

        #endregion
    }
}