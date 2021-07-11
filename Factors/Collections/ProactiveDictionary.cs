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

        protected IEqualityComparer<TValue> ValueComparer => valueComparer ?? EqualityComparer<TValue>.Default;

        public TValue this[TKey key]
        {
            get => Collection[key];
            set
            {
                Dictionary<TKey, TValue> collection = Collection;
                IState<Dictionary<TKey, TValue>> previousState;
                
                lock (syncLock)
                {
                    if (collection.TryGetValue(key, out TValue currentValue) && valueComparer.Equals(value, currentValue))
                    {
                        return;
                    }
                    else
                    {
                        IState<Dictionary<TKey, TValue>> newState = new State<Dictionary<TKey, TValue>>(collection);

                        collection[key] = value;
                        previousState   = state;
                        state           = newState;
                    }
                }

                previousState.Invalidate();
                Observer.NotifyChanged(previousState);
            }
        }
        
        public ICollection<TKey>   Keys        => keys   ?? (keys   = new ProactiveKeyConservator(this));
        public ICollection<TValue> Values      => values ?? (values = new ProactiveValueConservator(this));
        public bool                IsFixedSize => false;
        public bool                IsReadOnly  => false;

        #endregion

        #region Instance Methods

        //- TODO: We could save a lot of hassle by just making a CollectionState class that doesn't need to be recreated
        //        every time something changes.
        
        //- TODO : Some of these methods create new state instances inside the lock, some outside it, we should make them all consistent.
        //- TODO : Only some of these are thread-safe, we should make them all consistent.
        //- TODO : A lot of these methods grab Collection before getting the lock.  If we ever actually changed the
        //         collection it might create bugs if it grabs the value right before another thread changes it.
        
        public void Add(TKey key, TValue value)
        {
            Dictionary<TKey, TValue>         collection = Collection;
            IState<Dictionary<TKey, TValue>> newState   = new State<Dictionary<TKey, TValue>>(collection);
            IState<Dictionary<TKey, TValue>> previousState;

            lock (syncLock)
            {
                previousState = state;
                state = newState;
                collection.Add(key, value);
            }
                
            previousState.Invalidate();
            Observer.NotifyChanged(previousState);
        }

        public bool ContainsKey(TKey key) => Collection.ContainsKey(key);

        public bool Remove(TKey key)
        {
            Dictionary<TKey, TValue>         collection    = Collection;
            IState<Dictionary<TKey, TValue>> previousState = null;
            bool wasSuccessful;

            lock (syncLock)
            {
                wasSuccessful = collection.Remove(key);
                
                if (wasSuccessful)
                {
                    previousState = state;
                    state = new State<Dictionary<TKey, TValue>>(collection);
                }
            }

            if (wasSuccessful)
            {
                previousState.Invalidate();
                Observer.NotifyChanged(previousState);
            }
            
            return wasSuccessful;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            lock (syncLock)
            {
                return Collection.TryGetValue(key, out value);
            }
        }

        public void Add(object key, object value)
        {
            if (key is TKey keyOfCorrectType)
            {
                if (value is TValue valueOfCorrectType)
                {
                    Dictionary<TKey, TValue>         collection = Collection;
                    State<Dictionary<TKey, TValue>>  newState   = new State<Dictionary<TKey, TValue>>(collection);
                    IState<Dictionary<TKey, TValue>> previousState;

                    lock (syncLock)
                    {
                        previousState = state;
                        collection.Add(keyOfCorrectType, valueOfCorrectType);
                        state = newState;
                    }

                    previousState.Invalidate();
                    Observer.NotifyChanged(previousState);
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
            lock (syncLock)
            {
                if (key is TKey keyOfCorrectType)
                {
                    return Collection.ContainsKey(keyOfCorrectType);
                }
                else
                {
                    return false;
                }
            }
        }

        public void Remove(object key)
        {
            if (key is TKey keyOfCorrectType)
            {
                IState<Dictionary<TKey, TValue>> previousState = null;
                bool keyWasRemoved = false;

                lock (keyOfCorrectType)
                {
                    var collection = Collection;

                    if (collection.Remove(keyOfCorrectType))
                    {
                        var newState = new State<Dictionary<TKey, TValue>>(collection);

                        previousState = state;
                        keyWasRemoved = true;
                        state         = newState;
                    }
                }

                if (keyWasRemoved)
                {
                    previousState.Invalidate();
                    Observer.NotifyChanged(previousState);
                }
            }
        }

        //- TODO : We made a FactorDictionaryEnumerator class for things like this, but we still have to decide if we 
        //         want to notify for every element the enumerator has.
        public new IDictionaryEnumerator GetEnumerator() => ((IDictionary)Collection).GetEnumerator();

        #endregion

        
        #region Constructors

        //- TODO : Add a constructor that lets users specify a comparer for keys and/or values.
        
        public ProactiveDictionary(string name = null) : 
            this(new Dictionary<TKey, TValue>(), EqualityComparer<KeyValuePair<TKey, TValue>>.Default, name)
        {
        }
        
        public ProactiveDictionary(IEqualityComparer<KeyValuePair<TKey, TValue>> comparer = null, string name = null) : 
            this(new Dictionary<TKey, TValue>(), comparer, name)
        {
        }
        
        public ProactiveDictionary(IDictionary<TKey, TValue> collectionToUse, IEqualityComparer<KeyValuePair<TKey, TValue>> comparer = null, 
            string name = null) : 
            this(new Dictionary<TKey, TValue>(collectionToUse), comparer, name)
        {
        }

        //- TODO : Should we be allowing the users to provide a collection that we use directly,
        //         since that means they can change the collection without its dependents getting notified? What's the use case for it?
        public ProactiveDictionary(Dictionary<TKey, TValue> dictionaryToUse, 
                                   IEqualityComparer<KeyValuePair<TKey, TValue>> comparer = null, 
                                   string name = null) : 
            base(dictionaryToUse, comparer, name ?? NameOf<ProactiveDictionary<TKey, TValue>>())
        {
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
                            $"A process attempted to set the value for key {key.ToString()}" +
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