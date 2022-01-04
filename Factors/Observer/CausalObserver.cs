using System;
using Core.Factors;
using Core.States;

namespace Factors.Observer
{
    public class CausalObserver : Observer<IFactor, IObserved>
    {
        #region Static Fields

        [ThreadStatic] private static CausalObserver observerForThread;

        #endregion
        
        
        #region Static Properties

        public static CausalObserver ForThread => observerForThread ??= new CausalObserver();


        #endregion
    }
}