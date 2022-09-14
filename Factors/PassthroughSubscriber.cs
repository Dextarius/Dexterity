using Core.Factors;
using Core.States;
using Dextarius.Collections;
using JetBrains.Annotations;

namespace Factors
{
    public class PassthroughSubscriber : IReactorSubscriber
    {
        #region Properties
        
        public IFactorSubscriber Subscriber  { get; set; }
        public bool              IsNecessary { get; set; }
        public bool              IsUnstable  { get; set; }
        public bool              IsTriggered { get; set; } = true;

        #endregion


        #region Instance Methods

        public bool Trigger() => Trigger(null, TriggerFlags.Default, out _);

        public bool Trigger(IFactor triggeringFactor,  long triggerFlags, out bool removeSubscription)
        {
            if (Subscriber is null)
            {
                removeSubscription = true;
                return false;
            }
            else if (IsTriggered)
            {
                removeSubscription = false;
                return false;
            }
            else 
            {
                return Subscriber.Trigger(triggeringFactor, triggerFlags, out removeSubscription);
            }
        }

        public bool Destabilize()
        {
            if (IsNecessary)
            {
                return true;
            }
            else if (IsUnstable)
            {
                return false;
            }
            else
            {
                return Subscriber?.Destabilize() ?? false;
            }
        }

        #endregion


        public PassthroughSubscriber(IFactorSubscriber subscriberToCall)
        {
            Subscriber = subscriberToCall;
        }
    }
}