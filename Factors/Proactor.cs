using Core.Causality;
using Core.Factors;
using Core.States;
using Factors.Outcomes.Influences;
using JetBrains.Annotations;

namespace Factors
{
    //- TODO : Do we really need this class?
    public abstract class Proactor : Factor
    {
        protected abstract IFactor Influence          { get; }
        public override    bool    IsNecessary        => Influence.IsNecessary;
        public override    bool    HasDependents      => Influence.HasDependents;
        public    override int     NumberOfDependents => Influence.NumberOfDependents;
        public override    int     Priority           => Influence.Priority;

        public override bool AddDependent(IDependent dependentToAdd)       => Influence.AddDependent(dependentToAdd);
        public override void RemoveDependent(IDependent dependentToRemove) => Influence.RemoveDependent(dependentToRemove);
        public override void InvalidateDependents()                        => Influence.InvalidateDependents();
        public override void NotifyNecessary()                             => Influence.NotifyNecessary();
        public override void NotifyNotNecessary()                          => Influence.NotifyNotNecessary();
        public override bool Reconcile()                                   => Influence.Reconcile();


        protected Proactor(string name) : base(name)
        {
            
        }
    }
}