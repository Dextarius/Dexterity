using System;
using Core.States;

namespace Causality
{
    public class CausalObserver : Observer<IInfluence, IInfluenceable>
    {
        #region Static Fields

        [ThreadStatic] private static CausalObserver observerForThread;

        #endregion
        
        
        #region Static Properties

        public static CausalObserver ForThread => observerForThread??  (observerForThread = new CausalObserver());


        #endregion
    }
}