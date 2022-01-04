using System;
using System.Collections.Generic;
using System.Diagnostics;
using Core;
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
        

        #region Instance Properties

        public          string Name               { get; }
        public abstract bool   IsNecessary        { get; }
        public abstract bool   HasDependents      { get; }
        public abstract int    NumberOfDependents { get; }
        public abstract int    Priority           { get; }

        #endregion


        #region Instance Methods
        
        public abstract bool AddDependent(IDependent dependentToAdd);
        public abstract void RemoveDependent(IDependent dependentToRemove);
        public abstract void InvalidateDependents();
        public abstract void NotifyNecessary();
        public abstract void NotifyNotNecessary();

        public abstract bool Reconcile();
        
        #endregion
        

        #region Constructors

        protected Factor(string nameToGive = DefaultName)
        {
            Name = nameToGive ?? DefaultName;
        }
        
        #endregion
    }
}