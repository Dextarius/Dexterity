using System;
using System.Collections.Generic;

namespace Core.Factors
{
    public interface ICollectionFactor<TValue> : IEnumerableFactor<TValue>, IReadOnlyCollection<TValue>
    {
        bool Contains(TValue item);
        void CopyTo(TValue[] array, int index);
        void CopyTo(Array array, int index);
    }
}