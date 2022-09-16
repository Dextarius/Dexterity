using System;
using Core.Factors;
using Core.States;
using Factors.Cores;
using JetBrains.Annotations;

namespace Factors
{
    public abstract class TriggerFactor<TCore> : Factor<TCore>, ITriggerCoreCallback
        where TCore : TriggerCore
    {
        #region Instance Properties

        public bool HasTriggers      => core.HasTriggers;
        public int  NumberOfTriggers => core.NumberOfTriggers;
        public bool IsUnstable       => core.IsUnstable;
        public bool IsStabilizing    => core.IsStabilizing;
        public bool IsTriggered      => core.IsTriggered;

        #endregion
        

        #region Instance Methods

        public bool Destabilize() => core.Destabilize();

        public bool Trigger() => core.Trigger();

        public bool Trigger(IFactor triggeringFactor, long triggerFlags, out bool removeSubscription) =>
            core.Trigger(triggeringFactor, triggerFlags, out removeSubscription);

        public override bool Subscribe(IFactorSubscriber subscriberToAdd, bool isNecessary)
        {
            // if (IsReacting) { throw new InvalidOperationException("Recursive dependency"); } //- TODO : Better error message.

            bool successfullySubscribed = base.Subscribe(subscriberToAdd, isNecessary);

            //- Should we be Reacting if the subscriber is Necessary?
            
            if (successfullySubscribed &&
                isNecessary is false   &&
                IsTriggered || IsUnstable)
            {
                if (subscriberToAdd.Destabilize())
                {
                    NotifyNecessary(subscriberToAdd);
                }
            }

            return successfullySubscribed;
        }

        public bool DestabilizeSubscribers()
        {
            if (influence is null) { return false; }
            
            bool wasAlreadyNecessary     = Influence.HasNecessarySubscribers;
            bool hasNecessarySubscribers = Influence.DestabilizeSubscribers(this);

            if (hasNecessarySubscribers &&
                wasAlreadyNecessary is false)
            {
                OnNecessary();
                //  If we're Reflexive will we start updating in
                //  the middle of someone else propagating Destabilize()?
            }

            return hasNecessarySubscribers;
        }

        protected override void OnNecessary()
        {
            if (IsNecessary is false)
            {
                core.OnNecessary();
            }
        }

        protected override void OnNotNecessary()
        {
            if (IsNecessary)
            {
                core.OnNotNecessary();
            }
        }

        public override bool Reconcile() => core.Reconcile();

        public override void SwapCore(TCore newCore)
        {
            var oldCore = core;
            
            base.SwapCore(newCore);
            
          // newCore.IsNecessary = IsNecessary;
        }

        #endregion


        #region Constructors

        //- TODO : We shouldn't need to give the core a name and ourselves a name as well.
        protected TriggerFactor(TCore reactorCore, string nameToGive) : base(reactorCore, nameToGive)
        {
            reactorCore.SetCallback(this);
        }

        #endregion

        bool ITriggerCoreCallback.CoreUpdated(TriggerCore triggerCore)
        {
            throw new NotImplementedException();
        }
        
        bool ITriggerCoreCallback.CoreDestabilized(TriggerCore destabilizedCore)
        {
            if (EnsureIsCorrectCore(destabilizedCore))
            {
                return DestabilizeSubscribers();
            }
            else return false;
        }
        
        void ITriggerCoreCallback.CoreTriggered(TriggerCore triggeredCore)
        {
            if (EnsureIsCorrectCore(triggeredCore))
            {
                Influence.DestabilizeSubscribers(this);
            }
        }
    }
    
    
        public abstract class Trigger<TCore> : IDeterminant, IInfluenceOwner
        where TCore : IFactorCore
    {
        #region Constants

        private const string DefaultName = nameof(Factor<TCore>);

        #endregion


        #region Instance Fields
        
        protected Influence influence;
        private   TCore     core;

        #endregion


        #region Instance Properties

        public            string     Name                    { get; }
        protected         IInfluence Influence               => influence ??= new Influence();
        public    virtual bool       IsNecessary             => HasNecessarySubscribers;
        public            bool       HasSubscribers          => influence?.HasSubscribers ?? false;
        public            bool       HasNecessarySubscribers => influence?.HasNecessarySubscribers ?? false;
        public            int        NumberOfSubscribers     => influence?.NumberOfSubscribers ?? 0;
        public            int        UpdatePriority          => core.UpdatePriority;
        public            uint       VersionNumber           { get; protected set; }

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
        
        protected virtual void OnFirstSubscriberGained()             { }
        protected virtual void OnLastSubscriberLost()                { }
        protected virtual void OnNecessary()                         { }
        protected virtual void OnNotNecessary()                      { }

        public virtual bool Reconcile() => core.Reconcile();

        protected virtual void OnUpdated()
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

        protected Trigger(TCore factorCore, string factorsName = DefaultName)
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