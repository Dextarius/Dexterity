using Core.Causality;
using Core.Factors;
using JetBrains.Annotations;

namespace Factors
{
    //- TODO : Do we even need this class?
    public abstract class Proactor : Factor
    {
        [NotNull] 
        protected abstract IState State { get; }
        public override bool IsConsequential => State.IsConsequential;

        
        protected Proactor(string name) : base(name)
        {
        }
    }
}