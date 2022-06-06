using System.Collections.Generic;
using Core.Factors;
using Factors.Observer;

namespace Factors.Cores.DirectReactorCores
{
    public abstract class DirectReactorCore : ReactorCore
    {
        #region Static Properties

        protected static CausalObserver Observer => CausalObserver.ForThread;

        #endregion
        
        
        #region Instance Properties

        public override bool HasTriggers => true;

        #endregion
        

        #region Instance Methods

        protected void SubscribeToInputs()
        {
            if (VersionNumber == 0)
            {
                foreach (var input in Triggers)
                {
                    AddTrigger(input, IsNecessary);
                }
            }
        }
        
        protected override void InvalidateOutcome(IFactor changedParentState) { }
        
        public void NotifyInvolved() => Observer.NotifyInvolved(Owner);
        public void NotifyChanged()  => Observer.NotifyChanged(Owner);
        
        #endregion
    }
}