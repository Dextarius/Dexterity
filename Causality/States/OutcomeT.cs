using Core.Causality;
using Core.Factors;


namespace Causality.States
{
    public class Outcome<T> : Outcome, IOutcome<T>, IState<T>
    {
        #region Instance Fields

        protected T currentValue;

        #endregion
        
        
        #region Instance Properties
        
        public T Value
        {
            get
            {
                Observer.NotifyInvolved(this);
                return currentValue;
            }
            set => currentValue = value;
        }

        #endregion


        #region Instance Methods

        public T Peek() => currentValue;

        #endregion


        #region Constructors

        public Outcome(INotifiable reactor) : base(reactor)
        {
        }
        
        public Outcome() : this(null)
        {
        }

        #endregion
    }
}