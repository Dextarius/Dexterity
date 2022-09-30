using System;
using System.Collections.Generic;

namespace Factors.Collections
{
    public interface IListImplementer<T> : ICollectionImplementer<T>, IList<T>
    {
        int Capacity { get; }

        List<T>       AsNormalList();
        int           AddObject(object value);
        int           BinarySearch(T item, IComparer<T> comparer, int startIndex, int count);
        int           BinarySearch(T item, IComparer<T> comparer);
        int           BinarySearch(T item);
        List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter);
        bool          Exists(Predicate<T> predicate);
        T             Find(Predicate<T> predicate);
        List<T>       FindAll(Predicate<T> predicate);
        int           FindIndex(Predicate<T> predicate, int startIndex, int count);
        int           FindIndex(Predicate<T> predicate, int startIndex);
        int           FindIndex(Predicate<T> predicate);
        int           FindLastIndex(Predicate<T> predicate, int startIndex, int count);
        int           FindLastIndex(Predicate<T> predicate, int startIndex);
        int           FindLastIndex(Predicate<T> predicate);
        T             FindLast(Predicate<T> predicate);
        void          ForEach(Action<T> action);
        List<T>       GetRange(int startIndex, int numberOfItems);
        void          InsertRange(int index, IEnumerable<T> itemsToInsert);
        int           LastIndexOf(T item, int startIndex, int count);
        int           LastIndexOf(T item, int startIndex);
        int           LastIndexOf(T item);
        int           RemoveAll(Predicate<T> predicate);
        void          RemoveRange(int startingIndex, int numberOfItemsToRemove);
        void          Reverse(int index, int count);
        void          Reverse();
        void          Sort(int index, int count, IComparer<T> comparer);
        void          Sort(Comparison<T> comparison);
        void          Sort(IComparer<T> comparer);
        void          Sort();
        T[]           ToArray();
        bool          TrueForAll(Predicate<T> predicate);
        void          TrimExcess();
    }
}