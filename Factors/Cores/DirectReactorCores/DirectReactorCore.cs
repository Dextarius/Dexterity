using System.Collections.Generic;
using Core.Factors;
using Core.States;
using Factors.Observer;

namespace Factors.Cores.DirectReactorCores
{
    public abstract class DirectReactorCore : ReactorCore
    {
        #region Instance Properties

        public override bool HasTriggers => true;

        #endregion
        

        #region Instance Methods

        protected void SubscribeToInputs()
        {
            if (HasReacted is false)
            {
                foreach (var trigger in Triggers)
                {
                    AddTrigger(trigger, IsReflexive);
                }
            }
        }
        
        protected override void InvalidateOutcome(IFactor changedFactor) { }
        
        // public void NotifyInvolved(IFactor involvedFactor) => Observer.NotifyInvolved(involvedFactor);
        // public void NotifyChanged(IFactor involvedFactor)  => Observer.NotifyChanged(involvedFactor);
        
        #endregion


        #region Constructors

        protected DirectReactorCore(bool useWeakSubscriber = true) : base(useWeakSubscriber)
        {

        }

        #endregion
    }
}