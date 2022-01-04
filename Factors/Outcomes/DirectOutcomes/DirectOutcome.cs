using System.Collections.Generic;
using Core.Factors;
using Core.States;
using Factors.Observer;

namespace Factors.Outcomes.DirectOutcomes
{
    public abstract class DirectOutcome : OutcomeBase
    {
        #region Static Properties

        protected static CausalObserver Observer => CausalObserver.ForThread;

        #endregion
        
        
        #region Instance Properties

        public override bool                 IsBeingInfluenced  => true;
        public abstract IEnumerable<IFactor> Inputs             { get; }

        #endregion
        

        #region Instance Methods
        
        protected override void OnNecessary()
        {
            foreach (var determinant in Inputs)
            {
                determinant.NotifyNecessary();
            }
        }

        protected override void OnNotNecessary()
        {
            foreach (var determinant in Inputs)
            {
                determinant.NotifyNotNecessary();
            }
        }

        protected override bool TryStabilizeOutcome()
        {
            foreach (var determinant in Inputs)
            {
                if (determinant.Reconcile() is false)
                {
                    return false;
                }
            }

            return true;
        }

        protected void AddSelfAsDependentToInputs()
        {
            foreach (var input in Inputs)
            {
                input.AddDependent(this);
            }
        }
        
        protected override void InvalidateOutcome(IFactor changedParentState) { }
        
        public void NotifyInvolved() => Observer.NotifyInvolved(this);
        public void NotifyChanged()  => Observer.NotifyChanged(this);
        
        #endregion
        

        #region Constructors

        public DirectOutcome(string name) : base(name)
        {
        }

        #endregion
    }
}