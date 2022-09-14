﻿using System;
using System.Collections.Generic;
using Core;
using Core.Factors;
using Core.States;
using Factors.Observer;
using JetBrains.Annotations;

namespace Factors
{
    public abstract class Factor<TCore> : Factor, IDeterminant, IInfluenceOwner
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
        protected      IInfluence Influence               => influence ??= new Influence();
        public virtual bool       IsNecessary             => HasNecessarySubscribers;
        public         bool       HasSubscribers          => influence?.HasSubscribers ?? false;
        public         bool       HasNecessarySubscribers => influence?.HasNecessarySubscribers ?? false;
        public         int        NumberOfSubscribers     => influence?.NumberOfSubscribers ?? 0;
        public         int        UpdatePriority          => core.UpdatePriority;
        public         uint       VersionNumber           { get; protected set; }

        #endregion


        #region Instance Methods

        public virtual bool Subscribe(IFactorSubscriber subscriberToAdd, bool isNecessary)
        {
            bool alreadyHadSubscribers = HasSubscribers;
            bool wasAlreadyNecessary   = HasNecessarySubscribers;

            if (Influence.Subscribe(subscriberToAdd, isNecessary))
            {
                if (alreadyHadSubscribers    is false &&
                    Influence.HasSubscribers is true)
                {
                    OnFirstSubscriberGained();
                }

                if (isNecessary)
                {
                    if (wasAlreadyNecessary is false &&
                        Influence.HasNecessarySubscribers)
                    {
                        OnNecessary();
                    }
                }

                return true;
            }
            else return false;
        }

        public void Unsubscribe(IFactorSubscriber subscriberToRemove)
        {
            if (subscriberToRemove != null)
            {
                bool hadSubscribers = Influence.HasSubscribers;
                bool wasNecessary   = Influence.HasNecessarySubscribers;

                if (Influence.Unsubscribe(subscriberToRemove))
                {
                    if (hadSubscribers &&
                        Influence.HasSubscribers is false)
                    {
                        OnLastSubscriberLost();
                    }
                    
                    if (wasNecessary &&
                        Influence.HasNecessarySubscribers is false)
                    {
                        OnNotNecessary();
                    }
                }
            }
        }

        public virtual void NotifyNecessary(IFactorSubscriber necessarySubscriber)
        {
            if (influence != null)
            {
                bool wasAlreadyNecessary = HasNecessarySubscribers;
            
                Influence?.NotifyNecessary(necessarySubscriber);

                if (wasAlreadyNecessary is false)
                {
                    OnNecessary();
                
                    //- TODO : OnNecessary is going to update a Reactor, so do we want that to
                    //         happen before or after we add the subscriber?
                }
            }
        }

        public virtual void NotifyNotNecessary(IFactorSubscriber unnecessarySubscriber)
        {
            if (HasNecessarySubscribers)
            {
                Influence.NotifyNotNecessary(unnecessarySubscriber);

                if (HasNecessarySubscribers is false)
                {
                    OnNotNecessary();
                }
            }
        }

        public void TriggerSubscribers(long triggerFlags) => influence?.TriggerSubscribers(this, triggerFlags);
        public void TriggerSubscribers()                  => TriggerSubscribers(TriggerFlags.Default);

        protected virtual void OnFirstSubscriberGained() { }
        protected virtual void OnLastSubscriberLost()    { }
        protected virtual void OnNecessary()             { }
        protected virtual void OnNotNecessary()          { }

        public virtual bool Reconcile() => core.Reconcile();

        protected virtual void OnUpdated(long triggerFlags)
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
        
        protected virtual bool EnsureIsCorrectCore(IFactorCore coreToTest)
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

    
    public abstract class Factor
    {
        // #region Constants
        //
        // public const long TriggerFlags.Default        = ~0;
        // public const long NoTriggerFlags             =  0;
        // public const int  NumberOfBitsInTriggerFlags = 64;
        //
        //
        // #endregion
    }
}
