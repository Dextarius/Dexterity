using System.Collections.Generic;
using Core.Factors;

namespace Core.States
{
    public interface IListResult<T> : ICollectionResult<T>, IReadOnlyListMembers<T>
    {
        T this[int index] { get; }

        List<T> AsNormalList();
    }
}