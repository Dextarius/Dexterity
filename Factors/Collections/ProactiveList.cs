using System;
using System.Collections;
using System.Collections.Generic;
using Causality;
using Causality.States;
using Core.Causality;
using Core.Factors;
using Core.Tools;
using static Core.Tools.Types;


namespace Factors.Collections
{
    public class ProactiveList<T> : ProactiveCollection<List<T>, T>, IList<T>, IList
    {
        #region Instance Properties

        public T this[int index]
        {
            get => Collection[index];
            set
            {
                var collection   = state.Peek();
                var currentValue = collection[index];

                if (itemComparer.Equals(currentValue, value) is false)
                {
                    collection[index] = value;
                }
            }
        }

        public int Capacity => Collection.Capacity;

        #endregion

        
        #region Instance Methods 
        
        public int IndexOf(T item) => Collection.IndexOf(item);

        public void Insert(int index, T item)
        {
            List<T> collection = state.Peek();
            
            collection.Insert(index, item);
            state.OnChanged();
        }
        
        public void InsertRange(int index, IEnumerable<T> elements)
        {
            List<T> collection = state.Peek();
            
            collection.InsertRange(index, elements);
            state.OnChanged();
        }
        
        public void RemoveAt(int index)
        {
            List<T> collection = state.Peek();
            
            collection.RemoveAt(index);
            state.OnChanged();
        }
        
        public int RemoveAll(Predicate<T> predicate)
        {
            List<T> collection      = state.Peek();
            int     elementsRemoved = collection.RemoveAll(predicate);

            if (elementsRemoved > 0)
            {
                state.OnChanged();
            }
            
            return elementsRemoved;
        }
        //- TODO : Figure out if we think that methods which modify the collection, but also return a value,
        //         should call/not call NotifyInvolved(), since people may expect Reactors to be invalidated
        //         if the method might return a different value.
        
        public void RemoveRange(int index, int count)
        {
            List<T> collection = state.Peek();

            if (count > 0)
            {
                collection.RemoveRange(index, count);
                state.OnChanged();
            }
            else if (count < 0)
            {
                throw new ArgumentException($"{nameof(count)} cannot be less than 0. ");
            }
        }
        
        public void Reverse(int index, int count)
        {
            List<T> collection = state.Peek();

            if (collection.Count > 1 && count > 1) //- No point in reversing 1 element.
            {
                collection.Reverse(index, count);
                state.OnChanged();
            }
        }
        
        public void Reverse() => Reverse(0, Count);

        //- TODO : If we write our own sorting methods we can keep track of whether anything actually changes or not. 
        public void Sort(int index, int count, IComparer<T> comparer)
        {
            List<T> collection = state.Peek();

            if (collection.Count > 1 && count > 1)
            {
                collection.Sort(index, count, comparer);
                state.OnChanged();
            }
        }

        public void Sort(Comparison<T> comparison)
        {
            List<T> collection = state.Peek();

            if (collection.Count > 1)
            {
                Collection.Sort(comparison);
                state.OnChanged();
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

        public bool    TrueForAll(Predicate<T> predicate)    => Collection.TrueForAll(predicate);
        public bool        Exists(Predicate<T> predicate)    => Collection.Exists(predicate);
        public T             Find(Predicate<T> predicate)    => Collection.Find(predicate);
        public List<T>    FindAll(Predicate<T> predicate)    => Collection.FindAll(predicate);
        public T         FindLast(Predicate<T> predicate)    => Collection.FindLast(predicate);
        public void       ForEach(Action<T>    action)       => Collection.ForEach(action);
        public List<T>   GetRange(int startIndex, int count) => Collection.GetRange(startIndex, count);
        public T[]        ToArray()                          => Collection.ToArray();
        public void    TrimExcess()                          => Collection.TrimExcess();
        
        public List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter) => Collection.ConvertAll(converter);

        //- TODO : We may want to pause the Observer if it seems like the user is calling this to make a collection that creates no dependencies
        public List<T> AsNormalList => new List<T>(Collection);

        #endregion

        
        #region Constructors
        
        public ProactiveList(string name = null) : 
            this(new List<T>(), null, name)
        {
        }
        
        public ProactiveList(IEqualityComparer<T> comparerForItems = null, string name = null) : 
            this(new List<T>(), comparerForItems, name)
        {
        }
        
        public ProactiveList(
            ICollection<T> collectionToCopy, IEqualityComparer<T> comparerForItems = null, string name = null) : 
                this(new List<T>(collectionToCopy), comparerForItems, name)
        {
        }

        public ProactiveList(List<T> listToUse, IEqualityComparer<T> comparerForItems, string name) : 
            base(listToUse, comparerForItems, name?? NameOf<ProactiveList<T>>())
        {
            state.Value = listToUse;
        }

        #endregion
        
        
        #region Explicit Implementations

        bool IList.IsFixedSize => false;
        bool IList.IsReadOnly  => false;
        
        int  IList.IndexOf(object value)  => (value is T valueOfCorrectType) ?  IndexOf(valueOfCorrectType) : -1;
        bool IList.Contains(object value) => (value is T valueOfCorrectType) && Contains(valueOfCorrectType);

        int IList.Add(object value)
        {
            if (value is T ||
                value == null  &&  TheType<T>.IsNullable)
            {
                IList           collection    = state.Peek();
                int             indexOfItem   = collection.Add(value);
                
                state.OnChanged();
                state.NotifyInvolved();
                
                return indexOfItem;
            }
            else
            {
                throw new ArgumentException("A process attempted to add an object of type " +
                                            $"{value?.GetType()} to a {NameOf<ProactiveList<T>>()}");
            }
        }
        
        void IList.Insert(int index, object value)
        {
            if (value is T valueOfCorrectType)
            {
                Insert(index, valueOfCorrectType);
            }
            else if (default(T) == null  &&  value == null)
            {
                Insert(index, default);
            }
            else
            {
                throw new ArgumentException("A process attempted to insert an object of type " +
                                            $"{value?.GetType()} into a {NameOf<ProactiveList<T>>()}");
            }
        }

        void IList.Remove(object value)
        {
            if (value is T valueOfCorrectType)
            {
                Remove(valueOfCorrectType);
            }
            else if (default(T) == null  &&  value == null)
            {
                Remove(default);
            }
        }
        
        object IList.this[int index]
        {
            get => this[index];
            set
            {
                if (value is T valueOfCorrectType)
                {
                    Collection[index] = valueOfCorrectType;
                }
                else if (default(T) == null  &&  value == null)
                {
                    Collection[index] = default(T);
                }
                else
                {
                    throw new ArgumentException("A process attempted to add an object of type " +
                                                $"{value?.GetType()} into a {NameOf<ProactiveList<T>>()}");
                }
            }
        }

        #endregion
    }
}