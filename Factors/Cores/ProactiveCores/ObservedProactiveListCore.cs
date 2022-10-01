using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Core.States;
using Core.Tools;
using JetBrains.Annotations;
using Dextarius.Collections;
using Factors.Collections;
using static Dextarius.Utilities.Types;
using static Factors.TriggerFlags;

namespace Factors.Cores.ProactiveCores
{
    public class ObservedProactiveListCore<T> : ObservedProactiveCollectionCore<List<T>, T, IListImplementer<T>>, 
                                                IProactiveListCore<T>, IListOwner<T>
    {
        #region Instance Properties

        public T this[int index]
        {
            get => implementer[index];
            set => implementer[index] = value;
        }

        public int Capacity => implementer.Capacity;

        #endregion

        
        #region Instance Methods
        
        public void OnRangeOfItemsAdded(int startingIndex, int count)
        {
            for (int i = startingIndex; i < startingIndex + count; i++)
            {
                ItemWasAdded.Send(this[i]);
            }
        }

        public void Insert(int index, T itemToInsert)                    => implementer.Insert(index, itemToInsert);
        public void InsertRange(int index, IEnumerable<T> itemsToInsert) => implementer.InsertRange(index, itemsToInsert);
        public void RemoveAt(int index)                                  => implementer.RemoveAt(index);
        public int  RemoveAll(Predicate<T> predicate)                    => implementer.RemoveAll(predicate);
        public void RemoveRange(int index, int count)                    => implementer.RemoveRange(index, count);
        public int  AddObject(object value)                              => implementer.AddObject(value);
        public void Reverse(int index, int count)                        => implementer.Reverse(index, count);
        public void Reverse()                                            => Reverse(0, Count);
        public void Sort(int index, int count, IComparer<T> comparer)    => implementer.Sort(index, count, comparer);
        public void Sort(Comparison<T> comparison)                       => implementer.Sort(comparison);
        public void Sort(IComparer<T> comparer)                          => implementer.Sort(0, Count, comparer);
        public void Sort()                                               => implementer.Sort(0, Count, Comparer<T>.Default);
        
        public int BinarySearch(T item, IComparer<T> comparer, int startIndex, int count) => 
            implementer.BinarySearch(item, comparer, startIndex, count);
        
        public int     BinarySearch(T item, IComparer<T> comparer)    => implementer.BinarySearch(item, comparer);
        public int     BinarySearch(T item)                           => implementer.BinarySearch(item);
        public int     LastIndexOf(T item, int startIndex, int count) => implementer.LastIndexOf(item, startIndex, count);
        public int     LastIndexOf(T item, int startIndex)            => implementer.LastIndexOf(item, startIndex);
        public int     LastIndexOf(T item)                            => implementer.LastIndexOf(item);
        public int     IndexOf(T item)                                => implementer.IndexOf(item);
        public bool    TrueForAll(Predicate<T> predicate)             => implementer.TrueForAll(predicate);
        public bool    Exists(Predicate<T> predicate)                 => implementer.Exists(predicate);
        public T       Find(Predicate<T> predicate)                   => implementer.Find(predicate);
        public List<T> FindAll(Predicate<T> predicate)                => implementer.FindAll(predicate);
        public T       FindLast(Predicate<T> predicate)               => implementer.FindLast(predicate);
        public void    ForEach(Action<T> action)                      => implementer.ForEach(action);
        public List<T> GetRange(int startIndex, int numberOfItems)    => implementer.GetRange(startIndex, numberOfItems);
        public T[]     ToArray()                                      => implementer.ToArray();
        public void    TrimExcess()                                   => implementer.TrimExcess();
        public List<T> AsNormalList()                                 => implementer.AsNormalList();
        public int     FindLastIndex(Predicate<T> predicate)          => implementer.FindLastIndex(predicate);
        
        public int FindLastIndex(Predicate<T> predicate, int startIndex, int count) => 
            implementer.FindLastIndex(predicate, startIndex, count);
        
        public int FindLastIndex(Predicate<T> predicate, int startIndex) =>
            implementer.FindLastIndex(predicate, startIndex);
        
        public int           FindIndex(Predicate<T> predicate, int startIndex, int count) => implementer.FindIndex(predicate, startIndex, count);
        public int           FindIndex(Predicate<T> predicate, int startIndex)            => implementer.FindIndex(predicate, startIndex);
        public int           FindIndex(Predicate<T> predicate)                            => implementer.FindIndex(predicate);
        public List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)         => implementer.ConvertAll(converter);
        
        #endregion

        
        #region Constructors

        public ObservedProactiveListCore(IListImplementer<T> implementation)
        {
            implementer = implementation;
        }

        public ObservedProactiveListCore(List<T> list, IEqualityComparer<T> comparerForItems) 
        {
            implementer = new ListImplementer<T>(this, list, comparerForItems);
        } 
        
        public ObservedProactiveListCore(IEnumerable<T> collectionToCopy, IEqualityComparer<T> comparerForItems = null) : 
            this(new List<T>(collectionToCopy), comparerForItems)
        {
        }
        
        public ObservedProactiveListCore(IEqualityComparer<T> itemComparer) : this(new List<T>(), itemComparer)
        {
        }

        public ObservedProactiveListCore() : this(new List<T>(), null)
        {
        }

        #endregion
    }
}