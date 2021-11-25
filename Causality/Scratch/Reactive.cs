using System;
using System.Diagnostics;
using Causality.States;
using Core.States;
using JetBrains.Annotations;

namespace Causality.Scratch
{
    public abstract class Reactorio : Factorio, IResult, IUpdateable
    {
        #region Instance Fields
        
        private bool    isReflexive;
        private Outcome outcome;
        
        #endregion
    
        
        #region Instance Properties
        
        public          bool IsUpdating         { get; protected set; }
        public          bool IsStable           { get; protected set; }
        public          bool IsValid            { get; protected set; }
        public          bool IsStablizing       { get; protected set; }
        public          bool IsBeingInfluenced  => outcome.IsBeingInfluenced;
        public          int  NumberOfInfluences => outcome.NumberOfInfluences;
        public override int  Priority           => outcome.Priority;
        public          bool IsUnstable         => IsStable is false;
        public          bool IsInvalid          => IsValid  is false;
    //  public          bool AllowRecursion     { get; set; }
    
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
                        
                        if (IsNecessary is false) //- If it's true then we've already told our influences that they are Necessary.
                        {
                            outcome.NotifyNecessary();
                        }
                        
                        Stabilize();
                    }
                }
                else if (isReflexive is true)
                {
                    isReflexive = false;
    
                    if (IsNecessary is false)  //<- If it's true then we still need our influences to be Necessary.
                    {
                        outcome.NotifyNotNecessary();
                    }
                }
            }
        }
        
        //- What happens if someone sets RecalculatesAutomatically to true inside an update method, on an Outcome that 
        //  has been invalidated during that same update process?
    
        //- bool AllowDependencies :  This may be good for Action based Outcomes that want to track their dependencies,
        //                            but aren't intended to have their own dependents, like a Reaction that just prints
        //                            a value to the Console.  Alternatively, add a mechanic that allows something to
        //                            subscribe to a Reactive it accesses, even if the Reactive is hidden below the surface
        //                            i.e. being able to subscribe to a property that only exposes the value of the Reactive. 
        //
    
        #endregion
    
    
        #region Instance Methods
    
        public bool React()
        {
            bool outcomeChanged;
    
            if (IsUpdating) { Debug.Fail($"Update loop in {nameof(Response)} => {this}."); }
    
            IsUpdating   = true;
            IsStable     = true;
            IsValid      = true;
            IsStablizing = false;
    
            try
            {
                outcomeChanged = ExecuteProcess();
            }
            catch (Exception e)
            {
                //InvalidateOutcome(null);
                
                //- TODO : Consider having Outcomes store exceptions as an accessible field,
                //         similar to some of the reactives available in other libraries.
                throw;
            }
            finally
            {
                IsUpdating = false;
            }
            
            if (outcomeChanged)
            {
                InvalidateDependents();
    
                return true;
            }
            else
            {
                return false;
            }
        }
    
        protected abstract bool ExecuteProcess();
        
        public bool Invalidate(IInfluence changedParentState)
        {
            if (IsUpdating)
            {
                throw new InvalidOperationException(
                    "An Outcome was invalidated while it was updating, meaning either the Outcome's update process " +
                    "caused it to invalidated itself creating an update loop, " +
                    "or the Outcome was accessed by two different threads at the same time. \n  " +
                   $"The invalidated outcome was '{this}' and it was invalidated by '{changedParentState}'. ");
                //- If this Outcome is in the update list we should know it's a loop, if it's not then it should be
                //  another thread accessing it.  Well actually, the parent won't add us to the list until this returns...
            }
    
            return InvalidateOutcome(changedParentState);
        }
        
        protected bool InvalidateOutcome(IInfluence changedParentState)
        {
            if (IsValid)
            {
                IsValid = false;
                outcome.Invalidate(changedParentState);
    
                if (IsNecessary || IsReflexive)
                {
                    UpdateList.Update(this);
                }
                else if (DestabilizeDependents())
                {
                    numberOfNecessaryDependents++;
                    UpdateList.Update(this);
                }
    
                return true;
            }
    
            return false;
        }
        
        public virtual bool Invalidate() => Invalidate(null);

        public bool Stabilize()
        {
            if (IsInvalid)  //- Invalid states no longer have Influences.
            {
                return React() is false;
            }
            else if (IsUnstable)
            {
                return StabilizeOutcome();
            }
            else return true;
        }
        
        protected bool StabilizeOutcome()
        {
            IsStablizing = true;
                
            if (outcome.TryStabilize())
            {
                IsStablizing = false;
                IsStable     = true;
                Debug.Assert(IsValid);
                
                return true;
            }
            else
            {
                return React() is false;
            }
        }
        
        //- Does not imply the caller will queue this Outcome to be updated.
        //  Only that the caller should be notified if this Outcome is Necessary
        //  and if not that it should mark itself and its dependents as Unstable
        public bool Destabilize()
        {
            if (IsStablizing)
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
            else if (IsInvalid)
            {
                return false;
            }
            else if (IsStable)  //- Can we be Stable and Invalid?
            {
                bool hasNecessaryDependents = DestabilizeDependents();
                
                if (hasNecessaryDependents)
                {
                    numberOfNecessaryDependents++;
                    return true;
                }
                else
                {
                    IsStable = false;
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

        public override bool AddDependent(IDependency dependentToAdd)
        {
            if(dependentToAdd == null) { throw new ArgumentNullException(nameof(dependentToAdd)); }
    
            if (IsUpdating)
            {
                throw new InvalidOperationException(""); //- Recursive dependency
            }
            
            return base.AddDependent(dependentToAdd);
        }
        
        //- We may have to keep track of how many Necessary children an Outcome has, so that if one notifies us
        // it's no longer Necessary we know if we are still Necessary or not.  We could iterate through our
        // dependents, but that seems cumbersome.
        public override void NotifyNecessary()
        {
            bool wasAlreadyNecessary = IsNecessary;
    
            base.NotifyNecessary();
    
            if (wasAlreadyNecessary  ||  IsReflexive  ||  (IsValid && IsStable))  
            {
                return;
                //- We don't propagate the Necessary status if we don't need to update, to avoid walking the tree.
            }
            else //- We know it's invalid or unstable
            {
                outcome.NotifyNecessary(); //- Let our influences stabilize before we do.
                Stabilize();
            }
        }
        
        public override void NotifyNotNecessary()
        {
            base.NotifyNotNecessary();
    
            if (numberOfNecessaryDependents <= 0 && 
                IsReflexive is false)
            {
                outcome.NotifyNotNecessary();
            }
        }
    
        protected void OnInvolved()
        {
            Stabilize();
            NotifyInvolved();
            
            //- If this Outcome is updating, then either it's accessing itself in its update method, or
            //  something it affected during this update is.  We could ask the Observer what Outcome is
            //  actively updating, and if it's not us then it has to be one we accessed during this update,
            //  which means it's one we depend on, which means something we depend on depends on us, which
            //  means there's a loop.
        }
        
        public override bool Reconcile() => Stabilize();

        #endregion
        
    
        #region Constructors
    
        protected Reactorio() : base()
        {
            IsValid = false;
        }
    
        #endregion
        
    
        #region Explicit Implementations
    
        bool IUpdateable.Update() => Stabilize();
    
        #endregion
    }

}
    
