using Core.Causality;
using Core.Factors;
using Core.States;
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

        void IFactorCoreCallback.CoreUpdated(IFactorCore triggeredCore)
        {
            if (EnsureIsCorrectCore(triggeredCore))
            {
                OnUpdated();
            }
        }

        #endregion
    }
}