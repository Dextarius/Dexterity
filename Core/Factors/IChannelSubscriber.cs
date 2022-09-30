using System.Collections.Generic;

namespace Core.Factors
{
    public interface IChannelSubscriber<T>
    {
        void Receive(T value);
        void Receive(IEnumerable<T> values);
    }
}