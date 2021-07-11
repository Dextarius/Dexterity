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
                List<T>         collection    = Collection;
                IState<List<T>> previousState = state;
                IState<List<T>> newState      = new State<List<T>>(collection);
                
                lock (syncLock)
                {
                    state = newState;
                    collection[index] = value;
                }
                
                previousState.Invalidate();
                Observer.NotifyChanged(previousState);
            }
        }

        public int Capacity => Collection.Capacity;

        #endregion

        
        #region Instance Methods 
        
        public int IndexOf(T item)
        {
            lock (syncLock)
            {
                return Collection.IndexOf(item);
            }
        }

        public void Insert(int index, T item)
        {
            List<T>         collection    = Collection;
            IState<List<T>> previousState = state;
            IState<List<T>> newState      = new State<List<T>>(collection);
                
            lock (syncLock)
            {
                state = newState;
                collection.Insert(index, item);
            }
                
            previousState.Invalidate();
            Observer.NotifyChanged(previousState);
        }
        
        public void InsertRange(int index, IEnumerable<T> elements)
        {
            List<T>         collection    = Collection;
            IState<List<T>> previousState = state;
            IState<List<T>> newState      = new State<List<T>>(collection);
                
            lock (syncLock)
            {
                state = newState;
                collection.InsertRange(index, elements);
            }
                
            previousState.Invalidate();
            Observer.NotifyChanged(previousState);
        }
        
        public void RemoveAt(int index)
        {
            List<T>         collection    = Collection;
            IState<List<T>> previousState = state;
            IState<List<T>> newState      = new State<List<T>>(collection);
                
            lock (syncLock)
            {
                state = newState;
                collection.RemoveAt(index);
            }
                
            previousState.Invalidate();
            Observer.NotifyChanged(previousState);
        }
        
        public int RemoveAll(Predicate<T> predicate)
        {
            List<T>         collection    = Collection;
            IState<List<T>> previousState = state;
            int             elementsRemoved;
 
            lock (syncLock)
            {
                elementsRemoved = collection.RemoveAll(predicate);

                if (elementsRemoved > 0)
                {
                    state = new State<List<T>>(collection);
                }
            }

            if (elementsRemoved > 0)
            {
                previousState.Invalidate();
                Observer.NotifyChanged(previousState);
            }
            
            return elementsRemoved;
        }
        
        public void RemoveRange(int index, int count)
        {
            List<T>         collection    = Collection;
            IState<List<T>> previousState = state;
            IState<List<T>> newState      = new State<List<T>>(collection);
 
            lock (syncLock)
            {
                state = newState;
                Collection.RemoveRange(index, count);
            }

            previousState.Invalidate();
            Observer.NotifyChanged(previousState); 
        }

        
        //- TODO: A few of these methods could avoid creating a new state if the changes have no effect
        //        on the collection (i.e. reversing a list with 1 element)
        public void Reverse(int index, int count)
        {
            if (this.Count > 1  && count > 1)
            {
                List<T>         collection    = Collection;
                IState<List<T>> newState      = new State<List<T>>(collection);
                IState<List<T>> previousState;

                lock (syncLock)
                {
                    previousState = state;
                    state = newState;
                    Collection.Reverse(index, count);
                }

                previousState.Invalidate();
                Observer.NotifyChanged(previousState); 
            }
        }
        
        public void Reverse() => Reverse(0, Count);

        public void Sort(int index, int count, IComparer<T> comparer)
        {
            List<T>         collection    = Collection;
            IState<List<T>> newState      = new State<List<T>>(collection);
            IState<List<T>> previousState;

            lock (syncLock)
            {
                previousState = state;
                state = newState;
                Collection.Sort(index, count, comparer);
            }

            previousState.Invalidate();
            Observer.NotifyChanged(previousState); 
        }

        public void Sort(Comparison<T> comparison)
        {
            List<T>         collection    = Collection;
            IState<List<T>> newState      = new State<List<T>>(collection);
            IState<List<T>> previousState;

            lock (syncLock)
            {
                previousState = state;
                state = newState;
                Collection.Sort(comparison);
            }

            previousState.Invalidate();
            Observer.NotifyChanged(previousState); 
        }

        //- TODO: We're probably going to need to do more thread synchronization for these methods 
        
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
        
        public ProactiveList(IEqualityComparer<T> comparer = null, string name = null) : 
            this(new List<T>(), comparer, name)
        {
            
        }
        
        public ProactiveList(ICollection<T> collectionToUse, IEqualityComparer<T> comparer = null, string name = null) : 
            this(new List<T>(collectionToUse), comparer, name)
        {
            
        }

        public ProactiveList(List<T> listToUse, IEqualityComparer<T> comparer, string name) : 
            base(listToUse, comparer, name ?? NameOf<ProactiveList<T>>())
        {
            
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
                IList           collection    = Collection;
                IState<List<T>> previousState = state;
                IState<List<T>> newState      = new State<List<T>>(Collection);
                int             indexOfItem;

                lock (syncLock)
                {
                    indexOfItem = collection.Add(value);
                    state       = newState;
                }

                previousState.Invalidate();
                Observer.NotifyChanged(previousState);

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