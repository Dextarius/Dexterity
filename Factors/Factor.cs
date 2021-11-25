using Core.Causality;
using Core.Factors;
using Core.States;
using JetBrains.Annotations;

namespace Factors
{
    public abstract class Factor : IFactor
    {
        #region Constants

        private const string DefaultName = nameof(Factor);

        #endregion
        
        
        #region Properties

        public         string Name               { get; }
        public virtual bool   HasDependents      => GetFactorImplementation().HasDependents;
        public virtual int    NumberOfDependents => GetFactorImplementation().NumberOfDependents;
        public virtual int    Priority           => GetFactorImplementation().Priority;
        public virtual bool   IsNecessary        => GetFactorImplementation().IsNecessary;

        #endregion

        
        #region Instance Methods

        public virtual bool AddDependent(IDependency dependent)     => GetFactorImplementation().AddDependent(dependent);
        public virtual void ReleaseDependent(IDependency dependent) => GetFactorImplementation().ReleaseDependent(dependent);
        public virtual void InvalidateDependents()                  => GetFactorImplementation().InvalidateDependents();
        public virtual void NotifyNecessary()                       => GetFactorImplementation().NotifyNecessary();
        public virtual void NotifyNotNecessary()                    => GetFactorImplementation().NotifyNotNecessary();
        public virtual bool Reconcile()                             => GetFactorImplementation().Reconcile();
        public virtual void NotifyInvolved()                        => GetFactorImplementation().NotifyInvolved();
        public virtual void OnChanged()                             => GetFactorImplementation().OnChanged();

        
        protected abstract IFactor GetFactorImplementation();

        #endregion


        #region Constructors

        protected Factor(string nameToGive = DefaultName)
        {
            Name = nameToGive ?? DefaultName;
        }
        
        #endregion
        
    }
}