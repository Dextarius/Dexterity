using System.Collections.Generic;

namespace Core.Factors
{
    public interface IChannelModifier<T>
    {
        T    Receive(T value);
        void Receive(IEnumerable<T> values);

    }
}