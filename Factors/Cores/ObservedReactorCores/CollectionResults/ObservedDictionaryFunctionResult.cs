using System;
using System.Collections.Generic;
using Core.Tools;
using JetBrains.Annotations;

namespace Factors.Cores.ObservedReactorCores.CollectionResults
{
    public class ObservedDictionaryFunctionResult<TKey, TValue> : ObservedDictionaryResult<TKey, TValue>
    {
        #region Instance Fields

        [NotNull]
        private readonly Func<IEnumerable<KeyValuePair<TKey, TValue>>> elementGenerator;

        #endregion
        

        #region Instance Methods

        protected override IEnumerable<KeyValuePair<TKey, TValue>> GetElements() => elementGenerator();
        
        public override string ToString() => Delegates.GetClassAndMethodName(elementGenerator);
        
        #endregion
        
        
        #region Constructors

        public ObservedDictionaryFunctionResult(Func<IEnumerable<KeyValuePair<TKey, TValue>>> functionForElements,
                                                IEqualityComparer<TKey>                       comparerForKeys   = null,
                                                IEqualityComparer<TValue>                     comparerForValues = null) : 
            base(comparerForKeys, comparerForValues)
        {
            elementGenerator = functionForElements ?? throw new ArgumentNullException(nameof(functionForElements));
        }

        public ObservedDictionaryFunctionResult(Func<IEnumerable<KeyValuePair<TKey, TValue>>> functionForElements,
                                                IEqualityComparer<TValue>                     comparerForValues) : 
            this(functionForElements, null, comparerForValues)
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