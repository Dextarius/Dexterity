using System;
using System.Collections.Generic;
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
                SendValueToSubscribers(valueToSend);
            }
        }

        protected abstract void SendValueToSubscribers(TValue value);

        #endregion
    }
    
    public class Channel<T> : ChannelBase<T, IChannelSubscriber<T>>, IChannel<T>
    {
        protected override void SendValueToSubscribers(T value)
        {
            foreach (var subscriber in subscribers)
            {
                subscriber.Receive(value);
            }
        }
    }
    
    
    public class ModifiableChannel<T> : ChannelBase<T, IChannelModifier<T>>, IModifiableChannel<T>
    {
        //- Add a priority system for subscribers.
        
        protected override void SendValueToSubscribers(T value)
        {
            foreach (var subscriber in subscribers)
            {
               value = subscriber.Receive(value);
            }
        }
    }

    public interface IModifiableChannel<T> 
    {
        bool   Subscribe(IChannelModifier<T> subscriberToAdd);
        bool Unsubscribe(IChannelModifier<T> subscriberToRemove);
    }
    
    public interface IChannel<T> 
    {
        bool   Subscribe(IChannelSubscriber<T> subscriberToAdd);
        bool Unsubscribe(IChannelSubscriber<T> subscriberToRemove);
    }
    
    public interface IChannelSubscriber<T>
    {
        void Receive(T value);
    }
    
    public interface IChannelModifier<T>
    {
        T Receive(T value);
    }
}