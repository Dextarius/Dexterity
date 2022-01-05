using System;
using System.Diagnostics;
using Core.Factors;
using Core.States;
using Factors.Outcomes.Influences;

namespace Factors.Outcomes
{
    public abstract class ReactorCore : FactorCore, IReactor, IUpdateable
    {
        #region Instance Fields

        protected WeakReference<IFactorSubscriber> weakReferenceToSelf;
        private   bool                             isReflexive;

        #endregion


        #region Instance Properties

        public          bool IsReacting       { get; protected set; }
        public          bool IsStabilizing    { get; protected set; }
        public abstract bool HasTriggers      { get; }
        public abstract int  NumberOfTriggers { get; }
        public          bool IsUnstable       { get; protected set; }
        public          bool HasBeenTriggered { get; protected set; }

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
                        
                        if (IsNecessary is false) //<- If it's true we've already told our influences that they are Necessary.
                        {
                            OnNecessary();
                        }
                        
                        AttemptReaction();
                    }
                }
                else if (isReflexive is true)
                {
                    isReflexive = false;
    
                    if (IsNecessary is false) //<- If it's true then we still need our influences to be Necessary.
                    {
                        OnNotNecessary();
                    }
                }
            }
        }
        //^ What happens if someone sets RecalculatesAutomatically to true inside an update method, on a Reactor that 
        //  has been invalidated during that same update process?
        
        #endregion

        
        #region Instance Methods

        public bool ForceReaction() => React();

        protected bool React()
        {
            bool outcomeChanged;
    
            if (IsReacting) { Debug.Fail($"Update loop in Outcome => {this}."); }
    
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
                //- TODO : Consider having Outcomes store exceptions as an accessible field,
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
    
                return true;
            }
            else
            {
                return false;
            }
        }
        
        protected abstract bool GenerateOutcome();

        public bool AttemptReaction()
        {
            if (HasBeenTriggered)
            {
                return ForceReaction() is false;
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
                    return ForceReaction() is false;
                }
            }
            else return true;
        }
        
        protected abstract bool TryStabilizeOutcome();

        public bool Trigger() => Trigger(null);

        public bool Trigger(IFactor triggeringFactor)
        {
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
    
                if (IsNecessary || IsReflexive)
                {
                    UpdateOutcome();
                }
                else if (DestabilizeDependents())
                {
                    numberOfNecessaryDependents++;
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
            else if (IsNecessary || IsReflexive)
            {
                return true;
                
                //- We shouldn't need to mark ourselves as Unstable. If we're Necessary then either
                //  we're going to be  invalidated when our parent recalculates, or the parent won't
                //  change, in which case we aren't Unstable.
            }
            else if (HasBeenTriggered)
            {
                return false;
            }
            else if (IsUnstable is false) 
            {
                bool hasNecessaryDependents = DestabilizeDependents();
                
                if (hasNecessaryDependents)
                {
                    numberOfNecessaryDependents++;
                    return true;
                }
                else
                {
                    IsUnstable = true;
                }
            }
    
            return false;
        }
        
        protected bool DestabilizeDependents()
        {
            var formerDependents = affectedResults;
            
            if (formerDependents.Count > 0) 
            {
                foreach (var dependentReference in formerDependents)
                {
                    if (dependentReference.TryGetTarget(out var dependent))
                    {
                        if (dependent.Destabilize())
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
                throw new InvalidOperationException(""); //- Recursive dependency
            }
            
            return base.Subscribe(subscriberToAdd);
        }
        
        public override void NotifyNecessary()
        {
            bool wasAlreadyNecessary = IsNecessary;
    
            base.NotifyNecessary();
            
            if (wasAlreadyNecessary || 
                IsReflexive         ||  
               ((HasBeenTriggered || IsUnstable) is false))  
            {
                return;
                //- We don't propagate the Necessary status if we don't need to update, to avoid walking up the tree.
            }
            else //- We're either triggered or unstable
            {
                OnNecessary(); //- Let our influences stabilize before we do.
                AttemptReaction();
            }
        }

        public override void NotifyNotNecessary()
        {
            if (IsNecessary)
            {
                base.NotifyNotNecessary();
    
                if (HasSubscribers is false && 
                    IsReflexive   is false)
                {
                    OnNotNecessary();
                }
            }
        }

        protected abstract void OnNecessary();
        protected abstract void OnNotNecessary();

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