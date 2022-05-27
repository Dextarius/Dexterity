using System;
using System.Diagnostics;
using Core.Factors;
using Core.States;
using Factors.Cores;
using JetBrains.Annotations;
using static Core.Tools.Delegates;
using static Core.Tools.Types;


namespace Factors
{
    public class Reactor<TCore> : Factor<TCore>, IReactor, IUpdateable, IReactorCoreOwner
        where TCore : IReactorCore
    {
        #region Instance Fields

        private bool isReflexive;

        #endregion

        #region Instance Properties

        public    bool HasTriggers      => core.HasTriggers;
        public    int  NumberOfTriggers => core.NumberOfTriggers;
        public    bool IsUnstable       => core.IsUnstable;
        public    bool IsReacting       => core.IsReacting;
        public    bool IsStabilizing    => core.IsStabilizing;
        public    bool HasBeenTriggered => core.HasBeenTriggered;
        public    bool HasReacted       => core.HasReacted; 
        protected bool IsQueued         { get; set; }


        public bool IsReflexive
        {
            get => isReflexive;
            set
            {
                if (value is true)
                {
                    if (isReflexive is false)
                    {
                        isReflexive = true;

                        if (base.IsNecessary is false)
                        {
                            OnNecessary();
                        }
                    }
                }
                else if (isReflexive is true)
                {
                    isReflexive = false;

                    if (base.IsNecessary is false)
                    {
                        OnNotNecessary();
                    }
                }
            }
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
        
        protected bool React()
        {
            bool outcomeChanged = core.GenerateOutcome();

            if (outcomeChanged)
            {
                TriggerSubscribers();

                return true;
            }
            else return false;
        }
        
        public bool AttemptReaction()
        {
            if (HasBeenTriggered)
            {
                return React();
            }
            else if (IsUnstable)
            {
                if (core.TryStabilizeOutcome())
                {
                    Debug.Assert(HasBeenTriggered is false);

                    return false;
                }
                else
                {
                    return React();
                }
            }
            else return false;
        }
        
        public bool ForceReaction() => React();

        //- Does not imply the caller will queue this Outcome to be updated.
        //  Only that the caller should be notified if this Outcome is Necessary
        //  and if not that it should mark itself and its dependents as Unstable
        public bool Destabilize(IFactor factor) => core.Destabilize(factor);
        
        public bool CoreDestabilized()
        {
            bool hasNecessaryDependents = DestabilizeSubscribers();

            return hasNecessaryDependents;
        }
        
        protected bool DestabilizeSubscribers()
        {
            var subscribersToDestabilize = allSubscribers;

            if (subscribersToDestabilize.Count > 0)
            {
                foreach (var subscriber in subscribersToDestabilize)
                {
                    if (subscriber.Destabilize(this))
                    {
                        necessarySubscribers.Add(subscriber);
                        return true;
                    }
                }

                //- TODO : We specifically tried to avoid using foreach before when triggering subscribers because they might
                //         choose to remove themselves, so consider if we really want to use it here.  
            }
            return false;
        }
        
        public bool Trigger() => core.Trigger();
        
        public bool Trigger(IFactor triggeringFactor, out bool removeSubscription) =>
            core.Trigger(triggeringFactor, out removeSubscription);
        
        public void CoreTriggered()
        {
            Debug.Assert(IsQueued is false);

            if (IsNecessary || DestabilizeSubscribers())
            {
                UpdateOutcome();
            }
        }

        public override bool Subscribe(IFactorSubscriber subscriberToAdd, bool isNecessary)
        {
            if (IsReacting) { throw new InvalidOperationException("Recursive dependency"); } //- TODO : Better error message.

            bool successfullySubscribed = base.Subscribe(subscriberToAdd, isNecessary);

            if (successfullySubscribed &&
                isNecessary is false &&
                HasBeenTriggered || IsUnstable)
            {
                subscriberToAdd.Destabilize(this);
                //- TODO : Doesn't this break the rule that Destabilize is supposed to request the caller to update 
                //         if the dependent returns true?
            }

            return successfullySubscribed;
        }
        
        protected override void AddSubscriberAsNecessary(IFactorSubscriber necessarySubscriber)
        {
            bool wasAlreadyNecessary = IsNecessary;

            base.AddSubscriberAsNecessary(necessarySubscriber);

            if (wasAlreadyNecessary is false)
            {
                OnNecessary();

                //- TODO : OnNecessary is going to update this Reactor, so do we want that to happen before or after
                //         we add the subscriber?
            }
        }

        protected override void RemoveSubscriberFromNecessary(IFactorSubscriber unnecessarySubscriber)
        {
            if (base.IsNecessary)
            {
                base.RemoveSubscriberFromNecessary(unnecessarySubscriber);

                if (IsNecessary is false)
                {
                    OnNotNecessary();
                }
            }
        }
        
        protected virtual void OnNecessary()
        {
            if (HasBeenTriggered || IsUnstable)
            {
                AttemptReaction();
                //- TODO : This seems like it could lead to some weird behavior when combined with the fact that Reactives
                //         already update before returning a value. Think about it for a bit when you get a chance.
            }

            core.OnNecessary();

            //- TODO : Should we check if we're already reacting, or if we're queued?
        }

        protected virtual void OnNotNecessary()
        {
            core.OnNotNecessary();
        }

        public override bool Reconcile()
        {
            if (HasBeenTriggered || IsUnstable)
            {
                return AttemptReaction() is false;
            }
            else return true;

            //- This method is intended to be used by factors that have been made unstable.
            //  If they were made unstable, then we (or another Reactor) were either triggered or made unstable,
            // and in turn made them unstable. If we are no longer triggered or unstable then we reacted
            // before this call, and we would have triggered our subscribers if the reaction 'changed something'.
            // Since they're trying to reconcile, then they haven't been triggered, so we accept the reconciliation.

            //- TODO : We could probably just return "AttemptReaction() is false" if we can guarantee that
            //         AttemptReaction() is always going to be based on HasBeenTriggered and IsUnstable anyways.
        }

        protected virtual void UpdateOutcome()
        {
            IsQueued = true;
            UpdateList.Update(this);
        }

        #endregion


        #region Constructors

        //- TODO : We shouldn't need to give the core a name and ourselves a name as well.
        protected Reactor(TCore reactorCore, string nameToGive) : base(reactorCore, nameToGive)
        {

        }

        #endregion


        #region Explicit Implementations

        void IUpdateable.Update()
        {
            IsQueued = false;
            AttemptReaction();
        }

        #endregion
    }
}