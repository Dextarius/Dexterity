using System.Collections;
using System.Collections.Generic;
using Core.Redirection;
using Core.States;
using Dextarius.Collections;
using Factors.Collections;
using static Dextarius.Utilities.Types;
using static Dextarius.Collections.Utilities;

namespace Factors.Cores.ProactiveCores
{
    public class ObservedProactiveDictionaryCore<TKey, TValue> : 
        ObservedProactiveCollectionCore<Dictionary<TKey,TValue>, KeyValuePair<TKey, TValue>, 
                                        IDictionaryImplementer<TKey, TValue>>, 
        IDictionaryCore<TKey, TValue>, IDictionaryOwner<TKey, TValue>
    {
        #region Instance Properties

        public ICollection<TKey>   Keys   => implementer.Keys;
        public ICollection<TValue> Values => implementer.Values;

        public TValue this[TKey index]
        {
            get => implementer[index];
            set => implementer[index] = value;
        }

        #endregion
        

        #region Instance Methods

        public     void                  Add(TKey key, TValue value)             => implementer.Add(key, value);
        public     bool                  Remove(TKey key)                        => implementer.Remove(key);
        public     bool                  TryGetValue(TKey key, out TValue value) => implementer.TryGetValue(key, out value);
        public     bool                  ContainsKey(TKey keyToLookFor)          => implementer.ContainsKey(keyToLookFor);
        public     bool                  ContainsValue(TValue valueToLookFor)    => implementer.ContainsValue(valueToLookFor);
        public     ICollection           GetKeysAsICollection()                  => implementer.GetKeysAsICollection();
        public     ICollection           GetValuesAsICollection()                => implementer.GetValuesAsICollection();
        public new IDictionaryEnumerator GetEnumerator()                         => implementer.GetEnumerator();

        #endregion
        

        #region Constructors
        
        protected ObservedProactiveDictionaryCore(IDictionaryImplementer<TKey, TValue> implementation)
        {
            implementer = implementation;
        }
        
        protected ObservedProactiveDictionaryCore(Dictionary<TKey, TValue>  dictionaryToCopy, 
                                                  IEqualityComparer<TValue> comparerForValues)
        {
            implementer = new DictionaryImplementer<TKey, TValue>(this, dictionaryToCopy, comparerForValues);
        }
        
        public ObservedProactiveDictionaryCore(IEnumerable<KeyValuePair<TKey, TValue>> collectionToCopy  = null,
                                               IEqualityComparer<TKey>                 comparerForKeys   = null, 
                                               IEqualityComparer<TValue>               comparerForValues = null) : 
            this(CreateDictionaryFrom(collectionToCopy, comparerForKeys ?? EqualityComparer<TKey>.Default), 
                 comparerForValues)
        {
            
        }
        
        public ObservedProactiveDictionaryCore(ICollection<KeyValuePair<TKey, TValue>> collectionToCopy, 
                                               IEqualityComparer<TValue>               comparerForValues) : 
            this(collectionToCopy, null, comparerForValues)
        {
        }
        
        public ObservedProactiveDictionaryCore(IEqualityComparer<TKey>   comparerForKeys, 
                                               IEqualityComparer<TValue> comparerForValues = null) : 
            this(new Dictionary<TKey, TValue>(comparerForKeys ?? EqualityComparer<TKey>.Default), comparerForValues)
        {
        }

        public ObservedProactiveDictionaryCore(IEqualityComparer<TValue> comparerForValues) : 
            this(null, null, comparerForValues)
        {
        }
        
        public ObservedProactiveDictionaryCore(Dictionary<TKey, TValue>  dictionaryToCopy,
                                               IEqualityComparer<TKey>   comparerForKeys   = null, 
                                               IEqualityComparer<TValue> comparerForValues = null) : 
            this(new Dict<TKey, TValue>(dictionaryToCopy, comparerForKeys ?? dictionaryToCopy.Comparer), comparerForValues)
        {
        }

        public ObservedProactiveDictionaryCore() : this(new Dictionary<TKey, TValue>(), null)
        {
        }

        #endregion
    }
}