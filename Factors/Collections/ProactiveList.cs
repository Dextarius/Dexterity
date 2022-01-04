using System;
using System.Collections;
using System.Collections.Generic;
using Core.Factors;
using Core.States;
using Core.Tools;
using static Core.Tools.Types;

namespace Factors.Collections
{
    public class ProactiveList<T> : ProactiveCollection<IListState<T>, T>, IList<T>, IList
    {
        #region Instance Properties

        public T this[int index]
        {
            get => collection[index];
            set => collection[index] = value;
        }

        public int Capacity => collection.Capacity;

        #endregion

        
        #region Instance Methods

        public void    InsertRange(int index, IEnumerable<T> elements)   => collection.InsertRange(index, elements);
        public void    Insert(int index, T item)                         => collection.Insert(index, item);
        public int     IndexOf(T item)                                   => collection.IndexOf(item);
        public void    RemoveAt(int index)                               => collection.RemoveAt(index);
        public int     RemoveAll(Predicate<T> predicate)                 => collection.RemoveAll(predicate);
        public void    RemoveRange(int index, int count)                 => collection.RemoveRange(index, count);
        public void    ForEach(Action<T>    action)                      => collection.ForEach(action);
        public List<T> GetRange(int startIndex, int count)               => collection.GetRange(startIndex, count);
        public T[]     ToArray()                                         => collection.ToArray();
        public void    TrimExcess()                                      => collection.TrimExcess();
        public bool    TrueForAll(Predicate<T> predicate)                => collection.TrueForAll(predicate);
        public bool    Exists(Predicate<T> predicate)                    => collection.Exists(predicate);
        public T       Find(Predicate<T> predicate)                      => collection.Find(predicate);
        public List<T> FindAll(Predicate<T> predicate)                   => collection.FindAll(predicate);
        public T       FindLast(Predicate<T> predicate)                  => collection.FindLast(predicate);
        public List<T> AsNormalList()                                    => collection.AsNormalList();
        public void    Reverse()                                         => Reverse(0, Count);
        public void    Reverse(int index, int count)                     => collection.Reverse(index, count);
        public void    Sort()                                            => Sort(0, Count, Comparer<T>.Default);
        public void    Sort(IComparer<T> comparer)                       => Sort(0, Count, comparer);
        public void    Sort(Comparison<T> comparison)                    => collection.Sort(comparison);
        public void    Sort(int index, int count, IComparer<T> comparer) => collection.Sort(index, count, comparer);

        public int LastIndexOf(T item, int startIndex, int count) => collection.LastIndexOf(item, startIndex, count);
        public int LastIndexOf(T item, int startIndex)            => collection.LastIndexOf(item, startIndex);
        public int LastIndexOf(T item)                            => collection.LastIndexOf(item);
        
        public int FindIndex(Predicate<T> predicate, int startIndex, int count) => collection.FindIndex(predicate, startIndex, count);
        public int FindIndex(Predicate<T> predicate, int startIndex)            => collection.FindIndex(predicate, startIndex);
        public int FindIndex(Predicate<T> predicate)                            => collection.FindIndex(predicate);
        
        public int FindLastIndex(Predicate<T> predicate, int startIndex, int count) => collection.FindLastIndex(predicate, startIndex, count);
        public int FindLastIndex(Predicate<T> predicate, int startIndex)            => collection.FindLastIndex(predicate, startIndex);
        public int FindLastIndex(Predicate<T> predicate)                            => collection.FindLastIndex(predicate);
        
        public int BinarySearch(T item, IComparer<T> comparer, int startIndex, int count) => collection.BinarySearch(item, comparer, startIndex, count);
        public int BinarySearch(T item, IComparer<T> comparer)                            => collection.BinarySearch(item, comparer);
        public int BinarySearch(T item)                                                   => collection.BinarySearch(item);

        public List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter) => collection.ConvertAll(converter);

        #endregion

        
        #region Constructors

        public ProactiveList(IListState<T> listState, string name) : base(listState, name?? NameOf<ProactiveList<T>>())
        {
            
        }

        #endregion
        
        
        #region Explicit Implementations

        bool IList.IsFixedSize => false;
        bool IList.IsReadOnly  => false;
        
        int  IList.Add(object value) => collection.AddObject(value);
        
        int  IList.IndexOf(object value)
        {
            if (value is T valueOfCorrectType)
            {
                return IndexOf(valueOfCorrectType);
            }
            else return -1;
        }

        bool IList.Contains(object value)
        {
            if (value is T valueOfCorrectType)
            {
                return Contains(valueOfCorrectType);  
            }                                        
            else if (default(T) is null  &&  value is null)
            {
                return Contains(default(T));
            }
            else
            {
                return false;
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
                    collection[index] = valueOfCorrectType;
                }
                else if (value == null  &&  TheType<T>.IsNullable)
                {
                    collection[index] = default(T);
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