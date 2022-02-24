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

        public override int Priority => 0;

        #endregion


        #region Instance Methods

        public void NotifyInvolved() => Observer.NotifyInvolved(this);
        public void NotifyChanged()  => Observer.NotifyChanged(this);
        
        #endregion
        
        
        public ObservedFactorCore(string name) : base(name)
        {
            
        }
    }
}