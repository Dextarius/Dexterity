using System;
using System.Collections.Generic;
using System.Diagnostics;
using Core.Factors;
using Core.States;
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

        public             bool                 IsReacting           { get; protected set; }
        public             bool                 IsStabilizing        { get; protected set; }
        protected abstract IEnumerable<IFactor> Triggers             { get; }
        public    abstract bool                 HasTriggers          { get; }
        public    abstract int                  NumberOfTriggers     { get; }
        public             bool                 IsUnstable           { get; protected set; }
        public             bool                 HasBeenTriggered     { get; protected set; } = true;
        public    override bool                 IsNecessary          => IsReflexive || base.IsNecessary;
        public             bool                 HasReacted           => NumberOfTimesReacted > 0;
        public             uint                 NumberOfTimesReacted { get; private set; }

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
            IsStabilizing    = false;
    
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
                bool wasNecessary = IsNecessary;
                
                TriggerSubscribers();
                NumberOfTimesReacted++;

                if (wasNecessary && IsNecessary is false)
                {
                    OnNotNecessary();
                }
                
                return true;
            }
            else return false;
        }
        
        protected abstract bool GenerateOutcome();

        public bool AttemptReaction()
        {
            if (HasBeenTriggered)
            {
                return React() is false;
            }
            else if (IsUnstable)
            {
                IsStabilizing = true;
                
                if (TryStabilizeOutcome())
                {
                    IsStabilizing = false;
                    IsUnstable    = false;
                    
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
            foreach (var determinant in Triggers)
            {
                if (determinant.Reconcile() is false)
                {
                    return false;
                    //- No point in stabilizing the rest.  We try to stabilize to avoid updating, and if one
                    //  of our triggers fails to stabilize then we have no choice but to update.  In addition
                    //  we don't even know if we'll use the same triggers when we update. If we do end
                    //  up accessing those same triggers, they'll try to stabilize themselves when we access
                    //  them anyways.
                }
            }

            return true;
        }

        public bool Trigger() => Trigger(null, out _);

        public bool Trigger(IFactor triggeringFactor, out bool removeSubscription)
        {
            removeSubscription = removeSubscriptionWhenTriggered;
            
            if (IsReacting)
            {
                throw new InvalidOperationException(
                    "An Outcome was invalidated while it was updating, meaning either the Outcome's update process " +
                    "caused it to invalidate itself, creating an update loop, " +
                    "or the Outcome was accessed by two different threads at the same time. \n  " +
                    $"The invalidated outcome was '{this}' and it was invalidated by '{triggeringFactor}'. ");
                //- If this Outcome is in the update list we should know it's a loop, if it's not then it should be
                //  another thread accessing it.
                //  Well actually, the parent won't add us to the list until this returns...
            }
            
            if (HasBeenTriggered is false)
            {
                HasBeenTriggered = true;
                InvalidateOutcome(triggeringFactor);
    
                if (IsNecessary)
                {
                    UpdateOutcome();
                }
                else if (DestabilizeSubscribers())
                {
                    numberOfNecessarySubscribers++;
                    UpdateOutcome();
                }
    
                return true;
            }
    
            return false;
        }

        protected abstract void InvalidateOutcome(IFactor changedParentState);
        
        protected virtual void UpdateOutcome() => UpdateList.Update(this);
        
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
                //  we're going to be triggered when our parent recalculates, or the parent won't
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
                    numberOfNecessarySubscribers++;
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
            var subscribersToDestabilize = subscribers;
            
            if (subscribersToDestabilize.Count > 0) 
            {
                foreach (var subscriberReference in subscribersToDestabilize)
                {
                    if (subscriberReference.TryGetTarget(out var subscriber))
                    {
                        if (subscriber.Destabilize())
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        
        public override bool Subscribe(IFactorSubscriber subscriberToAdd)
        {
            if(subscriberToAdd == null) { throw new ArgumentNullException(nameof(subscriberToAdd)); }
    
            if (IsReacting)
            {
                throw new InvalidOperationException("Recursive dependency"); //- TODO : Better error message.
            }

            if (base.Subscribe(subscriberToAdd))
            {
                if (HasBeenTriggered || IsUnstable)
                {
                    bool subscriberIsNecessary = subscriberToAdd.Destabilize();

                    if (subscriberIsNecessary)
                    {
                        AttemptReaction();
                        //- TODO : This seems like it could lead to some weird behavior 
                        //         when combined with the fact that the 'involved' process
                        //         already updates Reactors before adding subscribers.
                        //         Think about it for a bit when you get a chance.
                    }
                }
                
                return true;
            }
            else return false;
        }
        
        public override void NotifyNecessary()
        {
            bool wasAlreadyNecessary = IsNecessary;
    
            base.NotifyNecessary();

            if (wasAlreadyNecessary is false)
            {
                OnNecessary();
            }
        }
        
        public override void NotifyNotNecessary()
        {
            if (base.IsNecessary)
            {
                base.NotifyNotNecessary();
    
                if (IsNecessary is false)
                {
                    OnNotNecessary();
                }
            }
        }
        
        private void OnNecessary()
        {
            //- We don't propagate the Necessary status if we don't need to update, to avoid walking up the tree.
            //  All Factors walk down the tree to destabilize their dependents, so we can let them know then
            //  if need be.
            if (HasBeenTriggered || IsUnstable)
            {
                if (HasReacted)
                {
                    foreach (var trigger in Triggers)
                    {
                        trigger.NotifyNecessary();
                    }
                }
            }

            AttemptReaction();
            //- Should we check if we're already reacting?
        }
        
        private void OnNotNecessary()
        {
            if (HasReacted)
            {
                foreach (var trigger in Triggers)
                {
                    trigger.NotifyNotNecessary();
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

            //- TODO : We could probably just return "AttemptReaction() is false" if we can 
            //         guarantee that AttemptReaction() is always going to be based on
            //         HasBeenTriggered and IsUnstable anyways.
        }

        #endregion


        #region Constructors

        protected ReactorCore(string name) : base(name)
        {
            
        }

        #endregion

        
        #region Explicit Implementations

        void IUpdateable.Update() => AttemptReaction();

        #endregion
    }
}