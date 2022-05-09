using System;
using System.Collections.Generic;

namespace Core.States
{
    public interface IListCore<T> : ICollectionCore<T>, IReadOnlyListMembers<T>
    {
        #region Properties

        T   this[int index] { get; set; }
        int Capacity        { get; }

        #endregion
        

        #region Instance Methods

        int     AddObject(object value);
        List<T> AsNormalList();
        void    Insert(int index, T item);
        void    InsertRange(int index, IEnumerable<T> elements);
        void    RemoveAt(int index);
        int     RemoveAll(Predicate<T> predicate);
        void    RemoveRange(int index, int count);
        void    Reverse(int index, int count);
        void    Reverse();
        void    Sort(int index, int count, IComparer<T> comparer);
        void    Sort(Comparison<T> comparison);
        void    Sort(IComparer<T> comparer);
        void    Sort(); 
        void    TrimExcess();

        #endregion
    }
}