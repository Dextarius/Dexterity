using System;
using Core.Factors;
using Factors.Observer;

namespace Factors.Cores.ProactiveCores
{
    public class ObservedFactorCore : FactorCore, IInvolved
    {
        #region Static Properties

        protected static CausalObserver Observer => CausalObserver.ForThread;

        #endregion

        
        #region Instance Properties

        protected          IFactor Owner          { get; set; }
        public    override int     UpdatePriority => 0;

        #endregion


        #region Instance Methods

        public void NotifyInvolved() => Observer.NotifyInvolved(Owner);
        public void NotifyChanged()  => Observer.NotifyChanged(Owner);
        
        public void SetOwner(IReactorCoreOwner reactor)
        {
            if (Owner != null)
            {
                throw new InvalidOperationException(
                    $"A process attempted to set the Owner for a ReactorCore to {reactor}, " +
                    $"but its Owner property is already assigned to {Owner}. ");
            }
            
            Owner = reactor ?? throw new ArgumentNullException(nameof(reactor));
        }
        
        public override void Dispose()
        {
            Owner = null;
        }
        
        #endregion
        
        
        public ObservedFactorCore() : base()
        {
            
        }
    }
}