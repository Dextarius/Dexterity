using Core.Causality;
using Core.Factors;

namespace Factors
{
    public class Proaction : Proactor
    {
        protected IState state;

        protected override IState State => state;

        public void Act()
        {
            State.NotifyInvolved();
        }

        public Proaction(string name) : base(name)
        {
        }
    }
}