using System;
using System.Collections.Generic;
using Core;
using Core.Factors;
using Core.States;
using JetBrains.Annotations;

namespace Factors
{
    public abstract class Factor<TCore> : IDeterminant, IInfluenceOwner
        where TCore : IFactorCore
    {
        #region Constants

        private const string DefaultName = nameof(Factor<TCore>);

        #endregion


        #region Instance Fields

        [NotNull]
        protected TCore     core;
        protected Influence influence;

        #endregion


        #region Instance Properties

        public         string     Name                    { get; }
        protected      IInfluence Influence               => influence ??= new Influence(this);
        public virtual bool       IsNecessary             => HasNecessarySubscribers;
        public         bool       HasSubscribers          => Influence.HasSubscribers;
        public         bool       HasNecessarySubscribers => Influence.HasNecessarySubscribers;
        public         int        NumberOfSubscribers     => Influence.NumberOfSubscribers;
        public         int        UpdatePriority          => core.UpdatePriority;
        public         uint       VersionNumber           { get; protected set; }

        #endregion


        #region Instance Methods

        public virtual bool Subscribe(IFactorSubscriber subscriberToAdd, bool isNecessary) => Influence.Subscribe(subscriberToAdd, isNecessary);
        public         void Unsubscribe(IFactorSubscriber subscriberToRemove)              => Influence.Unsubscribe(subscriberToRemove);
        
        public virtual void NotifyNecessary(IFactorSubscriber necessarySubscriber) => 
            Influence.NotifyNecessary(necessarySubscriber);

        public virtual void NotifyNotNecessary(IFactorSubscriber unnecessarySubscriber) =>
            Influence.NotifyNotNecessary(unnecessarySubscriber);
        
        public void TriggerSubscribers() => Influence.TriggerSubscribers(this);

        protected virtual void OnFirstSubscriberGained() { }
        protected virtual void OnLastSubscriberLost()    { }
        protected virtual void OnNecessary()             { }
        protected virtual void OnNotNecessary()          { }

        public virtual bool Reconcile() => core.Reconcile();

        public virtual void OnUpdated()
        {
            VersionNumber++;
            TriggerSubscribers();
        }

        public virtual void SwapCore(TCore newCore)
        {
            if (newCore is null) { throw new ArgumentNullException(nameof(newCore)); }
            
            var oldCore = core;

            core = newCore;
            oldCore.Dispose();
        }
        
        protected bool EnsureIsCorrectCore(IFactorCore coreToTest)
        {
            if (ReferenceEquals(core, coreToTest) is false)
            {
                //- We probably swapped out the core but forgot to Dispose() of the old one or something.
                coreToTest.Dispose();
                return false;
            }
            else return true;
        }


        
        public override string ToString() => $"{Name} : {core.ToString()}";

        #endregion


        #region Constructors

        protected Factor(TCore factorCore, string factorsName = DefaultName)
        {
            core = factorCore  ?? throw new ArgumentNullException(nameof(factorCore));
            Name = factorsName ?? DefaultName;
        }

        #endregion


        #region Explicit Implementations

        void IInfluenceOwner.OnFirstSubscriberGained() => OnFirstSubscriberGained();
        void IInfluenceOwner.OnLastSubscriberLost()    => OnLastSubscriberLost();
        void IInfluenceOwner.OnNecessary()             => OnNecessary();
        void IInfluenceOwner.OnNotNecessary()          => OnNotNecessary();

        #endregion
    }
    
    
        public abstract class FactorSlim<TCore> : IDeterminant, IInfluenceOwner
        where TCore : IFactorCore
    {
        #region Constants

        private const string DefaultName = nameof(Factor<TCore>);

        #endregion


        #region Instance Fields

        [NotNull]
        protected TCore     core;
        protected Influence influence;

        #endregion


        #region Instance Properties

        public         string     Name                    { get; }
        protected      IInfluence Influence               => influence ??= new Influence(this);
        public virtual bool       IsNecessary             => HasNecessarySubscribers;
        public         bool       HasSubscribers          => Influence.HasSubscribers;
        public         bool       HasNecessarySubscribers => Influence.HasNecessarySubscribers;
        public         int        NumberOfSubscribers     => Influence.NumberOfSubscribers;
        public         int        UpdatePriority          => core.UpdatePriority;
        public         uint       VersionNumber           { get; protected set; } //X

        #endregion


        #region Instance Methods

        public virtual bool Subscribe(IFactorSubscriber subscriberToAdd, bool isNecessary) => Influence.Subscribe(subscriberToAdd, isNecessary);
        public         void Unsubscribe(IFactorSubscriber subscriberToRemove)              => Influence.Unsubscribe(subscriberToRemove);
        
        public virtual void NotifyNecessary(IFactorSubscriber necessarySubscriber) => 
            Influence.NotifyNecessary(necessarySubscriber);

        public virtual void NotifyNotNecessary(IFactorSubscriber unnecessarySubscriber) =>
            Influence.NotifyNotNecessary(unnecessarySubscriber);
        
        public void TriggerSubscribers() => Influence.TriggerSubscribers(this);

        protected virtual void OnFirstSubscriberGained() { }
        protected virtual void OnLastSubscriberLost()    { }
        protected virtual void OnNecessary()             { }
        protected virtual void OnNotNecessary()          { }

        public virtual bool Reconcile() => core.Reconcile();

        public virtual void OnUpdated()
        {
            VersionNumber++;
            TriggerSubscribers();
        }

        public virtual void SwapCore(TCore newCore)
        {
            if (newCore is null) { throw new ArgumentNullException(nameof(newCore)); }
            
            var oldCore = core;

            core = newCore;
            oldCore.Dispose();
        }
        
        protected bool EnsureIsCorrectCore(IFactorCore coreToTest)
        {
            if (ReferenceEquals(core, coreToTest) is false)
            {
                //- We probably swapped out the core but forgot to Dispose() of the old one or something.
                coreToTest.Dispose();
                return false;
            }
            else return true;
        }


        
        public override string ToString() => $"{Name} : {core.ToString()}";

        #endregion


        #region Constructors

        protected FactorSlim(TCore factorCore, string factorsName = DefaultName)
        {
            core = factorCore  ?? throw new ArgumentNullException(nameof(factorCore));
            Name = factorsName ?? DefaultName;
        }

        #endregion


        #region Explicit Implementations

        void IInfluenceOwner.OnFirstSubscriberGained() => OnFirstSubscriberGained();
        void IInfluenceOwner.OnLastSubscriberLost()    => OnLastSubscriberLost();
        void IInfluenceOwner.OnNecessary()             => OnNecessary();
        void IInfluenceOwner.OnNotNecessary()          => OnNotNecessary();

        #endregion
    }
}