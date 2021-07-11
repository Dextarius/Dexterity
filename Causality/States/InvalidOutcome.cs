using Core.Causality;
using Core.Factors;

namespace Causality.States
{
    public class InvalidOutcome : IOutcome
    {
        #region Static Properties

        public static readonly InvalidOutcome Default = new InvalidOutcome();

        #endregion

        #region Properties

        public bool IsBeingAffected { get; }
        public bool IsConsequential => false;
        public bool IsInvalid       => true;
        public bool IsValid         => false;
        public bool HasCallback     => false;

        #endregion


        #region Instance Methods

        public void NotifyInvolved() => Observer.NotifyInvolved(this);

        public bool Invalidate() => false;
        public bool Invalidate(IState invalidParentState) => false;
        public void InvalidateDependents() {  }
        public void ReleaseDependent(IOutcome dependentOutcome) { }

        public void SetInfluences(IState[] newInfluences)
        {
            //- I don't intend for this method to be used, but who knows if I'll remember.
            //  Might as well make it behave appropriately.
            foreach (var influence in newInfluences)
            {
                influence.ReleaseDependent(this);
            }
        }

        public bool AddDependent(IOutcome dependentOutcome)
        {
            //- TODO : This code is replicated in section where CasualEvent tries to add dependencies.
            if (dependentOutcome.IsValid)
            {
                dependentOutcome.Invalidate(this);
            }

            return false;
        }
        
        public void SetCallback(INotifiable objectToNotify)
        {
            //- TODO : Consider if immediately calling the object being notified is what most people would expect to
            //         happen when they set a callback to an already invalid outcome?
            objectToNotify.Notify();
        }

        public void DisableCallback() { }

        #endregion
    }

    public class InvalidOutcome<T> : InvalidOutcome, IOutcome<T>
    {
        public new static readonly InvalidOutcome<T> Default = new InvalidOutcome<T>();

        public T Value { get; set; } = default(T);
    }

}