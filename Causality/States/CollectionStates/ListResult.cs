using System;
using System.Collections.Generic;
using Core.Causality;
using Core.States;
using Core.Tools;

namespace Causality.States.CollectionStates
{
    public class ListResult<T> : CollectionResult<List<T>, T>, IListResult<T>
    {
        #region Instance Fields

        protected readonly IEqualityComparer<T> elementComparer;

        #endregion
        
        #region Properties

        public T this[int index] => Collection[index];

        #endregion
        
        #region Instance Methods

        public int IndexOf(T item) => Collection.IndexOf(item);

        protected override List<T> CreateCollectionFromElements(IEnumerable<T> newElements)
        {
            return new List<T>(newElements);
        }
        protected override bool AreCollectionsEqual(List<T> list1, List<T> list2) => 
            list1.IsEquivalentTo(list2, elementComparer);

        #endregion

        #region Constructors

        public ListResult(object owner, IProcess<IEnumerable<T>> processToGenerateItems, 
                           IEqualityComparer<T> comparerForElements) : 
            base(owner, processToGenerateItems)
        {
            elementComparer = comparerForElements ?? EqualityComparer<T>.Default;
        }

        #endregion
    }
}