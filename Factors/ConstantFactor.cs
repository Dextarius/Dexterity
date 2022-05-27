using Core.Factors;
using Core.States;

namespace Factors
{
    public class ConstantFactor : IFactor
    {
        #region Properties

        public string Name           { get; }
        public int    UpdatePriority => 0;
        public uint   VersionNumber  => 1; 

        #endregion

        #region Instance Methods

        public bool Reconcile()   => true;
        
        public bool Subscribe(IFactorSubscriber subscriberToAdd, bool isNecessary) => false;

        public void Unsubscribe(IFactorSubscriber subscriberToRemove)           { }
        public void NotifyNecessary(IFactorSubscriber necessarySubscriber)      { }
        public void NotifyNotNecessary(IFactorSubscriber unnecessarySubscriber) { }

        #endregion


        #region Constructors

        public ConstantFactor(string name)
        {
            Name = name;
        }

        #endregion
    }
}