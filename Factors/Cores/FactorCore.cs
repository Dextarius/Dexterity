using System;
using System.Collections.Generic;
using Core;
using Core.Factors;
using Core.States;
using Factors.Observer;
using JetBrains.Annotations;

namespace Factors.Cores
{
    public abstract class FactorCore : IFactorCore
    {
        #region Static Properties

        protected static CausalObserver Observer => CausalObserver.ForThread;

        #endregion
        
        #region Instance Properties

        public virtual int UpdatePriority => 0;

        #endregion

        
        #region Instance Methods
        
        public virtual bool Reconcile()
        {
            return true;
            //^ Reconcile is used when a subscriber is destabilized, and since only reactors destabilize their dependents,
            //  a basic factor should never be the parent that the caller needs to reconcile with.
        }
        
        public virtual void Dispose() { }

        #endregion
    }
}