using System;
using System.Collections.Generic;
using Core.Causality;
using Core.States;

namespace Causality.States.CollectionStates
{
    public class HashSetResult<T> : CollectionResult<HashSet<T>, T>, IHashsetResult<T>
    {
        #region Instance Fields

        protected IEqualityComparer<T> itemComparer;

        #endregion
        
        #region Instance Methods

        protected override HashSet<T> CreateCollectionFromElements(IEnumerable<T> elements) => 
            new HashSet<T>(elements, itemComparer);

        protected override bool AreCollectionsEqual(HashSet<T> set1, HashSet<T> set2)
        {
            return set1.SetEquals(set2);
        }

        #endregion


        #region Constructors

        public HashSetResult(
            object owner, IProcess<IEnumerable<T>> processToGenerateItems, IEqualityComparer<T> comparerForItems) : 
            base(owner, processToGenerateItems)
        {
            itemComparer = comparerForItems; //- It's fine if it's null.  Hashsets handle null comparers.
            
            //- TODO : Decide if we should be using Default<T>.EqualityComparer if comparerForItems is null.
            //         We probably shouldn't use the ReferenceEqualityComparer though, people are probably going
            //         to expect the same equality behaviour as a normal HashSet.
        }

        #endregion
    }
}