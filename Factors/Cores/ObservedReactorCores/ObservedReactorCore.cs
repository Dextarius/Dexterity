using System;
using System.Collections.Generic;
using Core.Factors;
using Core.States;
using Dextarius.Collections;
using Factors.Observer;
using JetBrains.Annotations;
using static Factors.Factor;

namespace Factors.Cores.ObservedReactorCores
{
    public abstract class ObservedReactorCore : ReactorCore, IObserved, IInvolved
    {
        #region Instance Fields

        [NotNull] 
        protected readonly Dict<IFactor, long> flagsByTrigger = new Dict<IFactor, long>();
        protected          int                 priority;

        #endregion
        
        
        #region Static Properties

        protected static CausalObserver Observer => CausalObserver.ForThread;

        #endregion
        
        
        #region Properties

        public override bool HasTriggers      => flagsByTrigger.Count > 0;
        public override int  NumberOfTriggers => flagsByTrigger.Count;
        public override int  UpdatePriority   => priority;

        protected override IEnumerable<IFactor> Triggers => flagsByTrigger.Keys;

        #endregion
        

        #region Instance Methods

        protected override void InvalidateOutcome(IFactor changedFactor) => RemoveTriggers();

        protected void RemoveTriggers()
        {
            flagsByTrigger.SetAllValuesTo(TriggerFlags.None);
        }
        
        
        public void NotifyInvolved(IFactor involvedFactor, long triggerFlags)
        {
            if (involvedFactor != null)
            {
                Observer.NotifyInvolved(involvedFactor, triggerFlags);
            }
            
            //- We could test for recursion here. If this Outcome is updating, then either it's accessing
            //  itself in its update method, or something it affected during this update is.  We could ask
            //  the Observer what Outcome is actively updating, and if it's not us then it has to be one we
            //  accessed during this update, which means it's one we depend on, which means something we
            //  depend on depends on us, which means there's a loop.
        }

        public void NotifyInvolved(long triggerFlags) => NotifyInvolved(Callback, triggerFlags);
        public void NotifyInvolved()                  => NotifyInvolved(TriggerFlags.Default);
        
        public void NotifyChanged(IFactor changedFactor)
        {
            Observer.NotifyChanged(changedFactor);
            //- TODO : Decide if we need something here.
        }

        protected void RemoveUnusedTriggers()
        {
            foreach (var keyValuePair in flagsByTrigger.RemoveWhereValueEquals(TriggerFlags.None))
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

        void IObserved.Notify_InfluencedBy(IFactor influentialFactor, long triggerFlags)
        {
            if (influentialFactor is null) { throw new ArgumentNullException(nameof(influentialFactor)); }
            
            if (IsTriggered is false)
            {
                AddTrigger(influentialFactor, IsReflexive, triggerFlags);

                if (flagsByTrigger.TryGetValue(influentialFactor, out var existingTriggerFlags))
                {
                    long flagsAlreadyPresent = triggerFlags & existingTriggerFlags;
                    
                    if (flagsAlreadyPresent != triggerFlags)
                    {
                        flagsByTrigger[influentialFactor] = triggerFlags | existingTriggerFlags;
                    }
                }
                //^ Maybe we should only call AddTrigger() if triggersByInUse doesn't contain influentialFactor.

                if (influentialFactor.UpdatePriority >= this.UpdatePriority)
                {
                    this.priority = influentialFactor.UpdatePriority + 1;
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