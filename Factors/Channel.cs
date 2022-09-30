using System;
using System.Collections.Generic;
using Core.Factors;
using Factors.Cores.ProactiveCores;

namespace Factors
{
    public abstract class ChannelBase<TValue, TSubscriber>
    {
        #region Instance Fields

        protected readonly HashSet<TSubscriber> subscribers = new HashSet<TSubscriber>();

        #endregion


        #region Instance Properties

        public bool AllowReceiversToModifyValue { get; set; }
        public bool HasSubscribers              => NumberOfSubscribers > 0;
        public int  NumberOfSubscribers         => subscribers.Count;

        #endregion
        

        #region Instance Methods

        public virtual bool Subscribe(TSubscriber subscriberToAdd)
        {
            if (subscriberToAdd == null) { throw new ArgumentNullException(nameof(subscriberToAdd)); }

            return subscribers.Add(subscriberToAdd);
        }

        public virtual bool Unsubscribe(TSubscriber subscriberToRemove)
        {
            if (subscriberToRemove != null)
            {
                return subscribers.Remove(subscriberToRemove);
            }
            else return false;
        }

        public void Send(TValue valueToSend)
        {
            if (subscribers.Count > 0)
            {
                SendToSubscribers(valueToSend);
            }
        }
        
        public void Send(IEnumerable<TValue> valuesToSend)
        {
            if (subscribers.Count > 0)
            {
                SendToSubscribers(valuesToSend);
            }
        }

        protected abstract void SendToSubscribers(TValue value);
        protected abstract void SendToSubscribers(IEnumerable<TValue> values);

        #endregion
    }
    
    
    
    public class Channel<T> : ChannelBase<T, IChannelSubscriber<T>>, IChannel<T>
    {
        protected override void SendToSubscribers(T value)
        {
            foreach (var subscriber in subscribers)
            {
                subscriber.Receive(value);
            }
        }
        
        protected override void SendToSubscribers(IEnumerable<T> values)
        {
            foreach (var subscriber in subscribers)
            {
                subscriber.Receive(values);
            }
        }
    }
}