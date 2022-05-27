using System;
using System.Collections.Generic;
using Core.Tools;
using JetBrains.Annotations;

namespace Factors.Cores.ObservedReactorCores.CollectionResults
{
    public class ObservedListFunctionResult<T> : ObservedListResult<T>
    {
        #region Instance Fields

        [NotNull]
        private readonly Func<IEnumerable<T>> elementGenerator;

        #endregion
        
        
        #region Instance Methods

        protected override IEnumerable<T> GetElements() => elementGenerator();

        public override string ToString() => Delegates.GetClassAndMethodName(elementGenerator);

        #endregion
        
        
        #region Constructors

        public ObservedListFunctionResult(Func<IEnumerable<T>> functionForElements,
                                          IEqualityComparer<T> comparerForElements = null) : 
            base(comparerForElements)
        {
            elementGenerator = functionForElements ?? throw new ArgumentNullException(nameof(functionForElements));
        }

        #endregion
    }
    
}