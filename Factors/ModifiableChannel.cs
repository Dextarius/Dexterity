using System.Collections.Generic;
using Core.Factors;

namespace Factors
{
    public class ModifiableChannel<T> : ChannelBase<T, IChannelModifier<T>>, IModifiableChannel<T>
    {
        //- Add a priority system for subscribers.
        
        protected override void SendToSubscribers(T value)
        {
            foreach (var subscriber in subscribers)
            {
                value = subscriber.Receive(value);
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