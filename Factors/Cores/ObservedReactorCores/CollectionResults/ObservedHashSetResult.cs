using System.Collections.Generic;
using Core.States;
using static Factors.CollectionFactor;

namespace Factors.Cores.ObservedReactorCores.CollectionResults
{
    public abstract class ObservedHashSetResult<T> : ObservedCollectionResult<HashSet<T>, T>, ISetResult<T>
    {
        #region Instance Fields

        protected readonly IEqualityComparer<T> elementComparer;

        #endregion
        
        
        #region Instance Methods

        public HashSet<T> AsNormalSet() => new HashSet<T>(Collection);

        protected override HashSet<T> CreateCollectionFromElements(IEnumerable<T> elements) => 
            new HashSet<T>(elements, elementComparer);

        protected override bool AreCollectionsEqual(HashSet<T> set1, HashSet<T> set2, out long triggerFlags) => 
            HaveSameItems(set1, set2, out triggerFlags);
        
        public bool IsProperSupersetOf(IEnumerable<T> other) => Collection.IsProperSupersetOf(other);
        public bool   IsProperSubsetOf(IEnumerable<T> other) => Collection.IsProperSubsetOf(other);
        public bool       IsSupersetOf(IEnumerable<T> other) => Collection.IsSupersetOf(other);
        public bool         IsSubsetOf(IEnumerable<T> other) => Collection.IsSubsetOf(other);
        public bool          SetEquals(IEnumerable<T> other) => Collection.SetEquals(other);
        public bool           Overlaps(IEnumerable<T> other) => Collection.Overlaps(other);

        #endregion


        #region Constructors

        protected ObservedHashSetResult(IEqualityComparer<T> comparerForElements = null)
        {
            elementComparer   = comparerForElements ?? EqualityComparer<T>.Default;
            currentCollection = new HashSet<T>();
        }

        #endregion
    }
}