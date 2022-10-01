using System;
using System.Collections;
using System.Collections.Generic;
using Core.Factors;
using Core.States;
using Core.Tools;
using Dextarius.Utilities;
using Factors.Cores.ProactiveCores;
using static Dextarius.Utilities.Types;

namespace Factors.Collections
{
    public class ProactiveList<T> : ProactiveCollection<IProactiveListCore<T>, T>, IList<T>, IList
    {
        #region Instance Properties

        public T this[int index]
        {
            get => core[index];
            set => core[index] = value;
        }

        public int Capacity => core.Capacity;

        #endregion

        
        #region Instance Methods

        public void    InsertRange(int index, IEnumerable<T> elements)   => core.InsertRange(index, elements);
        public void    Insert(int index, T item)                         => core.Insert(index, item);
        public int     IndexOf(T item)                                   => core.IndexOf(item);
        public void    RemoveAt(int index)                               => core.RemoveAt(index);
        public int     RemoveAll(Predicate<T> predicate)                 => core.RemoveAll(predicate);
        public void    RemoveRange(int index, int count)                 => core.RemoveRange(index, count);
        public void    ForEach(Action<T>    action)                      => core.ForEach(action);
        public List<T> GetRange(int startIndex, int count)               => core.GetRange(startIndex, count);
        public T[]     ToArray()                                         => core.ToArray();
        public void    TrimExcess()                                      => core.TrimExcess();
        public bool    TrueForAll(Predicate<T> predicate)                => core.TrueForAll(predicate);
        public bool    Exists(Predicate<T> predicate)                    => core.Exists(predicate);
        public T       Find(Predicate<T> predicate)                      => core.Find(predicate);
        public List<T> FindAll(Predicate<T> predicate)                   => core.FindAll(predicate);
        public T       FindLast(Predicate<T> predicate)                  => core.FindLast(predicate);
        public List<T> AsNormalList()                                    => core.AsNormalList();
        public void    Reverse()                                         => Reverse(0, Count);
        public void    Reverse(int index, int count)                     => core.Reverse(index, count);
        public void    Sort()                                            => Sort(0, Count, Comparer<T>.Default);
        public void    Sort(IComparer<T> comparer)                       => Sort(0, Count, comparer);
        public void    Sort(Comparison<T> comparison)                    => core.Sort(comparison);
        public void    Sort(int index, int count, IComparer<T> comparer) => core.Sort(index, count, comparer);

        public int LastIndexOf(T item, int startIndex, int count) => core.LastIndexOf(item, startIndex, count);
        public int LastIndexOf(T item, int startIndex)            => core.LastIndexOf(item, startIndex);
        public int LastIndexOf(T item)                            => core.LastIndexOf(item);
        
        public int FindIndex(Predicate<T> predicate, int startIndex, int count) => core.FindIndex(predicate, startIndex, count);
        public int FindIndex(Predicate<T> predicate, int startIndex)            => core.FindIndex(predicate, startIndex);
        public int FindIndex(Predicate<T> predicate)                            => core.FindIndex(predicate);
        
        public int FindLastIndex(Predicate<T> predicate, int startIndex, int count) => core.FindLastIndex(predicate, startIndex, count);
        public int FindLastIndex(Predicate<T> predicate, int startIndex)            => core.FindLastIndex(predicate, startIndex);
        public int FindLastIndex(Predicate<T> predicate)                            => core.FindLastIndex(predicate);
        
        public int BinarySearch(T item, IComparer<T> comparer, int startIndex, int count) => core.BinarySearch(item, comparer, startIndex, count);
        public int BinarySearch(T item, IComparer<T> comparer)                            => core.BinarySearch(item, comparer);
        public int BinarySearch(T item)                                                   => core.BinarySearch(item);

        public List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter) => core.ConvertAll(converter);

        #endregion

        
        #region Constructors

        public ProactiveList(IProactiveListCore<T> listCore, string name) : 
            base(listCore, name?? NameOf<ProactiveList<T>>())
        {
        }
        
        public ProactiveList(string name = null) : this(new ObservedProactiveListCore<T>(), name)
        {
        }

        #endregion
        
        
        #region Explicit Implementations

        bool IList.IsFixedSize => false;
        bool IList.IsReadOnly  => false;
        
        int  IList.Add(object value) => core.AddObject(value);
        
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
                    core[index] = valueOfCorrectType;
                }
                else if (value == null  &&  TheType<T>.IsNullable)
                {
                    core[index] = default(T);
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