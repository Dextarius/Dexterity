﻿using System.Collections;
using System.Collections.Generic;
using Core.Redirection;
using Core.States;
using Dextarius.Collections;
using Factors.Collections;
using static Core.Tools.Types;

namespace Factors.Cores.ProactiveCores
{
    public class ObservedDictionaryCore<TKey, TValue> : 
        ObservedCollectionCore<Dictionary<TKey,TValue>, KeyValuePair<TKey, TValue>>, IDictionaryCore<TKey, TValue>
    {
        #region Instance Fields

        private readonly IEqualityComparer<TValue> valueComparer;
        private          StateKeyConservator       keys;
        private          StateValueConservator     values;

        #endregion
        

        #region Instance Properties
        
        public ICollection<TKey>   Keys   => keys   ??= new StateKeyConservator(this);
        public ICollection<TValue> Values => values ??= new StateValueConservator(this);

        public TValue this[TKey key]
        {
            get => Collection[key];
            set
            {
                if (collection.TryGetValue(key, out TValue currentValue) is false || 
                    valueComparer.Equals(value, currentValue) is false)
                {
                    collection[key] = value;
                    OnCollectionChanged();
                }
            }
        }

        #endregion
        

        #region Instance Methods

        public void Add(TKey key, TValue value)
        {
            collection.Add(key, value);
            OnCollectionChanged();
        }

        public bool Remove(TKey key)
        {
            if (collection.Remove(key))
            {
                NotifyChanged();
                return true;
            }
            else return false;
        }

        public bool TryGetValue(TKey key, out TValue value) => Collection.TryGetValue(key, out value);
        public bool ContainsKey(TKey key)                   => Collection.ContainsKey(key);
        public bool ContainsValue(TValue key)               => Collection.ContainsValue(key);
        
        public ICollection GetKeysAsICollection()   => keys   ??= new StateKeyConservator(this);
        public ICollection GetValuesAsICollection() => values ??= new StateValueConservator(this);
        
        public new IDictionaryEnumerator GetEnumerator() => new FactorDictionaryEnumerator(this, Collection.GetEnumerator());
        //- TODO : Decide if we really want to make an enumerator object every time someone calls this method.  If not then 
        //         we can just can Collection to IDictionary and get the enumerator from it.

        #endregion
        

        #region Constructors
        
        protected ObservedDictionaryCore(Dictionary<TKey, TValue> dictionaryToCopy, IEqualityComparer<TValue> comparerForValues) : 
            base(dictionaryToCopy)
        {
            valueComparer = comparerForValues ?? EqualityComparer<TValue>.Default;
        }
        
        public ObservedDictionaryCore(IEnumerable<KeyValuePair<TKey, TValue>> collectionToCopy  = null,
                                      IEqualityComparer<TKey>                 comparerForKeys   = null, 
                                      IEqualityComparer<TValue>               comparerForValues = null) : 
            this(new Dictionary<TKey, TValue>(collectionToCopy, comparerForKeys ?? EqualityComparer<TKey>.Default), 
                 comparerForValues)
        {
            
        }
        
        public ObservedDictionaryCore(ICollection<KeyValuePair<TKey, TValue>> collectionToCopy, 
                                      IEqualityComparer<TValue>               comparerForValues) : 
            this(collectionToCopy, null, comparerForValues)
        {
        }
        
        public ObservedDictionaryCore(IEqualityComparer<TKey>   comparerForKeys, 
                                      IEqualityComparer<TValue> comparerForValues = null) : 
            this(new Dictionary<TKey, TValue>(comparerForKeys ?? EqualityComparer<TKey>.Default), comparerForValues)
        {
        }

        public ObservedDictionaryCore(IEqualityComparer<TValue> comparerForValues) : 
            this(null, null, comparerForValues)
        {
        }
        
        public ObservedDictionaryCore(Dictionary<TKey, TValue>  dictionaryToCopy,
                                      IEqualityComparer<TKey>   comparerForKeys   = null, 
                                      IEqualityComparer<TValue> comparerForValues = null) : 
            this(new Dict<TKey, TValue>(dictionaryToCopy, comparerForKeys ?? dictionaryToCopy.Comparer), comparerForValues)
        {
        }

        public ObservedDictionaryCore() : this(new Dictionary<TKey, TValue>(), null)
        {
        }

        #endregion


        #region Nested Classes

        protected class StateKeyConservator : ReadOnlyConservator< ObservedDictionaryCore<TKey, TValue>, TKey>
        {
            public override ICollection<TKey> ManagedCollection => collectionSource.Collection.Keys;
            
            protected override string GetCollectionDescription() => "The Keys collection of a ProactiveDictionary";
            
            internal StateKeyConservator(ObservedDictionaryCore<TKey, TValue> creator) : base(creator) { }
        }


        protected class StateValueConservator : ReadOnlyConservator< ObservedDictionaryCore<TKey, TValue>, TValue>
        {
            public override ICollection<TValue> ManagedCollection => collectionSource.Collection.Values;

            protected override string GetCollectionDescription() => "The Values collection of a ProactiveDictionary";

            internal StateValueConservator(ObservedDictionaryCore<TKey, TValue> creator) : base(creator) { }
        }

        #endregion
    }
}