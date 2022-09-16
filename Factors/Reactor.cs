using System;
using System.Diagnostics;
using Core.Factors;
using Core.States;
using JetBrains.Annotations;
using static Core.Tools.Delegates;
using static Core.Tools.Types;


namespace Factors
{
    public abstract class Reactor<TCore> : Factor<TCore>, IReactor, IReactorCoreCallback
        where TCore : IReactorCore
    {
        private bool isReflexive;

        #region Instance Properties

        public          bool HasTriggers      => core.HasTriggers;
        public          int  NumberOfTriggers => core.NumberOfTriggers;
        public          bool IsUnstable       => core.IsUnstable;
        public          bool IsReacting       => core.IsReacting;
        public          bool IsStabilizing    => core.IsStabilizing;
        public          bool IsTriggered      => core.IsTriggered;
        public          bool HasReacted       => VersionNumber > 0 || core.HasReacted;
        public override bool IsNecessary      => base.IsNecessary || IsReflexive;

        public bool IsReflexive
        {
            get => isReflexive;
            set
            {
                if (isReflexive != value)
                {
                    isReflexive = value;

                    if (core.IsReflexive != IsNecessary)
                    {
                        core.IsReflexive = IsNecessary;
                    }
                }
            }
        }
        
        public bool AutomaticallyReacts
        {
            get => core.AutomaticallyReacts;
            set => core.AutomaticallyReacts = value;
        }

        #endregion


        #region Static Methods

        protected static string CreateDefaultName<TReactor>(Delegate functionToCreateValue) =>
            $"{NameOf<TReactor>()} {GetClassAndMethodName(functionToCreateValue)}";

        protected static ArgumentNullException CannotConstructValueReactorWithNullProcess<T>()
            where T : Reactor<TCore> =>
            new ArgumentNullException(
                $"A {NameOf<T>()} cannot be constructed with a null process, as it would never have a value. ");

        protected static ArgumentNullException CannotConstructReactorWithNullProcess<T>()
            where T : Reactor<TCore> =>
            new ArgumentNullException(
                $"A {NameOf<T>()} cannot be constructed with a null process, as it would never do anything. ");

        #endregion


        #region Instance Methods

        public bool AttemptReaction() => core.AttemptReaction();
        public bool ForceReaction()   => core.ForceReaction();

        //- Does not imply the caller will queue this subscriber to be updated.  Only that the subscriber
        //  should mark itself and its dependents as Unstable, and return whether it is Necessary or not.
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
            
            bool wasAlreadyNecessary    = Influence.HasNecessarySubscribers;
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
            if (core.IsReflexive != IsNecessary)
            {
                core.IsReflexive = IsNecessary;
            }
        }

        protected override void OnNotNecessary()
        {
            if (core.IsReflexive != IsNecessary)
            {
                core.IsReflexive = IsNecessary;
            }
        }

        public override bool Reconcile() => core.Reconcile();

        public override void SwapCore(TCore newCore)
        {
            var oldCore = core;
            
            newCore.IsReflexive = IsNecessary;
            newCore.AutomaticallyReacts = oldCore.AutomaticallyReacts;
            
            base.SwapCore(newCore);
        }

        #endregion


        #region Constructors

        //- TODO : We shouldn't need to give the core a name and ourselves a name as well.
        protected Reactor(TCore reactorCore, string nameToGive) : base(reactorCore, nameToGive)
        {
            reactorCore.SetCallback(this);
        }

        #endregion


        #region Explicit Implementations

        void IFactorCoreCallback.CoreUpdated(IFactorCore triggeredCore, long triggerFlags)
        {
            if (EnsureIsCorrectCore(triggeredCore))
            {
                OnUpdated(triggerFlags);
            }
        }
        
        bool IReactorCoreCallback.ReactorDestabilized(IReactorCore destabilizedCore)
        {
            if (EnsureIsCorrectCore(destabilizedCore))
            {
                return DestabilizeSubscribers();
            }
            else return false;
        }
        
        bool IReactorCoreCallback.ReactorTriggered(IReactorCore triggeredCore)
        {
            if (EnsureIsCorrectCore(triggeredCore))
            {
                if (IsNecessary || Influence.DestabilizeSubscribers(this))
                {
                    return true;
                }
                else return false;
            }
            else return false;
        }

        #endregion
    }
}