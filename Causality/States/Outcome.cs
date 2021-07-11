using System;
using System.Diagnostics;
using System.Threading;
using Core.Causality;
using Core.Factors;
using JetBrains.Annotations;
using static Core.InterlockedUtils;

namespace Causality.States
{
    public class Outcome : State, IOutcome
    {
        #region Static Fields

        protected static readonly WeakReference<INotifiable> invalidReference      = new WeakReference<INotifiable>(null);
        protected static readonly IState[]                   invalidatedInfluences = new IState[0];

        #endregion
        
        
        #region Instance Fields

        [NotNull] 
        protected IState[]                   influences = Array.Empty<IState>();
        protected WeakReference<INotifiable> callbackReference;

        #endregion


        #region Instance Properties
        
        public bool IsBeingAffected => influences.Length > 0;
        public bool HasCallback     => callbackReference != null; 

        #endregion


        #region Instance Methods

        public override bool Invalidate() => Invalidate(null);

        public bool Invalidate(IState invalidState)
        {
            if (base.Invalidate())
            {
                RemoveInfluences(invalidState);

                var referenceToNotify = Interlocked.Exchange(ref callbackReference, invalidReference);
                
                if ((referenceToNotify != null)  &&  referenceToNotify.TryGetTarget(out var objectToNotify))
                {
                    Debug.Assert(referenceToNotify != invalidReference, "");
                    UpdateHandler.RequestUpdate(objectToNotify.Notify);
                }
                
                return true;
            }
            else
            {
                return false;
            }
        }

        protected void RemoveInfluences(IState stateToSkip) => ReplaceInfluences(stateToSkip, invalidatedInfluences);

        
        protected void ReplaceInfluences(IState stateToSkip, IState[] newInfluences)
        {
            if (newInfluences == null) { throw new ArgumentNullException(nameof(newInfluences)); }
            
            IState[] formerInfluences   = influences;
            IState[] influencesToRemove = null;
            bool     handled            = false;

            while (handled == false)
            {
                if (formerInfluences != invalidatedInfluences)
                {
                    if (TryCompareExchangeOrSet(ref influences, newInfluences, ref formerInfluences))
                    {
                        influencesToRemove = formerInfluences;
                        handled = true;
                    }
                }
                else
                {
                    influencesToRemove = newInfluences;
                    handled = true;
                }
            }

            if (influencesToRemove != null)
            {
                for (int i = 0; i < formerInfluences.Length; i++)
                {
                    IState currentFactor = formerInfluences[i];

                    if (currentFactor != stateToSkip)
                    {
                        formerInfluences[i]?.ReleaseDependent(this);
                    }
                }
            }
        }

        public void SetInfluences(IState[] newInfluences)
        {
            if (newInfluences == null) { throw new ArgumentNullException(nameof(newInfluences)); }

            ReplaceInfluences(null, newInfluences);
        }

        public void SetCallback(INotifiable objectToNotify)
        {
            if (objectToNotify == null) { throw new ArgumentNullException(nameof(objectToNotify)); }

            var formerReference = callbackReference;
            var newReference    = objectToNotify != null? new WeakReference<INotifiable>(objectToNotify) :
                                                          null;
            while (true)
            {
                if (formerReference != invalidReference)
                {
                    if (TryCompareExchangeOrSet(ref callbackReference, newReference, ref formerReference))
                    {
                        return;
                    }
                }
                else
                {
                    UpdateHandler.RequestUpdate(objectToNotify.Notify);
                    return;
                }
            }
        }
        
        public void DisableCallback()
        {
            var formerReference = callbackReference;

            while (IsValid && 
                   formerReference != null && 
                   formerReference != invalidReference)
            {
                if (TryCompareExchangeOrSet(ref callbackReference, null, ref formerReference))
                {
                    return;
                }
            }
        }

        #endregion


        #region Constructors
        
        public Outcome(INotifiable objectToNotify) 
        {
            callbackReference = objectToNotify != null?  new WeakReference<INotifiable>(objectToNotify) :
                                                         default;
        }

        public Outcome() : this(null)
        {
        }
        
        #endregion
    }
}