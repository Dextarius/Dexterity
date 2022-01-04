using System;
using System.Collections.Generic;

namespace Core.States
{
    public interface IReadOnlyListMembers<T>
    {
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
        void          ForEach(Action<T>    action);
        List<T>       GetRange(int startIndex, int count);
        int           IndexOf(T item);
        int           LastIndexOf(T item, int startIndex, int count);
        int           LastIndexOf(T item, int startIndex);
        int           LastIndexOf(T item);
        T[]           ToArray();
        bool          TrueForAll(Predicate<T> predicate);
    }
}