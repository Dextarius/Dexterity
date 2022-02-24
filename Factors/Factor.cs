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
    public abstract class Factor<TCore> : IFactor  where TCore : IFactor
    {
        #region Constants

        private const string DefaultName = nameof(Factor<TCore>);

        #endregion

        
        #region Instance Fields

        protected readonly TCore core;

        #endregion
        

        #region Instance Properties

        public string Name                { get; }
        public bool   IsNecessary         => core.IsNecessary;
        public bool   HasSubscribers      => core.HasSubscribers;
        public int    NumberOfSubscribers => core.NumberOfSubscribers;
        public int    Priority            => core.Priority;

        #endregion


        #region Instance Methods

        public bool Subscribe(IFactorSubscriber subscriberToAdd)      => core.Subscribe(subscriberToAdd);
        public void Unsubscribe(IFactorSubscriber subscriberToRemove) => core.Unsubscribe(subscriberToRemove);
        public void TriggerSubscribers()                              => core.TriggerSubscribers();
        public void NotifyNecessary()                                 => core.NotifyNecessary();
        public void NotifyNotNecessary()                              => core.NotifyNotNecessary();
        public bool Reconcile()                                       => core.Reconcile();
        
        #endregion
        

        #region Constructors

        protected Factor(TCore factorCore, string factorsName = DefaultName)
        {
            core = factorCore  ?? throw new ArgumentNullException(nameof(factorCore));
            Name = factorsName ?? DefaultName;
        }
        
        #endregion
    }
}