using System;
using System.Threading;
using Core.Subscriptions;
using static Core.InterlockedUtils;

namespace Subscriptions
{
    public class SubscriptionManager<TArg1, TArg2> :  ISubscribable, ISubscribable<TArg1>, ISubscriptionManager<TArg1, 
    TArg2>, IDisposable
    {
        #region Static Fields

        private static readonly ISubscriber<TArg1, TArg2>[] Terminated = new ISubscriber<TArg1, TArg2>[0];
        private static readonly ISubscriber<TArg1, TArg2>[] Disposed   = new ISubscriber<TArg1, TArg2>[0];

        #endregion
        

        #region Instance Fields

        private ISubscriber<TArg1, TArg2>[] subscriptions = Array.Empty<ISubscriber<TArg1, TArg2>>();
        private int                         subscriptionCount;

        #endregion


        #region Properties

        public bool HasSubscribers => Volatile.Read(ref subscriptionCount) > 0;

        #endregion

        
        #region Instance Methods

        //- TODO : We're assuming using arrays is going to be faster, even though we're recreating every time we add 
        //         or remove a subscription.  We should test this when we get a chance.

        private bool Add(ISubscriber<TArg1, TArg2> subscriptionToAdd)
        {
            if (subscriptionToAdd == null) { throw new ArgumentNullException(nameof(subscriptionToAdd)); }

            while (true)
            {
                var subscribers = Volatile.Read(ref subscriptions);

                if (subscribers == Disposed)
                {
                    ThrowDisposedException();
                    return false;
                }
                else if (subscribers == Terminated)
                {
                    return false;
                }
                else if ((Volatile.Read(ref subscriptionCount) < subscribers.Length) &&
                         TryCompareExchange(ref subscriptions[subscriptionCount], subscriptionToAdd, null))
                {
                    Interlocked.Increment(ref subscriptionCount);
                    return true;
                }
                else
                {
                    var expandedSubscribers = (subscribers.Length == 0)? new ISubscriber<TArg1, TArg2>[1] : 
                                                                         new ISubscriber<TArg1, TArg2>[subscribers.Length * 2];

                    for (int index = 0; index < subscribers.Length; index++)
                    {
                        expandedSubscribers[index] = subscribers[index];
                    }

                    expandedSubscribers[subscribers.Length] = subscriptionToAdd;

                    if (TryCompareExchange(ref subscriptions, expandedSubscribers, subscribers))
                    {
                        Interlocked.Increment(ref subscriptionCount);
                        return true;
                    }
                    
                    //- TODO : Decide if we should declare the expandedSubscribers variable higher up,
                    //         so we can avoid recreating it each time.
                }
            }
        }
        
        //- TODO : Consider making a RemoveAll() method. Perhaps a Contains() method as well?

        private void Remove(object subscriptionToRemove)
        {
            if (subscriptionToRemove == null) { throw new ArgumentNullException(nameof(subscriptionToRemove)); }

            while (true)
            {
                ISubscriber<TArg1, TArg2>[] currentSubscriptions = Volatile.Read(ref subscriptions);
                int              currentLength        = currentSubscriptions.Length;
                ISubscriber<TArg1, TArg2>[] newSubscriptions;
                
                if (currentLength == 0)
                {
                    return;
                }

                var matchingIndex = Array.IndexOf(currentSubscriptions, subscriptionToRemove);

                if (matchingIndex < 0)
                {
                    return;
                }

                if (currentLength == 1)
                {
                    newSubscriptions = Array.Empty<ISubscriber<TArg1, TArg2>>();
                }
                else
                {
                    newSubscriptions = new ISubscriber<TArg1, TArg2>[currentLength - 1];
                    
                    Array.Copy(currentSubscriptions, newSubscriptions, matchingIndex);
                    Array.Copy(currentSubscriptions, matchingIndex + 1, 
                               newSubscriptions,     matchingIndex, 
                               currentLength - matchingIndex - 1);
                }
                
                if (TryCompareExchange(ref subscriptions, newSubscriptions, currentSubscriptions))
                {
                    Interlocked.Decrement(ref subscriptionCount);
                    return;
                }
            }
        }
        
        private static void ThrowDisposedException() => throw new ObjectDisposedException(string.Empty);

        public void Publish(TArg1 newValue, TArg2 oldValue)
        {
            var subscribers = Volatile.Read(ref subscriptions);

            if (subscribers == Disposed)
            {
                ThrowDisposedException();
                return;
            }

            foreach (var subscription in subscribers)
            {
                subscription.Execute(newValue, oldValue);
            }
        }

        public ISubscription Subscribe(Action actionToAdd)
        {
            var subscription = new ActionSubscription<TArg1, TArg2>(actionToAdd, this);

            if (Add(subscription))
            {
                return subscription;
            }
            else
            {
                return default;
            }
        }

        public ISubscription Subscribe(Action<TArg1> actionToAdd)
        {
            var subscription = new ActionTSubscription<TArg1, TArg2>(actionToAdd, this);

            if (Add(subscription))
            {
                return subscription;
            }
            else
            {
                return default;
            }
        }

        public ISubscription Subscribe(Action<TArg1, TArg2> actionToAdd)
        {
            var subscription = new ActionT2Subscription<TArg1, TArg2>(actionToAdd, this);

            if (Add(subscription))
            {
                return subscription;
            }
            else
            {
                return default;
            }
        }

        public void Unsubscribe(ISubscription subscriptionToRemove) => Remove(subscriptionToRemove);

        public void Unsubscribe(Action<TArg1, TArg2> actionToRemove)        => Remove(actionToRemove);

        public void Unsubscribe(Action<TArg1> actionToRemove)           => Remove(actionToRemove);

        public void Unsubscribe(Action actionToRemove)              => Remove(actionToRemove);

        public void Dispose()
        {
            Interlocked.Exchange(ref subscriptions, Disposed);
        }

        #endregion

        //- TODO : We need to split some of the functionality in this class off into a base class.
        //         There should be separate classes to handle the different versions of ISubscriptionManager.
    }
}