using System;
using System.Diagnostics;
using Causality.States;
using Core.States;
using JetBrains.Annotations;
using static Core.Tools.Collections;

namespace Causality.Scratch
{
    public class Outcome : IInfluenceable
    {
        #region Static Fields
    
        protected static readonly IInfluence[] defaultInfluences = Array.Empty<IInfluence>();
    
        #endregion
    
    
        #region Instance Fields
    
        [NotNull] 
        protected IInfluence[] influences = defaultInfluences;
        protected IDependency  owner;
        private   int          nextOpenInfluenceIndex;
        protected int          priority;
        private   bool         isValid;

        #endregion

        
        #region Properties

        public bool IsExecuting        { get; protected set; }
        public bool IsBeingInfluenced  => influences.Length > 0;
        public int  NumberOfInfluences => nextOpenInfluenceIndex;
        public int  Priority           => priority;

        #endregion

        public void NotifyNecessary()
        {
            var currentInfluences = influences;
            
            for (int i = 0; i < currentInfluences.Length; i++)
            {
                currentInfluences[i].NotifyNecessary();
            }
        }
        
        public void NotifyNotNecessary()
        {
            var currentInfluences = influences;
            
            for (int i = 0; i < currentInfluences.Length; i++)
            {
                currentInfluences[i].NotifyNotNecessary();
            }
        }

        public bool TryStabilize()
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

        public void Invalidate(IInfluence stateToSkip)
        {
            if (isValid)
            {
                isValid = false;
                RemoveInfluences(stateToSkip);
            }
        }

        protected void RemoveInfluences(IInfluence stateToSkip)
        {
            var formerInfluences   = influences;
            var lastInfluenceIndex = nextOpenInfluenceIndex - 1;
            
            for (int i = 0; i <= lastInfluenceIndex; i++)
            {
                ref IInfluence currentInfluence = ref formerInfluences[i];
    
                if (currentInfluence != stateToSkip)
                {
                    currentInfluence.ReleaseDependent(owner);
                }
                
                currentInfluence = null;
            }
    
            nextOpenInfluenceIndex = 0;
            //- Note : We could choose to keep the influences until we recalculate, and then compare them with 
            //         the influences that are added during the update process.  
        }

        void IInfluenceable.Notify_InfluencedBy(IInfluence influence)
        {
            if (influence is null) { throw new ArgumentNullException(nameof(influence)); }
            
            if (IsExecuting)
            {
                if (isValid) 
                {
                    if (influence.AddDependent(owner))
                    {
                        //- We expect a State to add us as a dependent, only if they don't already have us as a dependent. 
                        Add(ref influences, influence, nextOpenInfluenceIndex);
                        nextOpenInfluenceIndex++;
    
                        if (influence.Priority >= this.Priority)
                        {
                            this.priority = influence.Priority + 1;
                        }
                    }
                }
            }
            else
            {
                #if AllowAddingInfluencesOutsideOfUpdate == false
                
                throw new InvalidOperationException("Influences can only be added while an Outcome is updating. ");
    
                #endif
            }
        }
    }

}