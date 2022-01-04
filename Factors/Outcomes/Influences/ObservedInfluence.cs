using Core.Factors;
using Factors.Observer;

namespace Factors.Outcomes.Influences
{
    public class ObservedInfluence : Influence, IInvolved
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
        
        
        public ObservedInfluence(string name) : base(name)
        {
            
        }
    }
}