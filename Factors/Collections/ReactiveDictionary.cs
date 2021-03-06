using System;
using System.Collections;
using System.Collections.Generic;
using Causality.Processes;
using Core.Causality;
using Core.Factors;
using Core.Tools;
using Factors.Exceptions;
using JetBrains.Annotations;

namespace Factors.Collections
{
    public partial class ReactiveDictionary<TKey, TValue> : ReactiveCollection<Dictionary<TKey,TValue>, KeyValuePair<TKey, TValue>>, 
        IDictionary, IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
    {
        #region Instance Fields

        private ReactiveKeyConservator   keys;
        private ReactiveValueConservator values;

        #endregion


        #region Static Properties

        public static IEqualityComparer<KeyValuePair<TKey,TValue>> DefaultValueComparer = 
            EqualityComparer<KeyValuePair<TKey,TValue>>.Default;

        #endregion
        

        #region Instance Properties

        public TValue              this[TKey key] => Collection[key];
        public ICollection<TKey>   Keys   => keys ??= CreateKeysCollection();
        public ICollection<TValue> Values => values ??= CreateValuesCollection();
        //^ The keys/value collections always represents the CURRENT dictionary keys/values (that's important to remember).
        //  This is not a static copy of what keys were present when it was called!

        #endregion
        

        #region Instance Methods

        public bool                     TryGetValue(TKey key, out TValue value) => Collection.TryGetValue(key, out value);
        public bool                     ContainsKey(TKey key)                   => Collection.ContainsKey(key);
        public bool                     ContainsValue(TValue key)               => Collection.ContainsValue(key);
        public Dictionary<TKey, TValue> AsNormalDictionary()                    => new Dictionary<TKey, TValue>
        (Collection); 

        private ReactiveValueConservator CreateValuesCollection() => new ReactiveValueConservator(this);
        private ReactiveKeyConservator   CreateKeysCollection()   => new ReactiveKeyConservator(this);
        
        
        
        protected override Dictionary<TKey, TValue> CreateCollectionFromElements(IEnumerable<KeyValuePair<TKey, TValue>> elements)
        {
            return elements.ToDictionary();
        }

        
        protected override bool AreCollectionsEqual(Dictionary<TKey, TValue> collection1, Dictionary<TKey, TValue> collection2)
        {
            return collection1.Equals(collection2) == false;
        }
        
        public new IDictionaryEnumerator GetEnumerator() => new FactorDictionaryEnumerator(outcome, Collection.GetEnumerator());

        #endregion
        

        #region Constructors

        public ReactiveDictionary([NotNull] Func< IEnumerable< KeyValuePair<TKey, TValue>>> functionToCreateKeyValuePairs, 
                                   string name = null) :
            this(FunctionalProcess.CreateFrom(functionToCreateKeyValuePairs), DefaultValueComparer,
                 name ?? CreateDefaultName<ReactiveDictionary<TKey, TValue>>(functionToCreateKeyValuePairs))
        {
        }
        
        public ReactiveDictionary([NotNull] Func< IEnumerable< KeyValuePair<TKey, TValue>>> functionToCreateKeyValuePairs,
                                  IEqualityComparer< KeyValuePair<TKey, TValue>> comparer, string name = null) :
            this(FunctionalProcess.CreateFrom(functionToCreateKeyValuePairs), comparer, 
                name ?? CreateDefaultName<ReactiveDictionary<TKey, TValue>>(functionToCreateKeyValuePairs))
        {
        }

        public ReactiveDictionary(IProcess< IEnumerable< KeyValuePair<TKey, TValue>>> processToGenerateItems, string name = null) : 
            this(processToGenerateItems, DefaultValueComparer, name)
        {
        }

        public ReactiveDictionary([NotNull] IProcess< IEnumerable< KeyValuePair<TKey, TValue>>> processToGenerateItems, 
                                  IEqualityComparer< KeyValuePair<TKey, TValue>> comparer, string name = null) : 
            base(processToGenerateItems, comparer, name)
        {
        }

        #endregion
        

        #region Explicit Implementations

        TValue IDictionary<TKey, TValue>.this[TKey key]
        {
            get => this[key];
            set => throw new CannotModifyReactiveValueException();
        }
        
        IEnumerable<TKey>   IReadOnlyDictionary<TKey, TValue>.Keys   => Keys;
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value) => throw new CannotModifyReactiveValueException();
        bool IDictionary<TKey, TValue>.Remove(TKey key)            => throw new CannotModifyReactiveValueException();
        

        ICollection IDictionary.Keys        => keys;
        ICollection IDictionary.Values      => values;
        bool        IDictionary.IsReadOnly  => true;
        bool        IDictionary.IsFixedSize => true;

        void IDictionary.Add(object key, object value) => throw new CannotModifyReactiveValueException();
        void IDictionary.Remove(object key)            => throw new CannotModifyReactiveValueException();
        void IDictionary.Clear()                       => throw new CannotModifyReactiveValueException();
        bool IDictionary.Contains(object key)          => ((IDictionary)Collection).Contains(key);
        
        object IDictionary.this[object key]
        {
            get => ((IDictionary)Collection)[key];
            set => throw new CannotModifyReactiveValueException();
        }

        #endregion
    }
}