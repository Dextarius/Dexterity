using System;
using JetBrains.Annotations;

namespace Core.Subscriptions
{
    public interface ISubscribable
    {
        bool HasSubscribers { get; }
        
        ISubscription Subscribe([NotNull] Action actionToAdd);
        void Unsubscribe(Action actionToRemove);
        void Unsubscribe(ISubscription subscriptionToRemove);
    }
    
    public interface ISubscribable<out TValue> : ISubscribable
    {
        ISubscription Subscribe([NotNull] Action<TValue> actionToAdd);
        void Unsubscribe(Action<TValue> actionToRemove);
    }
    
    
    public interface ISubscribable<out TValue1, out TValue2> : ISubscribable<TValue1>
    {
        ISubscription Subscribe([NotNull] Action<TValue1, TValue2> actionToAdd);
        void Unsubscribe(Action<TValue1, TValue2> actionToRemove);
    }
}