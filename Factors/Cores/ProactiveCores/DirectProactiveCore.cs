using System.Collections.Generic;
using Core.States;
using JetBrains.Annotations;

namespace Factors.Cores.ProactiveCores
{
    public class DirectProactiveCore<T> : ProactiveCore<T>
    {
        #region Instance Properties


        
        #endregion


        #region Constructors

        public DirectProactiveCore(T initialValue, IEqualityComparer<T> comparer = null) : 
            base(initialValue, comparer)
        {
        }
        
        #endregion
    }
}