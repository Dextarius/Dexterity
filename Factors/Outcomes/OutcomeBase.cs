using System;
using System.Diagnostics;
using Core.Factors;
using Core.States;
using Factors.Outcomes.Influences;

namespace Factors.Outcomes
{
    public abstract class OutcomeBase : Influence, IOutcome, IUpdateable
    {
        #region Instance Fields

        protected WeakReference<IDependent> weakReferenceToSelf;
        private   bool                       isReflexive;

        #endregion


        #region Instance Properties

        public          bool       IsUpdating         { get; protected set; }
        public          bool       IsStable           { get; protected set; }
        public          bool       IsValid            { get; protected set; }
        public          bool       IsStabilizing      { get; protected set; }
        public abstract bool       IsBeingInfluenced  { get; }
        public abstract int        NumberOfInfluences { get; }
        public          bool       IsUnstable         => IsStable is false;
        public          bool       IsInvalid          => IsValid is false;

        public WeakReference<IDependent> WeakReference => weakReferenceToSelf ??= new WeakReference<IDependent>(this);
        
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
                        
                        Stabilize();
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

        public bool Generate()
        {
            bool outcomeChanged;
    
            if (IsUpdating) { Debug.Fail($"Update loop in Outcome => {this}."); }
    
            IsUpdating    = true;
            IsStable      = true;
            IsValid       = true;
            IsStabilizing = false;
    
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
        
        protected abstract bool GenerateOutcome();

        public bool Stabilize()
        {
            if (IsInvalid)
            {
                return Generate() is false;
            }
            else if (IsUnstable)
            {
                IsStabilizing = true;
                
                if (TryStabilizeOutcome())
                {
                    IsStabilizing = false;
                    IsStable      = true;
                    
                    Debug.Assert(IsValid);
                
                    return true;
                }
                else
                {
                    return Generate() is false;
                }
            }
            else return true;
        }
        protected abstract bool TryStabilizeOutcome();

        public bool Invalidate() => Invalidate(null);

        public bool Invalidate(IFactor changedFactor)
        {
            if (IsUpdating)
            {
                throw new InvalidOperationException(
                    "An Outcome was invalidated while it was updating, meaning either the Outcome's update process " +
                    "caused it to invalidate itself, creating an update loop, " +
                    "or the Outcome was accessed by two different threads at the same time. \n  " +
                    $"The invalidated outcome was '{this}' and it was invalidated by '{changedFactor}'. ");
                //- If this Outcome is in the update list we should know it's a loop, if it's not then it should be
                //  another thread accessing it.
                //  Well actually, the parent won't add us to the list until this returns...
            }
            
            if (IsValid)
            {
                IsValid = false;
                InvalidateOutcome(changedFactor);
    
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
            else if (IsInvalid)
            {
                return false;
            }
            else if (IsStable) //- Can we be Stable and Invalid?
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
        
        public override bool AddDependent(IDependent dependentToAdd)
        {
            if(dependentToAdd == null) { throw new ArgumentNullException(nameof(dependentToAdd)); }
    
            if (IsUpdating)
            {
                throw new InvalidOperationException(""); //- Recursive dependency
            }
            
            return base.AddDependent(dependentToAdd);
        }
        
        public override void NotifyNecessary()
        {
            bool wasAlreadyNecessary = IsNecessary;
    
            base.NotifyNecessary();
    
            if (wasAlreadyNecessary  ||  IsReflexive  ||  (IsValid && IsStable))  
            {
                return;
                //- We don't propagate the Necessary status if we don't need to update, to avoid walking up the tree.
            }
            else //- We know we're either invalid or unstable
            {
                OnNecessary(); //- Let our influences stabilize before we do.
                Stabilize();
            }
        }

        public override void NotifyNotNecessary()
        {
            if (IsNecessary)
            {
                base.NotifyNotNecessary();
    
                if (HasDependents is false && 
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

        protected OutcomeBase(string name) : base(name)
        {
        }

        #endregion

        
        #region Explicit Implementations

        void IUpdateable.Update() => Stabilize();

        #endregion
    }
}