using System;
using System.Collections.Generic;
using Core.Tools;
using JetBrains.Annotations;

namespace Factors.Outcomes.ObservedOutcomes.CollectionResults
{
    public class ObservedDictionaryFunctionResult<TKey, TValue> : ObservedDictionaryResult<TKey, TValue>
    {
        #region Instance Fields

        [NotNull]
        private readonly Func<IEnumerable<KeyValuePair<TKey, TValue>>> elementGenerator;

        #endregion



        #region Instance Methods

        protected override IEnumerable<KeyValuePair<TKey, TValue>> GetElements() => elementGenerator();

        #endregion


        
        #region Constructors

        public ObservedDictionaryFunctionResult(Func<IEnumerable<KeyValuePair<TKey, TValue>>> functionForElements,
                                                string                    name,
                                                IEqualityComparer<TKey>   comparerForKeys   = null,
                                                IEqualityComparer<TValue> comparerForValues = null) : 
            base(name ?? Delegates.GetClassAndMethodName(functionForElements), comparerForKeys, comparerForValues)
        {
            elementGenerator = functionForElements ?? throw new ArgumentNullException(nameof(functionForElements));
        }
        
        public ObservedDictionaryFunctionResult(Func<IEnumerable<KeyValuePair<TKey, TValue>>> functionForElements,
                                                IEqualityComparer<TKey>   comparerForKeys   = null,
                                                IEqualityComparer<TValue> comparerForValues = null) : 
            this(functionForElements, null, comparerForKeys, comparerForValues)
        {
        }
        
        public ObservedDictionaryFunctionResult(Func<IEnumerable<KeyValuePair<TKey, TValue>>> functionForElements,
                                                IEqualityComparer<TValue> comparerForValues) : 
            this(functionForElements, null, null, comparerForValues)
        {
        }

        #endregion
    }
    
    
    public static class DictionaryFunctionResult 
    {
        public static ObservedDictionaryFunctionResult<TKey, TValue> CreateFrom<TKey, TValue>(
            Func<IEnumerable<KeyValuePair<TKey, TValue>>> functionToGenerateElements,
            IEqualityComparer<TKey>   comparerForKeys   = null,
            IEqualityComparer<TValue> comparerForValues = null)
        {
            {
                if (functionToGenerateElements is null) { throw new ArgumentNullException(nameof(functionToGenerateElements)); }
            
                return new ObservedDictionaryFunctionResult<TKey, TValue>(
                    functionToGenerateElements, comparerForKeys, comparerForValues);
            }
        }

        public static ObservedDictionaryFunctionResult<TKey, TValue> CreateFrom<TKey, TValue>(
                Func<IEnumerable<KeyValuePair<TKey, TValue>>> functionToGenerateElements,
                IEqualityComparer<TValue> comparerForValues) => 
            CreateFrom(functionToGenerateElements, null, comparerForValues);
    }
}