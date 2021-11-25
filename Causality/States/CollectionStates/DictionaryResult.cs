using System.Collections.Generic;
using Core.Causality;
using Core.States;
using Core.Tools;

namespace Causality.States.CollectionStates
{
    public class DictionaryResult<TKey, TValue> :  
        CollectionResult< Dictionary<TKey,TValue>, KeyValuePair<TKey, TValue>>,
        IDictionaryResult<TKey, TValue>
    {
        #region Instance Fields

        private IEqualityComparer<TKey>   keyComparer;
        private IEqualityComparer<TValue> valueComparer;


        #endregion
        
        
        #region Properties

        public TValue this[TKey key]
        {
            get => Collection[key];
        }
        //^ The keys/value collections always represents the CURRENT dictionary keys/values (that's important to remember).
        //  This is not a static copy of what keys were present when it was called!

        #endregion


        #region Instance Methods

        public  bool TryGetValue(TKey key, out TValue value) => Collection.TryGetValue(key, out value);
        public  bool ContainsKey(TKey key)                   => Collection.ContainsKey(key);
        public  bool ContainsValue(TValue key)               => Collection.ContainsValue(key);
        
        protected override Dictionary<TKey, TValue> CreateCollectionFromElements(
            IEnumerable<KeyValuePair<TKey, TValue>> elements)
        {
            return elements.ToDictionary(keyComparer);
        }

        protected override bool AreCollectionsEqual(Dictionary<TKey, TValue> dictionary1, Dictionary<TKey, TValue> dictionary2)
        {
            return dictionary1.HasSameKeysAndValuesAs(dictionary2, valueComparer);
        }
        
        #endregion


        #region Constructors

        public DictionaryResult(
            object owner, 
            IProcess< IEnumerable< KeyValuePair<TKey, TValue>>> processToGenerateItems,
            IEqualityComparer<TValue> comparerForValues = null) :
            this(owner, processToGenerateItems, null, comparerForValues)
        {
        }

        public DictionaryResult(
            object owner, 
            IProcess< IEnumerable< KeyValuePair<TKey, TValue>>> processToGenerateItems, 
            IEqualityComparer<TKey>   comparerForKeys   = null, 
            IEqualityComparer<TValue> comparerForValues = null) : 
            base(owner, processToGenerateItems)
        {
            keyComparer   = comparerForKeys;  
            valueComparer = comparerForValues ??  EqualityComparer<TValue>.Default;
            
            //- if keyComparer is null, the dictionary will use the standard comparer for the Dictionary type.
            //  I'd like to use the reference equality comparer for the keys, but that would likely lead to 
            //  situations where the user would use keys that would be considered equivalent by a normal
            //  Dictionary to match, but wouldn't match if you used reference equality. 
        }

        #endregion
    }
}