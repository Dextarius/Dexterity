using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Core.Causality;
using Core.Factors;
using Core.States;
using Factors.Cores.ProactiveCores;
using static Core.Tools.Types;

namespace Factors.Collections
{
    public partial class ProactiveDictionary<TKey, TValue> : 
        ProactiveCollection< IDictionaryState<TKey, TValue>, KeyValuePair<TKey, TValue>>, 
        IDictionary<TKey, TValue>, IDictionary
    {
        #region Properties
        
        public ICollection<TKey>   Keys   => core.Keys;
        public ICollection<TValue> Values => core.Values;
        
        public TValue this[TKey key]
        {
            get => core[key];
            set => core[key] = value;
        }

        #endregion

        
        #region Instance Methods

        public     bool                  TryGetValue(TKey key, out TValue value) => core.TryGetValue(key, out value);
        public     void                  Add(TKey key, TValue value)             => core.Add(key, value);
        public     bool                  Remove(TKey key)                        => core.Remove(key);
        public     bool                  ContainsKey(TKey key)                   => core.ContainsKey(key);
        public new IDictionaryEnumerator GetEnumerator()                         =>  core.GetEnumerator();

        #endregion

        
        #region Constructors
        
        public ProactiveDictionary(IDictionaryState<TKey, TValue> dictionaryCore, string name = null) : 
            base(dictionaryCore, name)
        {
            
        }
        
        public ProactiveDictionary(string name) : 
            this(new ObservedDictionaryState<TKey, TValue>(name), name)
        {
        }

        #endregion

        
        #region Explicit Implementations

        ICollection IDictionary.Keys        => core.GetKeysAsICollection();
        ICollection IDictionary.Values      => core.GetValuesAsICollection();
        bool        IDictionary.IsFixedSize => false;
        bool        IDictionary.IsReadOnly  => false;
        
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
                                                $"{value?.GetType()} to a {NameOf<ProactiveDictionary<TKey, TValue>>()}");
                }
            }
            else
            {
                throw new ArgumentException("A process attempted to add a key of type " +
                                            $"{key?.GetType()} to a {NameOf<ProactiveDictionary<TKey, TValue>>()}");
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