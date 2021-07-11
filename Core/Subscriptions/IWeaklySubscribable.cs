using System;
using JetBrains.Annotations;

namespace Core.Subscriptions
{
        public interface IWeaklySubscribable
        {
            void SubscribeWeakly([NotNull] Action actionToAdd);
        }
        
        public interface IWeaklySubscribable<TValue>
        {
            void SubscribeWeakly([NotNull] Action<TValue> actionToAdd);
        }
        
        public interface IWeaklySubscribable<TValue1, TValue2>
        {
            void SubscribeWeakly([NotNull] Action<TValue1, TValue2> actionToAdd);
        }
}