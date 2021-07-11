using JetBrains.Annotations;

namespace Core.Subscriptions
{
    public interface ISubscriptionManager : ISubscribable
    {
        void Publish();
    }
    
    public interface ISubscriptionManager<TValue> : ISubscribable<TValue>
    {
        void Publish([CanBeNull] TValue valueToPublish);
    }
    
    public interface ISubscriptionManager<TValue1, TValue2> : ISubscribable<TValue1, TValue2>
    {
        void Publish([CanBeNull] TValue1 firstValueToPublish, [CanBeNull] TValue2 secondValueToPublish);
    }
}