using System;
using System.Collections;
using System.Collections.Generic;
using Core.Factors;
using Core.Redirection;
using Dextarius.Collections;
using static Factors.TriggerFlags;
using static Dextarius.Utilities.Types;
using static Dextarius.Collections.Utilities;

namespace Factors.Collections
{
    public class DictionaryImplementer <TKey, TValue> : 
        CollectionImplementer<Dictionary<TKey,TValue>, KeyValuePair<TKey, TValue>, IDictionaryOwner<TKey, TValue>>, 
        IDictionaryImplementer<TKey, TValue>, IDictionary<TKey, TValue>, IDictionary
    {
        #region Instance Fields

        private readonly IEqualityComparer<TValue> valueComparer;
        private          StateKeyConservator       keys;
        private          StateValueConservator     values;

        #endregion


        #region Instance Properties

        public ICollection<TKey>   Keys        => keys   ??= new StateKeyConservator(this);
        public ICollection<TValue> Values      => values ??= new StateValueConservator(this);
        public bool                IsFixedSize => false;
        
        public TValue this[TKey key]
        {
            get
            {
                var currentValue = Collection[key];
                    
                NotifyInvolved(TriggerWhenItemRemoved | TriggerWhenItemReplaced);
                return currentValue;
            }
            set
            {
                long triggerFlags = TriggerFlags.None;
                
                if (collection.TryGetValue(key, out TValue currentValue) is false)
                {
                    if (valueComparer.Equals(value, currentValue) is false)
                    {
                        collection[key] = value;
                        triggerFlags    = TriggerFlags.ItemReplaced | TriggerFlags.ItemRemoved | TriggerFlags.ItemAdded;
                        Owner.OnItemRemoved(new KeyValuePair<TKey, TValue>(key, currentValue));
                    }
                }
                else
                {
                    collection[key] = value;
                    triggerFlags    = TriggerFlags.ItemAdded;
                }

                if (triggerFlags is not TriggerFlags.None)
                {
                    Owner.OnItemAdded(new KeyValuePair<TKey, TValue>(key, value));
                    Owner.OnCollectionChanged(triggerFlags);
                    
                }
            }
        }

        #endregion
        

        #region Instance Methods

        public void Add(TKey key, TValue value)
        {
            collection.Add(key, value);
            Owner.OnItemAdded(new KeyValuePair<TKey, TValue>(key, value));
            Owner.OnCollectionChanged(TriggerFlags.ItemAdded);
        }

        public bool Remove(TKey key)
        {
            if (Collection.TryGetValue(key, out var value))
            {
                collection.Remove(key);
                Owner.OnItemRemoved(new KeyValuePair<TKey, TValue>(key, value));
                Owner.OnCollectionChanged(TriggerFlags.ItemRemoved);
                return true;
            }
            else return false;
        }
        
        protected override bool AddItem(KeyValuePair<TKey, TValue> item, out long notifyInvolvedFlags, 
                                                                         out long additionalChangeFlags)
        {
            collection.Add(item.Key, item.Value);
            //- If it already exists the collection will throw an exception.
            //- TODO : Consider if we want to deviate from the behavior of the standard dictionary
            //         by not exploding when someone tries to add an existing key.
            //         If the key already exists, should we replace the value?
            //         Not sure whether I'd return true or false in that case though. 

            //- Hmm, the very fact of this method succeeded means it's already
            //  set up for the return value to be different the next time it's
            //  called.  So what do we consider that in terms of trigger flags?
            //  If they call it again before this new key is removed the
            //  dictionary will throw anyways, so I guess the only reasonable
            //  trigger would be when an item is removed.
            
            notifyInvolvedFlags   = TriggerFlags.ItemRemoved;
            additionalChangeFlags = TriggerFlags.None;
            return true;
        }
        
        protected override bool RemoveItem(KeyValuePair<TKey, TValue> item, out long additionalTriggerFlags)
        {
            additionalTriggerFlags = TriggerFlags.None;
            return collection.Remove(item.Key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            bool wasSuccessful = Collection.TryGetValue(key, out value);

            NotifyInvolved_ContainsItem(wasSuccessful);
            return wasSuccessful;
        }
        
        public bool ContainsKey(TKey keyToLookFor)
        {
            bool wasSuccessful = Collection.ContainsKey(keyToLookFor);

            if (wasSuccessful) { NotifyInvolved(TriggerFlags.ItemRemoved);                           }
            else               { NotifyInvolved(TriggerFlags.ItemAdded | TriggerFlags.ItemReplaced); }
            
            return wasSuccessful;
        }
        
        public bool ContainsValue(TValue valueToLookFor)
        {
            bool wasSuccessful = Collection.ContainsValue(valueToLookFor);

            NotifyInvolved_ContainsItem(wasSuccessful);
            return wasSuccessful;
        }

        public override bool CollectionEquals(IEnumerable<KeyValuePair<TKey, TValue>> collectionToCompare)
        {
            var  dictionaryToCompare = CreateDictionaryFrom(collectionToCompare, collection.Comparer);
            bool isEqual = this.collection.HasSameKeysAndValuesAs(dictionaryToCompare, valueComparer);
            
            NotifyInvolved(TriggerWhenItemRemoved | TriggerWhenItemReplaced | TriggerWhenItemAdded);
            //- Note : We can make the flags more specific if we compare the Dictionaries ourselves and 
            //         see whether this collection has too many items, too few items, etc.
            return isEqual;
        }
        
        public     ICollection           GetKeysAsICollection()   => keys   ??= new StateKeyConservator(this);

        public     ICollection           GetValuesAsICollection() => values ??= new StateValueConservator(this);
        
        public new IDictionaryEnumerator GetEnumerator()          => new FactorDictionaryEnumerator(this, Collection.GetEnumerator());
        
        //^ TODO : Decide if we really want to make our own enumerator object every time someone calls this method.
        //        If not then we can just cast Collection to IDictionary and get the enumerator from it.
        
        #endregion
        

        #region Constructors
        
        protected DictionaryImplementer(IDictionaryOwner<TKey, TValue> owner,
                                        Dictionary<TKey, TValue>       dictionaryToCopy, 
                                        IEqualityComparer<TValue>      comparerForValues) : 
            base(dictionaryToCopy, owner)
        {
            valueComparer = comparerForValues ?? EqualityComparer<TValue>.Default;
        }
        
        public DictionaryImplementer(IDictionaryOwner<TKey, TValue>          owner,
                                     IEnumerable<KeyValuePair<TKey, TValue>> collectionToCopy  = null,
                                     IEqualityComparer<TKey>                 comparerForKeys   = null, 
                                     IEqualityComparer<TValue>               comparerForValues = null) : 
            this(owner, CreateDictionaryFrom(collectionToCopy, comparerForKeys ?? EqualityComparer<TKey>.Default), 
                 comparerForValues)
        {
        }
        
        public DictionaryImplementer(IDictionaryOwner<TKey, TValue>          owner,
                                     ICollection<KeyValuePair<TKey, TValue>> collectionToCopy, 
                                     IEqualityComparer<TValue>               comparerForValues) : 
            this(owner, collectionToCopy, null, comparerForValues)
        {
        }
        
        public DictionaryImplementer(IDictionaryOwner<TKey, TValue> owner,
                                     IEqualityComparer<TKey>        comparerForKeys, 
                                     IEqualityComparer<TValue>      comparerForValues = null) : 
            this(owner, new Dictionary<TKey, TValue>(comparerForKeys ?? EqualityComparer<TKey>.Default), comparerForValues)
        {
        }

        public DictionaryImplementer(IDictionaryOwner<TKey, TValue> owner,
                                     IEqualityComparer<TValue>      comparerForValues) : 
            this(owner, null, null, comparerForValues)
        {
        }

        public DictionaryImplementer(IDictionaryOwner<TKey, TValue> owner,
                                     Dictionary<TKey, TValue>       dictionaryToCopy,
                                     IEqualityComparer<TKey>        comparerForKeys = null,
                                     IEqualityComparer<TValue>      comparerForValues = null) :
            this(owner,
                new Dict<TKey, TValue>(dictionaryToCopy,
                    comparerForKeys ?? dictionaryToCopy.Comparer),
                comparerForValues)
        {
        }

        #endregion


        #region Nested Classes

        protected class StateKeyConservator : ReadOnlyConservator< DictionaryImplementer<TKey, TValue>, TKey>
        {
            public override ICollection<TKey> ManagedCollection => collectionSource.Collection.Keys;
            
            protected override string GetCollectionDescription() => "The Keys collection of a ProactiveDictionary";
            
            internal StateKeyConservator(DictionaryImplementer<TKey, TValue> creator) : base(creator) { }
        }


        protected class StateValueConservator : ReadOnlyConservator< DictionaryImplementer<TKey, TValue>, TValue>
        {
            public override ICollection<TValue> ManagedCollection => collectionSource.Collection.Values;

            protected override string GetCollectionDescription() => "The Values collection of a ProactiveDictionary";

            internal StateValueConservator(DictionaryImplementer<TKey, TValue> creator) : base(creator) { }
        }

        #endregion


        #region Explicit Implementations

        ICollection IDictionary.Values => values;
        ICollection IDictionary.Keys   => keys;

        object IDictionary.this[object key]
        {
            get
            {
                if (key is TKey keyOfCorrectType)
                {
                    return this[keyOfCorrectType];
                }
                else throw new ArgumentException( 
                    $"A process attempted to set a key in a {NameOf<ProactiveDictionary<TKey, TValue>>()}, " + 
                    $"through {nameof(IDictionary)}, but the key provided was of type {key.GetType()}. "); 
            }
            set
            {
                if (key is TKey keyOfCorrectType)
                {
                    if (value is TValue valueOfCorrectType)
                    {
                        this[keyOfCorrectType] = valueOfCorrectType;
                    }
                    else
                    {
                        throw new ArgumentException($"A process attempted to set the value for key {key}" +
                                                    $"in a {NameOf<ProactiveDictionary<TKey, TValue>>()}, " +
                                                    $"but the value provided was of type {value.GetType()}. ");  
                    }
                }
                else
                {
                    throw new ArgumentException( "A process attempted to set a key in a " +
                                                 $"{NameOf<ProactiveDictionary<TKey, TValue>>()}, " +
                                                 $"but the key provided was of type {key.GetType()}. ");  
                }
            }
        }


        void IDictionary.Add(object key, object value)
        {
            if (key is TKey keyOfCorrectType)
            {
                if (value is TValue valueOfCorrectType)
                {
                    Add(keyOfCorrectType, valueOfCorrectType);
                }
                else
                {
                    throw new ArgumentException("A process attempted to add a value of type " +
                                                $"{value?.GetType()} to a {NameOf<DictionaryImplementer<TKey, TValue>>()}");
                }
            }
            else
            {
                throw new ArgumentException("A process attempted to add a key of type " +
                                            $"{key?.GetType()} to a {NameOf<DictionaryImplementer<TKey, TValue>>()}");
            }
            
        }
        
        void IDictionary.Remove(object key)
        {
            if (key is TKey keyOfCorrectType)
            {
                Remove(keyOfCorrectType);
            }
        }
        
        bool IDictionary.Contains(object key)
        {
            if (key is TKey keyOfCorrectType)
            {
                return ContainsKey(keyOfCorrectType);
            }
            else return false;
        }

        #endregion
    }
}