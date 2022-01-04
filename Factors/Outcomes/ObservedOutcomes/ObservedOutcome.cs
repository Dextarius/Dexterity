using System;
using Core.Factors;
using Core.States;
using Factors.Observer;
using JetBrains.Annotations;
using static Core.Tools.Collections;


namespace Factors.Outcomes.ObservedOutcomes
{
    public abstract class ObservedOutcome : OutcomeBase, IObserved, IInvolved
    {
        #region Static Fields
    
        protected static readonly IFactor[] defaultInfluences = Array.Empty<IFactor>();

        #endregion

        
        #region Instance Fields

        [NotNull] 
        protected IFactor[] influences = defaultInfluences;
        protected int       priority;
        protected int       nextOpenInfluenceIndex;

        #endregion
        
        
        #region Static Properties

        protected static CausalObserver Observer => CausalObserver.ForThread;

        #endregion
        
        
        #region Properties

        public override bool IsBeingInfluenced  => influences.Length > 0;
        public override int  NumberOfInfluences => nextOpenInfluenceIndex;
        public override int  Priority           => priority;

        //- bool AllowDependencies :  This could be good for Action based Outcomes that want to track their dependencies,
        //                            but aren't intended to have their own dependents, like a Reaction that just prints
        //                            a value to the Console.  Alternatively, add a mechanic that allows something to
        //                            subscribe to a Reactive it accesses, even if the Reactive is hidden below the surface
        //                            i.e. being able to subscribe to a property that only exposes the value of the Reactive. 
        //
        
        #endregion
        

        #region Instance Methods

        protected override void OnNecessary()
        {
            var currentInfluences = influences;
            
            for (int i = 0; i < currentInfluences.Length; i++)
            {
                currentInfluences[i].NotifyNecessary();
            }
        }

        protected override void OnNotNecessary()
        {
            var currentInfluences = influences;
            
            for (int i = 0; i < currentInfluences.Length; i++)
            {
                currentInfluences[i].NotifyNotNecessary();
            }
        }

        protected override bool TryStabilizeOutcome()
        {
            var formerInfluences = influences; 
            //^ Stabilizing any of these could result in it invalidating this Outcome,
            //  and if it does invalidate us then it will cause us to remove our influences.
                
            for (int i = 0; i < formerInfluences.Length; i++)
            {
                var currentInfluence = formerInfluences[i];
                
                if (currentInfluence.Reconcile() is false)
                {
                    return false;
                    //- No point in stabilizing the rest.  We try to stabilize to avoid recalculating, and if one
                    //  of our influences fails to stabilize then we have no choice but to recalculate.  In addition
                    //  we don't even know if we'll use the same influences when we recalculate. If we do end
                    //  up accessing those same influences, they'll try to stabilize themselves when we access them anyways.
                }
            }

            return true;
        }
        
        protected override void InvalidateOutcome(IFactor stateToSkip)
        {
            RemoveInfluences(stateToSkip);
        }
        

        protected void RemoveInfluences(IFactor stateToSkip)
        {
            var formerInfluences   = influences;
            var lastInfluenceIndex = nextOpenInfluenceIndex - 1;
            
            for (int i = 0; i <= lastInfluenceIndex; i++)
            {
                ref IFactor currentDeterminant = ref formerInfluences[i];
    
                if (currentDeterminant != stateToSkip)
                {
                    currentDeterminant.RemoveDependent(this);
                }
                
                currentDeterminant = null;
            }
    
            nextOpenInfluenceIndex = 0;
            //- Note : We could choose to keep the influences until we recalculate, and then compare them with 
            //         the influences that are added during the update process.  
        }
        
        public void NotifyInvolved()
        {
            Observer.NotifyInvolved(this);
            
            //- If this Outcome is updating, then either it's accessing itself in its update method, or
            //  something it affected during this update is.  We could ask the Observer what Outcome is
            //  actively updating, and if it's not us then it has to be one we accessed during this update,
            //  which means it's one we depend on, which means something we depend on depends on us, which
            //  means there's a loop.
        }

        public void NotifyChanged()
        {
            //- TODO : Decide if we need something here.
        }

        #endregion


        #region Constructors

        public ObservedOutcome(string name) : base(name)
        {

        }

        #endregion
        

        #region Explicit Implementations

        void IObserved.Notify_InfluencedBy(IFactor determinant)
        {
            if (determinant is null) { throw new ArgumentNullException(nameof(determinant)); }

            if (IsValid)
            {
                if (determinant.AddDependent(this))
                {
                    //- We expect a Factor to only add us as a dependent if they don't already have us as a dependent. 
                    Add(ref influences, determinant, nextOpenInfluenceIndex);
                    nextOpenInfluenceIndex++;

                    if (determinant.Priority >= this.Priority)
                    {
                        this.priority = determinant.Priority + 1;
                    }
                }
            }
        }

        #endregion
    }
}