using System;
using System.Collections.Generic;
using Core.Factors;
using Core.States;
using Dextarius.Collections;
using Factors.Observer;
using JetBrains.Annotations;


namespace Factors.Cores.ObservedReactorCores
{
    public abstract class ObservedReactorCore : ReactorCore, IObserved, IInvolved
    {
        #region Instance Fields

        [NotNull] 
        protected readonly Dict<IFactor, bool> triggersByInUse = new Dict<IFactor, bool>();
        protected          int                 priority;

        #endregion
        
        
        #region Static Properties

        protected static CausalObserver Observer => CausalObserver.ForThread;

        #endregion
        
        
        #region Properties

        public override bool HasTriggers      => triggersByInUse.Count > 0;
        public override int  NumberOfTriggers => triggersByInUse.Count;
        public override int  UpdatePriority   => priority;

        protected override IEnumerable<IFactor> Triggers => triggersByInUse.Keys;

        #endregion
        

        #region Instance Methods

        protected override void InvalidateOutcome(IFactor factorToSkip) => RemoveTriggers(factorToSkip);

        protected void RemoveTriggers(IFactor factorToSkip)
        {
            triggersByInUse.SetAllValuesTo(false);
        }
        
        public void NotifyInvolved()
        {
            Observer.NotifyInvolved(Owner);
            
            //- We could test for recursion here. If this Outcome is updating, then either it's accessing
            //  itself in its update method, or something it affected during this update is.  We could ask
            //  the Observer what Outcome is actively updating, and if it's not us then it has to be one we
            //  accessed during this update, which means it's one we depend on, which means something we
            //  depend on depends on us, which means there's a loop.
        }

        public void NotifyChanged()
        {
            Observer.NotifyChanged(Owner);
            //- TODO : Decide if we need something here.
        }

        protected void RemoveUnusedTriggers()
        {
            foreach (var keyValuePair in triggersByInUse.RemoveWhereValueEquals(false))
            {
                RemoveTrigger(keyValuePair.Key);
            }
        }

        #endregion


        #region Constructors

        protected ObservedReactorCore() : base()
        {
            
        }

        #endregion
        

        #region Explicit Implementations

        void IObserved.Notify_InfluencedBy(IFactor influentialFactor)
        {
            if (influentialFactor is null) { throw new ArgumentNullException(nameof(influentialFactor)); }

            if (HasBeenTriggered is false)
            {
                if (AddTrigger(influentialFactor, IsNecessary))
                {
                    //- If an Observed Reactor is necessary, but not Reflexive, it won't mark themselves as necessary
                    //  when it subscribes here.  This is because if we're subscribing through this method then
                    //  we're in the process of reacting, and as soon as we finish we're likely to trigger
                    //  our own subscribers.  If the subscribers that made us necessary are all Observed Reactors
                    //  like this one is, then they'll end up unsubscribing anyways when we trigger them, causing us
                    //  to no longer be necessary and we'll have to go back and notify all of the triggers that we just
                    //  told we were necessary, to mark us as not necessary. If we react and don't end up triggering our
                    //  subscribers because the outcome doesn't change, our triggers will just find out we're necessary
                    //  when they destabilize us anyways.

                    //- Factors are expected to only add us as a dependent if they didn't already have us as a dependent, so
                    //  so if we're here we should be good to assume we haven't added them as a trigger yet. 
                    
                    triggersByInUse[influentialFactor] = true;
                    
                    if (influentialFactor.UpdatePriority >= this.UpdatePriority)
                    {
                        this.priority = influentialFactor.UpdatePriority + 1;
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