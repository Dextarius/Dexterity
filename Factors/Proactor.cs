﻿using Core.Causality;
using Core.Factors;
using Core.States;
using Factors.Cores.ProactiveCores;
using JetBrains.Annotations;

namespace Factors
{
    public abstract class Proactor<TCore> : Factor<TCore>, IFactorCoreCallback 
        where TCore : IProactorCore
    {
        #region Constructors

        protected Proactor(TCore factorCore, string factorsName = "Factor") : base(factorCore, factorsName)
        {
            factorCore.SetCallback(this);
        }

        #endregion


        #region Explicit Implementations

        void IFactorCoreCallback.CoreUpdated(IFactorCore triggeredCore, long triggerFlags)
        {
            if (EnsureIsCorrectCore(triggeredCore))
            {
                OnUpdated(triggerFlags);
            }
        }

        #endregion
    }
    
    
    
    public class Proactor : Proactor<ProactorCore>
    {
        #region Instance Methods

        public override bool CoresAreNotEqual(ProactorCore oldCore, ProactorCore newCore) => false;

        #endregion
        
        
        #region Constructors

        public Proactor(ProactorCore factorCore, string factorsName = nameof(Proactor)) : base(factorCore, factorsName)
        {
            factorCore.SetCallback(this);
        }
        
        public Proactor(string factorsName = nameof(Proactor)) : base(new ProactorCore(), factorsName)
        {
        }

        #endregion
    }
}