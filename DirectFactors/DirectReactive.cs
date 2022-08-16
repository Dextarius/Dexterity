using System.Diagnostics;
using Core;
using Core.Factors;
using Core.States;
using JetBrains.Annotations;

namespace DirectFactors
{
    public class DirectReactive<T> : DirectFactor<T>, IReactiveCoreOwner<T>, IDirectReactive<T>, IUpdateable
    {
        #region Instance Fields

        private   bool             isReflexive;
        protected IReactiveCore<T> core;

        #endregion

        
        #region Instance Properties

        public override T    Value            => core.Value;
        public          bool HasTriggers      => core.HasTriggers;
        public          int  NumberOfTriggers => core.NumberOfTriggers;
        public          bool IsUnstable       => core.IsUnstable;
        public          bool IsReacting       => core.IsReacting;
        public          bool IsStabilizing    => core.IsStabilizing;
        public          bool HasBeenTriggered => core.HasBeenTriggered;
        public          bool HasReacted       => core.HasReacted;
        protected       bool IsQueued         { get; set; }

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
        

        #region Instance Methods

        protected bool React()
        {
            bool outcomeChanged = core.GenerateOutcome(out T oldValue, out T newValue);

            if (outcomeChanged)
            {
                NotifySubscribersValueChanged(oldValue, newValue);

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

        public void CoreTriggered(IReactiveCore<T> triggeredCore)
        {
            Debug.Assert(IsQueued is false); //< Is this still valid?

            if (IsNecessary || DestabilizeSubscribers())
            {
                UpdateOutcome();
            }
        }
        
        public bool Destabilize(IDirectFactor factor) => core.Destabilize(factor);

        public bool CoreDestabilized(IReactiveCore<T> destabilizedCore)
        {
            if (ReferenceEquals(core, destabilizedCore) is false)
            {
                //- We probably swapped out the core but forgot to Dispose() of the old one or something.
                core.Dispose();
                return false;
            }
            
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
                //-        Well we don't really want to use the RemoveWhere() method like we did in TriggerSubscribers()
                //         because we want to end this method as soon as we find a necessary subscriber.
            }
            
            return false;
        }

        public void CoreValueChanged(IReactiveCore<T> changedCore, T oldValue, T newValue)
        {
            NotifySubscribersValueChanged(oldValue, newValue);
        }

        public bool Recalculate()
        {
            throw new NotImplementedException();
        }
        
        protected virtual void OnNecessary()
        {
            core.OnNecessary();

            if (HasBeenTriggered || IsUnstable)
            {
                AttemptReaction();
                //- TODO : This seems like it could lead to some weird behavior when combined with the fact that Reactives
                //         already update before returning a value. Think about it for a bit when you get a chance.
            }
            
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
        
        
        #region Explicit Implementations

        void IUpdateable.Update()
        {
            IsQueued = false;
            AttemptReaction();
        }

        #endregion
    }

    public interface IDirectReactive<T> : IDirectFactor<T>, IPrioritizedUpdate
    {
        bool IsUnstable       { get; }
        bool IsReacting       { get; }
        bool IsStabilizing    { get; }
        bool HasTriggers      { get; }
        bool HasBeenTriggered { get; }
        int  NumberOfTriggers { get; }
        bool IsReflexive      { get; set; }
        
        bool AttemptReaction();
        bool ForceReaction();
    }

}