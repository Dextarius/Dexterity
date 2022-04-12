using System;
using System.Collections.Generic;
using System.Diagnostics;
using Core.Factors;
using Core.States;
using static Core.Factors.Reactors;
using static Core.Tools.Types;

namespace Factors.Cores
{
    public abstract class ReactorCore : FactorCore, IReactor, IUpdateable
    {
        #region Instance Fields

        protected WeakReference<IFactorSubscriber> weakReferenceToSelf;
        protected bool                             removeSubscriptionWhenTriggered;
        private   bool                             isReflexive;

        #endregion


        #region Instance Properties

        public             bool                  IsReacting           { get; protected set; }
        public             bool                  IsStabilizing        { get; protected set; }
        protected          bool                  IsQueued             { get; set; }
        protected abstract IEnumerable<IXXX> Triggers             { get; }
        public    abstract bool                  HasTriggers          { get; }
        public    abstract int                   NumberOfTriggers     { get; }
        public             bool                  IsUnstable           { get; protected set; }
        public             bool                  HasBeenTriggered     { get; protected set; } = true;
        public    override bool                  IsNecessary          => IsReflexive || base.IsNecessary;
        public             bool                  HasReacted           => NumberOfTimesReacted > 0;
        public             uint                  NumberOfTimesReacted { get; private set; }

        public WeakReference<IFactorSubscriber> WeakReference => weakReferenceToSelf ??= new WeakReference<IFactorSubscriber>(this);
        
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

        //^ What happens if someone sets IsReflexive to true inside an update method, on a Reactor that 
        //  has been invalidated during that same update process?
        
        #endregion

        
        #region Instance Methods

        public bool ForceReaction() => React();

        protected bool React()
        {
            bool outcomeChanged;

            if (IsReacting)                { Debug.Fail($"Update loop in {NameOf<ReactorCore>()} => {this}."); }
            if (HasBeenTriggered is false) { InvalidateOutcome(null); }
            
            IsReacting       = true;
            IsUnstable       = false;
            HasBeenTriggered = false;
    
            try
            {
                outcomeChanged = GenerateOutcome();
            }
            catch (Exception e)
            {
                //- TODO : Consider storing exceptions as an accessible field,
                //         similar to some of the reactives available in other libraries.
                
             // InvalidateOutcome(null);
                throw;
            }
            finally
            {
                IsReacting = false;
            }
            
            if (outcomeChanged)
            {
                TriggerSubscribers();
                NumberOfTimesReacted++;

                return true;
            }
            else return false;
        }
        
        protected abstract bool GenerateOutcome();

        public bool AttemptReaction()
        {
            if (HasBeenTriggered)
            {
                return React();
            }
            else if (IsUnstable)
            {
                if (TryStabilizeOutcome())
                {
                    Debug.Assert(HasBeenTriggered is false);

                    return true;
                }
                else
                {
                    return React() is false;
                }
            }
            else return false;
        }

        protected bool TryStabilizeOutcome()
        {
            bool successfullyReconciled = true;
            
            IsStabilizing = true;

            foreach (var determinant in Triggers)
            {
                if (determinant.Reconcile() is false)
                {
                    successfullyReconciled = false;
                    Debug.Assert(HasBeenTriggered);
                    //^ If the reconciliation failed then the trigger should have reacted and triggered its subscribers

                    break;
                    //- No point in stabilizing the rest.  We try to stabilize to avoid updating, and if one
                    //  of our triggers fails to stabilize then we have no choice but to update.  In addition
                    //  we don't even know if we'll use the same triggers when we update. If we do end
                    //  up accessing those same triggers, they'll try to stabilize themselves when we access
                    //  them anyways.
                }
            }
            
            IsUnstable    = false;
            IsStabilizing = false;

            return successfullyReconciled;
        }

        public bool Trigger() => Trigger(null, out _);

