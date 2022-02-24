using System.Collections;
using System.Collections.Generic;
using Core.Redirection;
using Core.States;
using Core.Tools;
using Factors.Collections;

namespace Factors.Cores.ObservedReactorCores.CollectionResults
{
    public abstract class ObservedDictionaryResult<TKey, TValue> : 
        ObservedCollectionResult<Dictionary<TKey,TValue>, KeyValuePair<TKey, TValue>>,
        IDictionaryResult<TKey, TValue>
    {
        #region Instance Fields

        private readonly IEqualityComparer<TKey>   keyComparer;
        private readonly IEqualityComparer<TValue> valueComparer;
        private          OutcomeKeyConservator     keys;
        private          OutcomeValueConservator   values;

        #endregion
        
        
        #region Properties

        public TValue this[TKey key] => Collection[key];
        
        public ICollection<TKey>   Keys   => keys   ??= new OutcomeKeyConservator(this);
        public ICollection<TValue> Values => values ??= new OutcomeValueConservator(this);
        //^ The keys/value collections always represents the CURRENT dictionary keys/values (that's important to remember).
        //  This is not a static copy of what keys were present when it was called!

        #endregion


        #region Instance Methods

        public Dictionary<TKey, TValue> AsNormalDictionary()                    => new Dictionary<TKey, TValue>(Collection);
        public bool                     TryGetValue(TKey key, out TValue value) => Collection.TryGetValue(key, out value);
        public bool                     ContainsKey(TKey key)                   => Collection.ContainsKey(key);
        public bool                     ContainsValue(TValue key)               => Collection.ContainsValue(key);
        public ICollection              GetKeysAsICollection()                  => keys   ??= new OutcomeKeyConservator(this);
        public ICollection              GetValuesAsICollection()                => values ??= new OutcomeValueConservator(this);

        public new IDictionaryEnumerator GetEnumerator() => new FactorDictionaryEnumerator(this, Collection.GetEnumerator());
        //- TODO : Decide if we really want to make an enumerator object every time someone calls this method.

        protected override Dictionary<TKey, TValue> CreateCollectionFromElements(
            IEnumerable<KeyValuePair<TKey, TValue>> elements) => 
                elements.ToDictionary(keyComparer);

        protected override bool AreCollectionsEqual
            (Dictionary<TKey, TValue> dictionary1, Dictionary<TKey, TValue> dictionary2) =>
                dictionary1.HasSameKeysAndValuesAs(dictionary2, valueComparer);

        #endregion


        #region Constructors

        protected ObservedDictionaryResult(string name,
                                           IEqualityComparer<TKey>   comparerForKeys   = null, 
                                           IEqualityComparer<TValue> comparerForValues = null) : 
            base(name)
        {
            currentCollection = new Dictionary<TKey, TValue>();
            valueComparer     = comparerForValues ?? EqualityComparer<TValue>.Default;
            keyComparer       = comparerForKeys;
            //- If keyComparer is null, the dictionary will use the standard comparer for the Dictionary type.
        }

        protected ObservedDictionaryResult(string name, IEqualityComparer<TValue> comparerForValues) : 
            this(name, EqualityComparer<TKey>.Default, comparerForValues)
        {
        }

        #endregion
        

        #region Nested Classes

        protected class OutcomeKeyConservator : ReadOnlyConservator< ObservedDictionaryResult<TKey, TValue>, TKey>
        {
            public override ICollection<TKey> ManagedCollection => collectionSource.Collection.Keys;
            
            protected override string GetCollectionDescription() => "The Keys collection of a ReactiveDictionary";

            internal OutcomeKeyConservator(ObservedDictionaryResult<TKey, TValue> creator) : base(creator) { }

            
            //- TODO : Consider if there is any reason a collection belonging to a Reactor would be mutable.
            
            //- TODO : We need to test the workings of GetEnumerator() to make sure that the reactive collection's
            //         outcome notifies the observer it's involved each time it accesses an element of the collection.
        }
        
        protected class OutcomeValueConservator : ReadOnlyConservator< ObservedDictionaryResult<TKey, TValue>, TValue>
        {
            public override ICollection<TValue> ManagedCollection => collectionSource.Collection.Values;
            
            protected override string GetCollectionDescription() => "The Values collection of a ReactiveDictionary";

            internal OutcomeValueConservator(ObservedDictionaryResult<TKey, TValue> creator) : base(creator) { }
        }

        #endregion
    }
}