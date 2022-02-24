using System;
using System.Collections.Generic;
using Core.Factors;
using Core.States;

namespace Factors.Cores
{
    class Subscription : IFactorSubscriber
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
                if (weakReference.TryGetTarget(out var weakSubscriber))
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
                if (weakReference.TryGetTarget(out var weakSubscriber))
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

        public Subscription(IFactorSubscriber subscriber)
        {
            this.subscriber = subscriber;
        }
    }
    
    
    
    
    
    
    
    
    
}