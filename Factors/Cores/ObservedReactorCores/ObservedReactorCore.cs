using System;
using System.Collections.Generic;
using Core.Factors;
using Core.States;
using Factors.Observer;
using JetBrains.Annotations;
using static Core.Tools.Collections;


namespace Factors.Cores.ObservedReactorCores
{
    public abstract class ObservedReactorCore : ReactorCore, IObserved, IInvolved
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

        public override bool HasTriggers      => influences.Length > 0;
        public override int  NumberOfTriggers => nextOpenInfluenceIndex;
        public override int  Priority         => priority;

        protected override IEnumerable<IFactor> Triggers
        {
            get
            {
                var currentInfluences  = influences;
                int lastInfluenceIndex = nextOpenInfluenceIndex - 1;
            
                for (int i = 0; i < lastInfluenceIndex; i++)
                {
                    yield return currentInfluences[i];
                }
            }
        }

        #endregion
        

        #region Instance Methods

        protected override void InvalidateOutcome(IFactor factorToSkip) => RemoveInfluences(factorToSkip);

        protected void RemoveInfluences(IFactor factorToSkip)
        {
            var formerInfluences   = influences;
            var lastInfluenceIndex = nextOpenInfluenceIndex - 1;
            
            for (int i = 0; i <= lastInfluenceIndex; i++)
            {
                ref IFactor currentInfluence = ref formerInfluences[i];
    
                if (currentInfluence != factorToSkip)
                {
                    currentInfluence.Unsubscribe(this);
                }
                
                currentInfluence = null;
            }
    
            nextOpenInfluenceIndex = 0;
            
            //- Note : We could choose to keep the influences until we recalculate, and then compare them with 
            //         the influences that are added during the update process.  
        }
        
        public void NotifyInvolved()
        {
            Observer.NotifyInvolved(this);
            
            //- We could test for recursion here. If this Outcome is updating, then either it's accessing
            //  itself in its update method, or something it affected during this update is.  We could ask
            //  the Observer what Outcome is actively updating, and if it's not us then it has to be one we
            //  accessed during this update, which means it's one we depend on, which means something we
            //  depend on depends on us, which means there's a loop.
        }

        public void NotifyChanged()
        {
            Observer.NotifyChanged(this);
            //- TODO : Decide if we need something here.
        }

        #endregion


        #region Constructors

        protected ObservedReactorCore(string name) : base(name)
        {
            removeSubscriptionWhenTriggered = true;
        }

        #endregion
        

        #region Explicit Implementations

        void IObserved.Notify_InfluencedBy(IFactor influentialFactor)
        {
            if (influentialFactor is null) { throw new ArgumentNullException(nameof(influentialFactor)); }

            if (HasBeenTriggered is false)
            {
                if (influentialFactor.Subscribe(this))
                {
                    //- We expect a Factor to only add us as a dependent if they didn't already have us as a dependent. 
                    Add(ref influences, influentialFactor, nextOpenInfluenceIndex);
                    nextOpenInfluenceIndex++;

                    if (influentialFactor.Priority >= this.Priority)
                    {
                        this.priority = influentialFactor.Priority + 1;
                    }
                }
            }
            
            //- Could we just change the process for this to something where the Reactor sets itself as the active
            //  outcome and then if a Factor is used it contacts the active outcome and proposes adding it as a subscriber.
            //  This is pretty similar to the current setup except the publisher is reaching out to subscriber,
            //  instead of the observer notifying the subscriber it was affected by something.
        }

        #endregion
    }
}