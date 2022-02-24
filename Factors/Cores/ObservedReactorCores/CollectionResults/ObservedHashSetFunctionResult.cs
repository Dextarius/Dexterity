using System;
using System.Collections.Generic;
using Core.Tools;
using JetBrains.Annotations;

namespace Factors.Cores.ObservedReactorCores.CollectionResults
{
    public class ObservedHashSetFunctionResult<T> : ObservedHashSetResult<T>
    {
        #region Instance Fields

        [NotNull]
        private readonly Func<IEnumerable<T>> elementGenerator;

        #endregion
        
        
        #region Instance Methods

        protected override IEnumerable<T> GetElements() => elementGenerator();

        #endregion
        
        
        #region Constructors

        public ObservedHashSetFunctionResult(Func<IEnumerable<T>> functionForElements,
                                             IEqualityComparer<T> comparerForElements = null, 
                                             string               name                = null) : 
            base(name ?? Delegates.GetClassAndMethodName(functionForElements), comparerForElements)
        {
            elementGenerator = functionForElements ?? throw new ArgumentNullException(nameof(functionForElements));
        }
        
        public ObservedHashSetFunctionResult(Func<IEnumerable<T>> functionForElements, string name) : 
            this(functionForElements, null, name)
        {
        }

        #endregion
    }
}