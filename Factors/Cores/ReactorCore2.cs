using System;
using System.Collections.Generic;
using System.Diagnostics;
using Core;
using Core.Factors;
using Core.States;
using Factors.Cores;
using static Core.Tools.Types;
using static Core.Settings;


namespace Factors
{
    public abstract class ReactorCore2 : FactorCore2, IFactorSubscriber, IReactorCore2
    {
        #region Instance Fields

        protected WeakReference<IReactorCoreOwner> weakReferenceToOwner;
        protected IReactorCoreOwner                referenceToOwner;
        protected bool                             unsubscribeWhenTriggered;

        #endregion


        #region Instance Properties

        public             bool                 IsReacting           { get; protected set; }
        public             bool                 IsStabilizing        { get; protected set; }
        public             bool                 IsUnstable           { get; protected set; } = true;
        public             bool                 IsNecessary          { get; protected set; }
        protected abstract IEnumerable<IFactor> Triggers             { get; }
        public    abstract bool                 HasTriggers          { get; }
        public    abstract int                  NumberOfTriggers     { get; }
        public             bool                 HasBeenTriggered     { get; protected set; } = true;
        public abstract    int                  UpdatePriority       { get; }
        public             bool                 HasReacted           => VersionNumber > 0;
        //^ This wont work if the outcome value is the same as the type's default value
        
        protected IReactorCoreOwner Owner
        {
            get
            {
                if (referenceToOwner != null)
                {
                    return referenceToOwner;
                }
                else
                {
                    weakReferenceToOwner.TryGetTarget(out var owner);
                    
                    return owner;
                }
            }
        }
        
        #endregion

        
        #region Instance Methods

        public bool GenerateOutcome()
        {
            if (IsReacting)                { Debug.Fail($"Update loop in {NameOf<ReactorCore>()} => {this}."); }
            if (HasBeenTriggered is false) { InvalidateOutcome(null); }
            
            bool outcomeChanged;
            
            IsReacting       = true;
            IsUnstable       = false;
            HasBeenTriggered = false;
            
            try
            {
                outcomeChanged = CreateOutcome();
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
                VersionNumber++;
                return true;
            }
            else return false;
        }

        protected abstract void InvalidateOutcome(IFactor changedParentState);
        protected abstract bool CreateOutcome();
        
        public bool TryStabilizeOutcome()
        {
            bool successfullyReconciled = true;
            
            IsStabilizing = true;

            foreach (var determinant in Triggers)
            {
                if (determinant.Reconcile() is false)
                {
                    successfullyReconciled = false;
                    Debug.Assert(HasBeenTriggered);
                    //^ If the reconciliation failed then the trigger should have reacted and then
                    //  triggered us since we're one of its subscribers.

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
            removeSubscription = unsubscribeWhenTriggered;
            
            if (IsReacting)
            {
                Logging.Notify_ReactorTriggeredWhileUpdating(this, triggeringFactor);

                //- If this Outcome is in the update list we should know it's a loop, if it's not then it should be
                //  another thread accessing it.
                //  Well actually, the parent won't add us to the list until this returns...
            }
            
            if (HasBeenTriggered is false)
            {
                HasBeenTriggered = true;
                InvalidateOutcome(triggeringFactor);
                Owner?.CoreTriggered();
                
                return true;
            }
    
            return false;
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
                if (NotifyOwner_CoreDestabilized())
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
        
        protected bool NotifyOwner_CoreDestabilized() => Owner?.CoreDestabilized() ?? false;
        
        public virtual void OnNecessary()
        {
            IsNecessary = true;
            
            //- We don't propagate the Necessary status to our triggers, to avoid walking up the tree.
            //  All Reactors walk down the tree to destabilize their dependents, so we can let them know then
            //  if need be.
        }
        
        public virtual void OnNotNecessary()
        {
            IsNecessary = false;

            if (HasReacted)
            {
                foreach (var trigger in Triggers)
                {
                    trigger.NotifyNotNecessary(this);
                }
            }
            
            //- Note : This may not work as intended if Observed cores continue to pass themselves as triggers,
            //         because the core won't be able to tell its owner when its dependents notify it it's not 
            //         necessary.
        }

        #endregion


        #region Constructors

        protected ReactorCore2() : base()
        {
            
        }

        #endregion
    }
}