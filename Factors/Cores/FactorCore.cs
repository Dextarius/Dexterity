using System;
using System.Collections.Generic;
using Core;
using Core.Factors;
using Core.States;
using JetBrains.Annotations;

namespace Factors.Cores
{
    public abstract class FactorCore : IFactorCore
    {
        #region Instance Properties

        public virtual int  UpdatePriority => 0;
        public         uint VersionNumber  { get; protected set; }

        #endregion

        
        #region Instance Methods
        
        public virtual void Dispose() { }

        #endregion
    }
}