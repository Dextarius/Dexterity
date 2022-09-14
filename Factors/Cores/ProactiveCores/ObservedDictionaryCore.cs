using System.Collections;
using System.Collections.Generic;
using Core.Redirection;
using Core.States;
using Dextarius.Collections;
using Factors.Collections;
using static Core.Tools.Types;
using static Core.Tools.Collections;

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
                long triggerFlags = TriggerFlags.None;
                
                if (collection.TryGetValue(key, out TValue currentValue) is false)
                {
                    if (valueComparer.Equals(value, currentValue) is false)
                    {
                        collection[key] = value;
                        triggerFlags    = TriggerFlags.ItemReplaced;
                    }
                }
                else
                {
                    collection[key] = value;
                    triggerFlags = TriggerFlags.ItemAdded;
                }

                if (triggerFlags is not TriggerFlags.None)
                {
                    OnCollectionChanged(triggerFlags);
                }
            }
        }

        #endregion
        

        #region Instance Methods

        public void Add(TKey key, TValue value)
        {
            collection.Add(key, value);
            OnCollectionChanged(TriggerFlags.ItemAdded);
        }

        public bool Remove(TKey key)
        {
            if (collection.Remove(key))
            {
                OnCollectionChanged(TriggerFlags.ItemRemoved);
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
            //  Well if they call it again before this new key is removed the
            //  dictionary will explode anyways, so I guess the only reasonable
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

            if (wasSuccessful)
            {
                NotifyInvolved(TriggerFlags.ItemRemoved | TriggerFlags.ItemReplaced);
            }
            else
            {
                NotifyInvolved(TriggerFlags.ItemAdded | TriggerFlags.ItemReplaced);
            }
            
            return wasSuccessful;
        }
        
        public bool ContainsKey(TKey keyToLookFor)
        {
            bool wasSuccessful = Collection.ContainsKey(keyToLookFor);

            if (wasSuccessful)
            {
                NotifyInvolved(TriggerFlags.ItemRemoved);
            }
            else
            {
                NotifyInvolved(TriggerFlags.ItemAdded | TriggerFlags.ItemReplaced);
            }
            
            return wasSuccessful;
        }
        
        public bool ContainsValue(TValue valueToLookFor)
        {
            bool wasSuccessful = Collection.ContainsValue(valueToLookFor);

            if (wasSuccessful)
            {
                NotifyInvolved(TriggerFlags.ItemRemoved | TriggerFlags.ItemReplaced);
            }
            else
            {
                NotifyInvolved(TriggerFlags.ItemAdded | TriggerFlags.ItemReplaced);
            }
            
            return wasSuccessful;
        }

        public ICollection GetKeysAsICollection()   => keys   ??= new StateKeyConservator(this);
        public ICollection GetValuesAsICollection() => values ??= new StateValueConservator(this);

        public new IDictionaryEnumerator GetEnumerator() => new FactorDictionaryEnumerator(this, Collection.GetEnumerator());
        //- TODO : Decide if we really want to make an enumerator object every time someone calls this method.  If not then 
        //         we can just cast Collection to IDictionary and get the enumerator from it.

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
            this(CreateNewDictionary(collectionToCopy, comparerForKeys ?? EqualityComparer<TKey>.Default), 
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