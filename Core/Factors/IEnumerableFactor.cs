using System.Collections.Generic;

namespace Core.Factors
{
    public interface IEnumerableFactor<TValue> : IFactor, IEnumerable<TValue>
    {

    }
}