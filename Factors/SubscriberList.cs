namespace Factors
{
    public class SubscriberList
    {
        
    }

    public readonly struct SubscriptionState
    {
        public readonly bool IsActive;
        public readonly bool IsNecessary;
        
        public SubscriptionState(bool active, bool necessary)
        {
            IsActive    = active;
            IsNecessary = necessary;
        }
    }

}