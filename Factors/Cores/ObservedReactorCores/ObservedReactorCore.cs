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
    
        protected static readonly IFactor[] defaultTriggers = Array.Empty<IFactor>();

        #endregion

        
        #region Instance Fields

        [NotNull] 
        protected IFactor[] triggers = defaultTriggers;
        protected int       priority;
        protected int       nextOpenTriggerIndex;

        #endregion
        
        
        #region Static Properties

        protected static CausalObserver Observer => CausalObserver.ForThread;

        #endregion
        
        
        #region Properties

        public override bool HasTriggers      => nextOpenTriggerIndex > 0;
        public override int  NumberOfTriggers => nextOpenTriggerIndex;
        public override int  Priority         => priority;

        protected override IEnumerable<IXXX> Triggers
        {
            get
            {
                var currentTriggers  = triggers;
                int lastTriggerIndex = nextOpenTriggerIndex - 1;
            
                for (int i = 0; i < lastTriggerIndex; i++)
                {
                    yield return currentTriggers[i];
                }
            }
        }

        #endregion
        

        #region Instance Methods

        protected override void InvalidateOutcome(IFactor factorToSkip) => RemoveTriggers(factorToSkip);

        protected void RemoveTriggers(IFactor factorToSkip)
        {
            var formerTriggers   = triggers;
            var lastTriggerIndex = nextOpenTriggerIndex - 1;
            
            for (int i = 0; i <= lastTriggerIndex; i++)
            {
                ref IFactor currentTrigger = ref formerTriggers[i];
    
                if (currentTrigger != factorToSkip)
                {
                    currentTrigger.Unsubscribe(this);
                }
                
                currentTrigger = null;
            }
    
            nextOpenTriggerIndex = 0;
            
            //- Note : We could choose to keep the triggers until we recalculate, and then compare them with 
            //         the triggers that are added during the update process.  
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
                if (influentialFactor.Subscribe(this, IsReflexive))
                {
                    //- Observed Reactors don't mark themselves as necessary when they subscribe here, if they
                    //  have necessary dependents, but are not Reflexive themselves.  This is because if they
                    //  are subscribing through this method then they are in the process of reacting, and as
                    //  soon as they finish they're likely to trigger their own subscribers.  If the necessary
                    //  subscribers we trigger are all Observed Reactors like this one is, then they'll end up
                    //  unsubscribing anyways, causing us to no longer be necessary and then have to propagate
                    //  that up the tree. If we react and don't end up triggering our subscribers because the
                    //  outcome doesn't change, our triggers will just find out we're necessary when they
                    //  destabilize us anyways.
                    
                    //- Factors are expected to only add us as a dependent if they didn't already have us as a dependent, so
                    //  so if we're here we should be good to assume we haven't added them as a trigger yet. 
                    Add(ref triggers, influentialFactor, nextOpenTriggerIndex);
                    nextOpenTriggerIndex++;

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