        public bool Trigger(IFactor triggeringFactor, out bool removeSubscription)
        {
            removeSubscription = removeSubscriptionWhenTriggered;
            
            if (IsReacting)
            {
                throw new InvalidOperationException(
                    "A Reactor was invalidated while it was updating, meaning either, the Reactor's update process " +
                    "caused it to invalidate itself, creating an update loop, " +
                    "or the Outcome was accessed by two different threads at the same time. \n  " +
                   $"The invalidated outcome was '{this}' and it was invalidated by '{triggeringFactor}'. ");
                //- If this Outcome is in the update list we should know it's a loop, if it's not then it should be
                //  another thread accessing it.
                //  Well actually, the parent won't add us to the list until this returns...
            }
            
            if (HasBeenTriggered is false)
            {
                Debug.Assert(IsQueued is false);
                HasBeenTriggered = true;
                InvalidateOutcome(triggeringFactor);

                if (IsNecessary || DestabilizeSubscribers())
                {
                    UpdateOutcome();
                }

                return true;
            }
    
            return false;
        }

        protected abstract void InvalidateOutcome(IFactor changedParentState);
        
        protected virtual void UpdateOutcome()
        {
            IsQueued = true;
            UpdateList.Update(this);
        }

        public bool Destabilize()
        {
            if (IsStabilizing)
            {
                return false;
            }
            else if (IsNecessary)
            {
                return true;
                
                //- We shouldn't need to mark ourselves as Unstable. If we're Necessary then either
                //  we're going to be triggered when our parent updates, or the parent won't
                //  change, in which case we aren't Unstable.
            }
            else if (HasBeenTriggered)
            {
                return false;
            }
            else if (IsUnstable is false) 
            {
                bool hasNecessaryDependents = DestabilizeSubscribers();
                
                if (hasNecessaryDependents)
                {
                    return true;
                }
                else
                {
                    IsUnstable = true;
                }
            }
    
            return false;
        }
        
        protected bool DestabilizeSubscribers()
        {
            var subscribersToDestabilize = allSubscribers;
            
            if (subscribersToDestabilize.Count > 0) 
            {
                foreach (var subscriber in subscribersToDestabilize)
                {
                    if (subscriber.Destabilize())
                    {
                        necessarySubscribers.Add(subscriber);
                        return true;
                    }
                }
                
                //- TODO : We specifically tried to avoid foreach when triggering subscribers because they might
                //         choose to remove themselves, so consider if we really want to use it here.  
            }
            return false;
        }
        
        public override bool Subscribe(IFactorSubscriber subscriberToAdd, bool isNecessary)
        {
            if (IsReacting) { throw new InvalidOperationException("Recursive dependency"); } //- TODO : Better error message.

            bool successfullySubscribed = base.Subscribe(subscriberToAdd, isNecessary);

            if (successfullySubscribed && 
                isNecessary is false   &&
                HasBeenTriggered  ||  IsUnstable)
            {
                subscriberToAdd.Destabilize();
                //- TODO : Doesn't this break the rule that Destabilize is supposed to cause the caller to update 
                //         based on its return value?
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

        private void OnNecessary()
        {
            if (HasBeenTriggered || IsUnstable)
            {
                AttemptReaction();
                //- TODO : This seems like it could lead to some weird behavior when combined with the fact that Reactives
                //         already update before returning a value. Think about it for a bit when you get a chance.
            }

            //- TODO : Should we check if we're already reacting, or if we're queued?
            
            //- We don't propagate the Necessary status to avoid walking up the tree.
            //  All Reactors walk down the tree to destabilize their dependents, so we can let them know then
            //  if need be.
        }
        
        private void OnNotNecessary()
        {
            if (HasReacted)
            {
                foreach (var trigger in Triggers)
                {
                    trigger.NotifyNotNecessary(this);
                }
            }
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

        #endregion


        #region Constructors

        protected ReactorCore(string name) : base(name)
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