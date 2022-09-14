using System;
using System.Collections.Generic;
using Core.Tools;
using JetBrains.Annotations;

namespace Factors.Cores.DirectReactorCores.CollectionResults
{
    public abstract class DirectListFunctionResult<T> : DirectListResult<T>
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

        protected DirectListFunctionResult(Func<IEnumerable<T>> functionForElements,
                                           IEqualityComparer<T> comparerForElements = null) : 
            base(comparerForElements)
        {
            elementGenerator = functionForElements ?? throw new ArgumentNullException(nameof(functionForElements));
        }

        #endregion
    }
}