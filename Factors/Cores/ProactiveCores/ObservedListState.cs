using System;
using System.Collections;
using System.Collections.Generic;
using Core.States;
using Core.Tools;
using JetBrains.Annotations;
using static Core.Tools.Types;

namespace Factors.Cores.ProactiveCores
{


    public class ObservedListState<T> : ObservedCollectionState<List<T>, T>, IListState<T>
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
                    OnCollectionChanged();
                }
            }
        }

        public int Capacity => Collection.Capacity;

        #endregion

        
        #region Instance Methods 
        
        public void Insert(int index, T item)
        {
            collection.Insert(index, item);
            OnCollectionChanged();
        }
        
        public void InsertRange(int index, IEnumerable<T> elements)
        {
            collection.InsertRange(index, elements);
            OnCollectionChanged();
        }
        
        public void RemoveAt(int index)
        {
            collection.RemoveAt(index);
            OnCollectionChanged();
        }
        
        public int RemoveAll(Predicate<T> predicate)
        {
            int elementsRemoved = collection.RemoveAll(predicate);

            if (elementsRemoved > 0)
            {
                OnCollectionChanged();
            }
            
            return elementsRemoved;
        }

        public void RemoveRange(int index, int count)
        {
            if (count > 0)
            {
                collection.RemoveRange(index, count);
                OnCollectionChanged();
            }
            else if (count < 0)
            {
                throw new ArgumentException($"{nameof(count)} cannot be less than 0. ");
            }
        }
        
        public int AddObject(object value)
        {
            if (value is T ||
                value == null  &&  TheType<T>.IsNullable)
            {
                IList collectionAsIList = collection;
                int   indexOfItem       = collectionAsIList.Add(value);

                if (indexOfItem >= 0)
                {
                    OnCollectionChanged();
                }

                return indexOfItem;
            }
            else
            {
                throw new ArgumentException("A process attempted to add an object of type " +
                                           $"{value?.GetType()} to a {NameOf<ObservedListState<T>>()}");
            }
        }
        
        public void Reverse(int index, int count)
        {
            if (collection.Count > 1  &&  count > 1) //- No point in reversing 1 element.
            {
                collection.Reverse(index, count);
                OnCollectionChanged();
            }
        }
        
        public void Reverse() => Reverse(0, Count);

        //- TODO : If we write our own sorting methods we can keep track of whether anything actually changes or not. 
        public void Sort(int index, int count, IComparer<T> comparer)
        {
            if (collection.Count > 1 && count > 1)
            {
                collection.Sort(index, count, comparer);
                OnCollectionChanged();
            }
        }

        public void Sort(Comparison<T> comparison)
        {
            if (collection.Count > 1)
            {
                Collection.Sort(comparison);
                OnCollectionChanged();
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
        public void    ForEach(Action<T>    action)        => Collection.ForEach(action);
        public List<T> GetRange(int startIndex, int count) => Collection.GetRange(startIndex, count);
        public T[]     ToArray()                           => Collection.ToArray();
        public void    TrimExcess()                        => Collection.TrimExcess();
        
        
        public List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter) => Collection.ConvertAll(converter);

        public List<T> AsNormalList() => new List<T>(Collection);
        //- TODO : We may want to grab the field directly if it seems like the user is trying to avoid creating dependencies
        //         when they call this.


        //- TODO : Figure out if we think that methods which modify the collection, but also return a value,
        //         should call/not call NotifyInvolved(), since people may expect Reactors to be invalidated
        //         if a change in the collection might cause the the value returned by the method to change.
        //- Note : We could try using a type of object that has an implicit conversion to the normal return value,
        //         which is designed to notify if the conversion is actually used.
        
        #endregion

        
        #region Constructors

        public ObservedListState(
            ICollection<T> collectionToCopy, IEqualityComparer<T> comparerForItems = null, string name = null) : 
                base(new List<T>(collectionToCopy), name ?? NameOf<ObservedListState<T>>())
        {
            itemComparer = comparerForItems ?? EqualityComparer<T>.Default;
        }
        public ObservedListState([NotNull] IEqualityComparer<T> itemComparer, string name = null) : 
            this(null, itemComparer, name)
        {
        }

        public ObservedListState(string name) : this(null, null, name)
        {
        }

        public ObservedListState(ICollection<T> collectionToCopy, string name = null) : 
            this(collectionToCopy, null, name)
        {
        }

        #endregion
    }
}