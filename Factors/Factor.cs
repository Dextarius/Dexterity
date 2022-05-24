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
    public abstract class Factor<TCore> : IDeterminant  where TCore : IDeterminant
    {
        #region Constants

        private const string DefaultName = nameof(Factor<TCore>);

        #endregion

        
        #region Instance Fields

        protected TCore core;

        #endregion
        

        #region Instance Properties

        public string Name                { get; }
        public bool   IsNecessary         => core.IsNecessary;
        public bool   HasSubscribers      => core.HasSubscribers;
        public int    NumberOfSubscribers => core.NumberOfSubscribers;
        public int    UpdatePriority      => core.UpdatePriority;
        public uint   VersionNumber       => core.VersionNumber;


        #endregion


        #region Instance Methods

        public bool Subscribe(IFactorSubscriber subscriberToAdd, bool isNecessary) => core.Subscribe(subscriberToAdd, isNecessary);
        public void Unsubscribe(IFactorSubscriber subscriberToRemove)              => core.Unsubscribe(subscriberToRemove);
        public void NotifyNecessary(IFactorSubscriber necessarySubscriber)         => core.NotifyNecessary(necessarySubscriber);
        public void NotifyNotNecessary(IFactorSubscriber unnecessarySubscriber)    => core.NotifyNotNecessary(unnecessarySubscriber);
        public void TriggerSubscribers()                                           => core.TriggerSubscribers();
        public bool Reconcile()                                                    => core.Reconcile();

        public override string ToString() => core.ToString();

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