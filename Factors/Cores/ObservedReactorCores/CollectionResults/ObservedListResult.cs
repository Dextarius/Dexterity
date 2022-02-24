using System;
using System.Collections.Generic;
using Core.States;
using Core.Tools;

namespace Factors.Cores.ObservedReactorCores.CollectionResults
{
    public abstract class ObservedListResult<T> : ObservedCollectionResult<List<T>, T>, IListResult<T>
    {
        #region Instance Fields

        protected readonly IEqualityComparer<T> elementComparer;

        #endregion
        
        #region Properties

        public T this[int index] => Collection[index];

        #endregion
        
        #region Instance Methods

        protected override List<T> CreateCollectionFromElements(IEnumerable<T> newElements) => new List<T>(newElements);
        
        protected override bool AreCollectionsEqual(List<T> list1, List<T> list2) => 
            list1.IsEquivalentTo(list2, elementComparer);
        
        public List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter) => Collection.ConvertAll(converter);
        public List<T>       GetRange(int startIndex, int count)                  => Collection.GetRange(startIndex, count);
        public T[]           ToArray()                                            => Collection.ToArray();
        public bool          TrueForAll(Predicate<T> predicate)                   => Collection.TrueForAll(predicate);
        public bool          Exists(Predicate<T> predicate)                       => Collection.Exists(predicate);
        public T             Find(Predicate<T> predicate)                         => Collection.Find(predicate);
        public List<T>       FindAll(Predicate<T> predicate)                      => Collection.FindAll(predicate);
        public int           IndexOf(T item)                                      => Collection.IndexOf(item);
        public T             FindLast(Predicate<T> predicate)                     => Collection.FindLast(predicate);
        public void          ForEach(Action<T>    action)                         => Collection.ForEach(action);
        public List<T>       AsNormalList()                                       => new List<T>(Collection);
        //^ We may want to pause the Observer if it seems like the user is calling this to make a collection that
        //  creates no dependencies.
        
        public int LastIndexOf(T item, int startIndex, int count) => Collection.LastIndexOf(item, startIndex, count);
        public int LastIndexOf(T item, int startIndex)            => Collection.LastIndexOf(item, startIndex);
        public int LastIndexOf(T item)                            => Collection.LastIndexOf(item);
        
        public int FindIndex(Predicate<T> predicate, int startIndex, int count) => Collection.FindIndex(startIndex, count, predicate);
        public int FindIndex(Predicate<T> predicate, int startIndex)            => Collection.FindIndex(startIndex, predicate);
        public int FindIndex(Predicate<T> predicate)                            => Collection.FindIndex(predicate);
        
        public int FindLastIndex(Predicate<T> predicate, int startIndex, int count) => Collection.FindLastIndex(startIndex, count, predicate);
        public int FindLastIndex(Predicate<T> predicate, int startIndex)            => Collection.FindLastIndex(startIndex, predicate);
        public int FindLastIndex(Predicate<T> predicate)                            => Collection.FindLastIndex(predicate);

        public int BinarySearch(T item, IComparer<T> comparer, int startIndex, int count) => Collection.BinarySearch(startIndex, count, item, comparer);
        public int BinarySearch(T item, IComparer<T> comparer)                            => Collection.BinarySearch(item, comparer);  //- Should we use the Comparer we already have?
        public int BinarySearch(T item)                                                   => Collection.BinarySearch(item);
        
        #endregion
        

        #region Constructors

        protected ObservedListResult(string name, IEqualityComparer<T> comparerForElements = null) : base(name)
        {
            elementComparer   = comparerForElements ?? EqualityComparer<T>.Default;
            currentCollection = new List<T>();
        }

        #endregion
    }
}