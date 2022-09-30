using System.Collections.Generic;

namespace Core.Factors
{
    public interface IEnumerableFactor<out TValue> : IFactor, IEnumerable<TValue>
    {

    }
}