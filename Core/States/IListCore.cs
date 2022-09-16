using System;
using System.Collections.Generic;

namespace Core.States
{
    public interface IListCore<T> : IProactiveCollectionCore<T>, IList<T>
    {
        #region Properties

        int Capacity { get; }

        #endregion
        

        #region Instance Methods

        int     AddObject(object value);
        List<T> AsNormalList();
        void    InsertRange(int index, IEnumerable<T> elements);
        int     RemoveAll(Predicate<T> predicate);
        void    RemoveRange(int index, int count);
        void    Reverse(int index, int count);
        void    Reverse();
        void    Sort(int index, int count, IComparer<T> comparer);
        void    Sort(Comparison<T> comparison);
        void    Sort(IComparer<T> comparer);
        void    Sort();
        
        T       Find(Predicate<T> predicate);
        int     FindIndex(Predicate<T> predicate, int startIndex, int count);
        int     FindIndex(Predicate<T> predicate, int startIndex);
        int     FindIndex(Predicate<T> predicate);
        int     LastIndexOf(T item, int startIndex, int count);
        int     LastIndexOf(T item, int startIndex);
        int     LastIndexOf(T item);
        int     FindLastIndex(Predicate<T> predicate, int startIndex, int count);
        int     FindLastIndex(Predicate<T> predicate, int startIndex);
        int     FindLastIndex(Predicate<T> predicate);
        T       FindLast(Predicate<T> predicate);
        int     BinarySearch(T item, IComparer<T> comparer, int startIndex, int count);
        int     BinarySearch(T item, IComparer<T> comparer);
        int     BinarySearch(T item);
        List<T> FindAll(Predicate<T> predicate);
        
        void    TrimExcess();
        bool    TrueForAll(Predicate<T> predicate);
        bool    Exists(Predicate<T> predicate);

        void    ForEach(Action<T> action);
        List<T> GetRange(int startIndex, int count);
        T[]     ToArray();

        List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter);

        #endregion
    }
}