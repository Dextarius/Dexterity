using System;
using System.Collections;
using System.Collections.Generic;
using Core.Causality;
using Core.Factors;
using Core.States;
using Core.Tools;
using Factors.Exceptions;
using JetBrains.Annotations;
using static Dextarius.Utilities.Types;

namespace Factors.Collections
{
    public class ReactiveDictionary<TKey, TValue> : 
        ReactiveCollection< IDictionaryResult<TKey, TValue>, KeyValuePair<TKey, TValue>>, 
        IDictionary, IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
    {
        #region Static Fields

        private static readonly string DefaultName = NameOf<ReactiveDictionary<TKey, TValue>>();

        #endregion


        #region Instance Properties

        public TValue              this[TKey key] => Collection[key];
        public ICollection<TKey>   Keys           => Collection.Keys;
        public ICollection<TValue> Values         => Collection.Values;
        //^ The keys/value collections always represents the CURRENT dictionary keys/values (that's important to remember).
        //  This is not a static copy of what keys were present when it was called!

        #endregion
        

        #region Instance Methods

        public     Dictionary<TKey, TValue> AsNormalDictionary()                    => Collection.AsNormalDictionary();
        public     bool                     TryGetValue(TKey key, out TValue value) => Collection.TryGetValue(key, out value);
        public     bool                     ContainsKey(TKey key)                   => Collection.ContainsKey(key);
        public     bool                     ContainsValue(TValue key)               => Collection.ContainsValue(key);
        public new IDictionaryEnumerator    GetEnumerator()                         => Collection.GetEnumerator();

        #endregion
        

        #region Constructors

        public ReactiveDictionary(IDictionaryResult<TKey, TValue> dictionaryCore, string name = null) : 
            base(dictionaryCore, name ?? NameOf<ReactiveDictionary<TKey, TValue>>())
        {
        }
        
        #endregion
        

        #region Explicit Implementations

        IEnumerable<TKey>   IReadOnlyDictionary<TKey, TValue>.Keys   => Keys;
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;
        ICollection         IDictionary.Keys                         => Collection.GetKeysAsICollection();
        ICollection         IDictionary.Values                       => Collection.GetValuesAsICollection();
        bool                IDictionary.IsReadOnly                   => true;
        bool                IDictionary.IsFixedSize                  => true;
        
        TValue IDictionary<TKey, TValue>.this[TKey key]
        {
            get => this[key];
            set => throw new CannotModifyReactiveValueException();
        }
        
        object IDictionary.this[object key]
        {
            get
            {
                if (key is TKey keyOfCorrectType)
                {
                    return Collection[keyOfCorrectType];
                }
                else return null;
            }
            set => throw new CannotModifyReactiveValueException();
        }

        
        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)   => throw new CannotModifyReactiveValueException();
        bool IDictionary<TKey, TValue>.Remove(TKey key)              => throw new CannotModifyReactiveValueException();
        void IDictionary.Add(object key, object value)               => throw new CannotModifyReactiveValueException();
        void IDictionary.Remove(object key)                          => throw new CannotModifyReactiveValueException();
        void IDictionary.Clear()                                     => throw new CannotModifyReactiveValueException();
        
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


    
    // public static class ReactiveDictionary
    // {
    //     
    //     public static ReactiveDictionary<TKey, TValue> CreateFrom<TKey, TValue>(
    //         [NotNull] Func< IEnumerable< KeyValuePair<TKey, TValue>>> functionToCreateKeyValuePairs,
    //         IEqualityComparer<TKey>   comparerForKeys   = null, 
    //         IEqualityComparer<TValue> comparerForValues = null, 
    //         string                    name = null)
    //     {
    //         if (functionToCreateKeyValuePairs is null) { throw new ArgumentNullException(nameof(functionToCreateKeyValuePairs)); }
    //
    //         name ?? CreateDefaultName<ReactiveDictionary<TKey, TValue>>(functionToCreateKeyValuePairs);
    //         result = DictionaryFunctionResult.CreateFrom(
    //             functionToCreateKeyValuePairs, this, comparerForKeys, comparerForValues);
    //     }
    //     public ReactiveDictionary([NotNull] Func< IEnumerable< KeyValuePair<TKey, TValue>>> functionToCreateKeyValuePairs, 
    //                               string name = null) : 
    //         this(functionToCreateKeyValuePairs, null, null, name)
    //     {
    //     }
    //
    //     public ReactiveDictionary([NotNull] Func< IEnumerable< KeyValuePair<TKey, TValue>>> functionToCreateKeyValuePairs, 
    //                               IEqualityComparer<TValue> comparerForValues = null,
    //                               string name = null) : 
    //         this(functionToCreateKeyValuePairs, null, comparerForValues, name)
    //     {
    //     }
    //
    //
    //     public ReactiveDictionary([NotNull] IDictionaryResult<TKey, TValue> collectionSource, string name = null) : 
    //         base(name ?? DefaultName)
    //     {
    //         result = collectionSource;
    //     }
    // }
}