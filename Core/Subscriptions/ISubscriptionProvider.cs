namespace Core.Subscriptions
{
    public interface ISubscriptionProvider
    {
        ISubscriptionManager<TValue1, TValue2> CreateNewSubscriptionManager<TValue1, TValue2>();
        ISubscriptionManager<TValue>           CreateNewSubscriptionManager<TValue>();
        ISubscriptionManager                   CreateNewSubscriptionManager();
    }
}