using System;
using System.Collections;
using System.Collections.Generic;
using Causality;
using Causality.States;
using Core.Causality;
using Core.Factors;
using static Core.Tools.Types;

namespace Factors.Collections
{
    public partial class ProactiveDictionary<TKey, TValue> : ProactiveCollection<Dictionary<TKey, TValue>, KeyValuePair<TKey, 
    TValue>>, 
        IDictionary<TKey, TValue>, IDictionary
    {
        #region Instance Fields

        private IEqualityComparer<TKey>   keyComparer;
        private IEqualityComparer<TValue> valueComparer;
        private ICollection<TKey>         keys;
        private ICollection<TValue>       values;

        #endregion
        
        
        #region Properties
        
        public TValue this[TKey key]
        {
            get => Collection[key];
            set
            {
                Dictionary<TKey, TValue> collection = state.Peek();

                if (collection.TryGetValue(key, out TValue currentValue) is false || 
                    valueComparer.Equals(value, currentValue) is false)
                {
                    collection[key] = value;
                    state.OnChanged();
                }
            }
        }
        
        public ICollection<TKey>   Keys        => keys??= new ProactiveKeyConservator(this);
        public ICollection<TValue> Values      => values??= new ProactiveValueConservator(this);
        public bool                IsFixedSize => false;
        public bool                IsReadOnly  => false;

        #endregion

        #region Instance Methods

        //- TODO: We could save a lot of hassle by just making a CollectionState class that doesn't need to be recreated
        //        every time something changes.  It would also let us avoid having to use Peek() all of the time.

        public void Add(TKey key, TValue value)
        {
            Dictionary<TKey, TValue> collection = state.Peek();

            collection.Add(key, value);
            state.OnChanged();
        }

        public bool ContainsKey(TKey key) => Collection.ContainsKey(key);

        public bool Remove(TKey key)
        {
            var  collection    = state.Peek();
            bool wasSuccessful = collection.Remove(key);

            if (wasSuccessful)
            {
                state.OnChanged();
            }

            return wasSuccessful;
        }

        public bool TryGetValue(TKey key, out TValue value) => Collection.TryGetValue(key, out value);

        public void Add(object key, object value)
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
                                                $"{value?.GetType()} to a {NameOf<ProactiveDictionary<TKey, TValue>>()}");
                }
            }
            else
            {
                throw new ArgumentException("A process attempted to add a key of type " +
                                            $"{key?.GetType()} to a {NameOf<ProactiveDictionary<TKey, TValue>>()}");
            }
            
        }

        public bool Contains(object key)
        {
            if (key is TKey keyOfCorrectType)
            {
                return ContainsKey(keyOfCorrectType);
            }
            else
            {
                return false;
            }
        }

        public void Remove(object key)
        {
            if (key is TKey keyOfCorrectType)
            {
                Remove(keyOfCorrectType);
            }
        }

        //- TODO : We made a FactorDictionaryEnumerator class for things like this, but we still have to decide if we 
        //         want to notify for every element the enumerator has.
        public new IDictionaryEnumerator GetEnumerator() => ((IDictionary)Collection).GetEnumerator();

        #endregion

        
        #region Constructors

        //- TODO : Add a constructor that lets users specify a comparer for keys and/or values.
        
        public ProactiveDictionary(string name = null) : this(new Dictionary<TKey, TValue>(), null, name)
        {
        }
        
        public ProactiveDictionary(
            IEqualityComparer<KeyValuePair<TKey, TValue>> keyPairComparer = null, string name = null) : 
                this(new Dictionary<TKey, TValue>(), keyPairComparer, name)
        {
        }
        
        public ProactiveDictionary(IDictionary<TKey, TValue>                     dictionaryToCopy, 
                                   IEqualityComparer<KeyValuePair<TKey, TValue>> keyPairComparer = null, 
                                   string                                        name            = null) : 
            this(new Dictionary<TKey, TValue>(dictionaryToCopy), keyPairComparer, name)
        {
        }

        //- TODO : Should we be allowing the users to provide a collection that we use directly,
        //         since that means they can change the collection without its dependents getting notified?
        //         What's the use case for it?
        public ProactiveDictionary(Dictionary<TKey, TValue>                      dictionaryToUse, 
                                   IEqualityComparer<KeyValuePair<TKey, TValue>> keyPairComparer = null, 
                                   string                                        name            = null) : 
            base(dictionaryToUse, 
                keyPairComparer ?? EqualityComparer<KeyValuePair<TKey, TValue>>.Default, 
                name            ?? NameOf<ProactiveDictionary<TKey, TValue>>())
        {
            keyComparer   = dictionaryToUse.Comparer;
            valueComparer = EqualityComparer<TValue>.Default;
            
            //- What if someone passes a different key comparer than the Dictionary is using?
        }

        #endregion

        
        #region Explicit Implementations

        ICollection IDictionary.Values => Collection.Values;
        ICollection IDictionary.Keys   => Collection.Keys;
        
        object IDictionary.this[object key]
        {
            get => (key is TKey keyOfCorrectType)? this[keyOfCorrectType] : throw new ArgumentException("");
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
                        throw new ArgumentException(
                            $"A process attempted to set the value for key {key}" +
                            $"in a {NameOf<ProactiveDictionary<TKey, TValue>>()}, " +
                            $"but the value provided was of type {value.GetType()}. ");  
                    }
                }
                else
                {
                    throw new ArgumentException(
                        $"A process attempted to set a key in a {NameOf<ProactiveDictionary<TKey, TValue>>()}, " +
                        $"but the key provided was of type {key.GetType()}. ");  
                }
            }
        }

        #endregion
    }
}