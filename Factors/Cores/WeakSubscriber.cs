﻿using System;
using System.Collections.Generic;
using Core.Factors;
using Core.States;

namespace Factors.Cores
{
    public class WeakSubscriber : IFactorSubscriber
    {
        #region Instance Fields

        private readonly IFactorSubscriber                subscriber;
        private          WeakReference<IFactorSubscriber> weakReferenceToSubscriber;

        #endregion

        
        #region Properties

        public bool IsNecessary      { get; private   set; }
        public bool IsUnstable       { get; protected set; }
        public bool HasBeenTriggered { get; protected set; } = true;

        #endregion


        #region Instance Methods

        public bool Trigger() => Trigger(null, out _);

        public bool Trigger(IFactor triggeringFactor, out bool removeSubscription)
        {
            if (subscriber != null)
            {
                return subscriber.Trigger(triggeringFactor, out removeSubscription);
            }
            else
            {
                if (weakReferenceToSubscriber.TryGetTarget(out var weakSubscriber))
                {
                    return weakSubscriber.Trigger(triggeringFactor, out removeSubscription);
                }
                else
                {
                    removeSubscription = true;
                    
                    return false;
                }
            }
        }

        public bool Destabilize()
        {
            if (IsNecessary)
            {
                return true;
            }
            else if (subscriber != null)
            {
                return subscriber.Destabilize();
            }
            else
            {
                if (weakReferenceToSubscriber.TryGetTarget(out var weakSubscriber))
                {
                    return weakSubscriber.Destabilize();
                }
                else
                {
                    return false;
                }
            }
        }

        #endregion

        public WeakSubscriber(IFactorSubscriber subscriberToCall, bool necessary = false)
        {
            weakReferenceToSubscriber = new WeakReference<IFactorSubscriber>(subscriberToCall);

            if (necessary)
            {
                subscriber = subscriberToCall;
            }
        }
    }



    public class Subscriber : IFactorSubscriber
    {
        #region Instance Fields

        private readonly IFactorSubscriber                subscriber;
        private          WeakReference<IFactorSubscriber> weakReferenceToSubscriber;

        #endregion

        
        #region Properties

        public bool IsNecessary      { get; private   set; }
        public bool IsUnstable       { get; protected set; }
        public bool HasBeenTriggered { get; protected set; } = true;

        #endregion


        #region Instance Methods

        public bool Trigger() => Trigger(null, out _);

        public bool Trigger(IFactor triggeringFactor, out bool removeSubscription)
        {
            if (subscriber != null)
            {
                return subscriber.Trigger(triggeringFactor, out removeSubscription);
            }
            else
            {
                if (weakReferenceToSubscriber.TryGetTarget(out var weakSubscriber))
                {
                    return weakSubscriber.Trigger(triggeringFactor, out removeSubscription);
                }
                else
                {
                    removeSubscription = true;
                    
                    return false;
                }
            }
        }

        public bool Destabilize()
        {
            if (IsNecessary)
            {
                return true;
            }
            else if (subscriber != null)
            {
                return subscriber.Destabilize();
            }
            else
            {
                if (weakReferenceToSubscriber.TryGetTarget(out var weakSubscriber))
                {
                    return weakSubscriber.Destabilize();
                }
                else
                {
                    return false;
                }
            }
        }
        
        protected bool TryStabilizeOutcome()
        {
            foreach (var determinant in Triggers)
            {
                if (determinant.Reconcile() is false)
                {
                    return false;
                    //- No point in stabilizing the rest.  We try to stabilize to avoid updating, and if one
                    //  of our triggers fails to stabilize then we have no choice but to update.  In addition
                    //  we don't even know if we'll use the same triggers when we update. If we do end
                    //  up accessing those same triggers, they'll try to stabilize themselves when we access
                    //  them anyways.
                }
            }

            return true;
        }
        
        protected abstract void UnmarkTriggerAsNotifiedNecessary(IFactor trigger);
        protected abstract void MarkTriggerAsNotifiedNecessary(IFactor trigger);
        
        

        #endregion

        public WeakSubscriber(IFactorSubscriber subscriberToCall, bool necessary = false)
        {
            weakReferenceToSubscriber = new WeakReference<IFactorSubscriber>(subscriberToCall);

            if (necessary)
            {
                subscriber = subscriberToCall;
            }
        }
    }

    
    
    
}