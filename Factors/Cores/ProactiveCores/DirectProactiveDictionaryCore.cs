using System.Collections;
using System.Collections.Generic;
using Core.Redirection;
using Core.States;
using Dextarius.Collections;
using Factors.Collections;
using static Dextarius.Collections.Utilities;
using static Factors.TriggerFlags;

namespace Factors.Cores.ProactiveCores
{
    public class DirectProactiveDictionaryCore<TKey, TValue> : 
        DirectProactiveCollectionCore<Dictionary<TKey,TValue>, KeyValuePair<TKey, TValue>>
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
                if (collection.TryGetValue(key, out TValue currentValue))
                {
                    if (valueComparer.Equals(value, currentValue) is false)
                    {
                        collection[key] = value;
                        OnItemRemoved(new KeyValuePair<TKey, TValue>(key, currentValue));
                        OnItemAdded(new KeyValuePair<TKey, TValue>(key, value));
                        OnCollectionChanged(ItemAdded | ItemRemoved | ItemReplaced);
                        
                        //- TODO : Consider if we want to add a ValueReplaced() method or something
                        //         so that we aren't telling subscribers we're removing the key and
                        //         then re-adding it.  
                    }
                }
                else
                {
                    collection[key] = value;
                    OnItemAdded(new KeyValuePair<TKey, TValue>(key, value));
                    OnCollectionChanged(ItemAdded);
                }
            }
        }

        #endregion
        

        #region Instance Methods

        public void Add(TKey key, TValue value)
        {
            collection.Add(key, value);
            OnItemAdded(new KeyValuePair<TKey, TValue>(key, value));
            OnCollectionChanged(ItemAdded);
        }

        public bool Remove(TKey key)
        {
            if (collection.TryGetValue(key, out var value))
            {
                collection.Remove(key);
                OnItemRemoved(new KeyValuePair<TKey, TValue>(key, value));
                OnCollectionChanged(ItemRemoved);
                
                return true;
            }
            else return false;
        }
        
        protected override bool AddItem(KeyValuePair<TKey, TValue> itemToAdd, out long notifyInvolvedFlags, 
                                                                              out long additionalChangeFlags)
        {
            collection.Add(itemToAdd.Key, itemToAdd.Value);
            notifyInvolvedFlags   = TriggerFlags.ItemRemoved;
            additionalChangeFlags = TriggerFlags.None;
            
            return true;        
        }

        protected override bool RemoveItem(KeyValuePair<TKey, TValue> item, out long additionalTriggerFlags)
        {
            additionalTriggerFlags = TriggerFlags.None;
            return collection.Remove(item.Key);
        }

        public     bool                  TryGetValue(TKey key, out TValue value) => Collection.TryGetValue(key, out value);
        
        public     bool                  ContainsKey(TKey key)                   => Collection.ContainsKey(key);
        
        public     bool                  ContainsValue(TValue key)               => Collection.ContainsValue(key);
        
        public     ICollection           GetKeysAsICollection()                  => keys   ??= new StateKeyConservator(this);
        
        public     ICollection           GetValuesAsICollection()                => values ??= new StateValueConservator(this);
        
        public new IDictionaryEnumerator GetEnumerator()                         => ((IDictionary) Collection).GetEnumerator();

        #endregion
        

        #region Constructors
        
        protected DirectProactiveDictionaryCore(Dictionary<TKey, TValue>  dictionaryToCopy, 
                                                IEqualityComparer<TValue> comparerForValues) : 
            base(dictionaryToCopy)
        {
            valueComparer = comparerForValues ?? EqualityComparer<TValue>.Default;
        }
        
        public DirectProactiveDictionaryCore(IEnumerable<KeyValuePair<TKey, TValue>> collectionToCopy  = null,
                                             IEqualityComparer<TKey>                 comparerForKeys   = null, 
                                             IEqualityComparer<TValue>               comparerForValues = null) : 
            this(CreateDictionaryFrom(collectionToCopy, comparerForKeys ?? EqualityComparer<TKey>.Default), 
                 comparerForValues)
        {
        }
        
        public DirectProactiveDictionaryCore(ICollection<KeyValuePair<TKey, TValue>> collectionToCopy, 
                                             IEqualityComparer<TValue>               comparerForValues) : 
            this(collectionToCopy, null, comparerForValues)
        {
        }
        
        public DirectProactiveDictionaryCore(IEqualityComparer<TKey>   comparerForKeys, 
                                             IEqualityComparer<TValue> comparerForValues = null) : 
            this(new Dictionary<TKey, TValue>(comparerForKeys ?? EqualityComparer<TKey>.Default), comparerForValues)
        {
        }

        public DirectProactiveDictionaryCore(IEqualityComparer<TValue> comparerForValues) : 
            this(null, null, comparerForValues)
        {
        }
        
        public DirectProactiveDictionaryCore(Dictionary<TKey, TValue>  dictionaryToCopy,
                                             IEqualityComparer<TKey>   comparerForKeys   = null, 
                                             IEqualityComparer<TValue> comparerForValues = null) : 
            this(new Dict<TKey, TValue>(dictionaryToCopy, comparerForKeys ?? dictionaryToCopy.Comparer), comparerForValues)
        {
        }

        public DirectProactiveDictionaryCore() : this(new Dictionary<TKey, TValue>(), null)
        {
        }

        #endregion


        #region Nested Classes

        protected class StateKeyConservator : ReadOnlyConservator<DirectProactiveDictionaryCore<TKey, TValue>, TKey>
        {
            public override ICollection<TKey> ManagedCollection => collectionSource.Collection.Keys;
            
            protected override string GetCollectionDescription() => "The Keys collection of a ProactiveDictionary";
            
            internal StateKeyConservator(DirectProactiveDictionaryCore<TKey, TValue> creator) : base(creator) { }
        }


        protected class StateValueConservator : ReadOnlyConservator<DirectProactiveDictionaryCore<TKey, TValue>, TValue>
        {
            public override ICollection<TValue> ManagedCollection => collectionSource.Collection.Values;

            protected override string GetCollectionDescription() => "The Values collection of a ProactiveDictionary";

            internal StateValueConservator(DirectProactiveDictionaryCore<TKey, TValue> creator) : base(creator) { }
        }

        #endregion
    }
}