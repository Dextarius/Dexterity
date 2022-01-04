using System.Collections.Generic;

namespace Core.States
{
    public interface ISetResult<T> : ICollectionResult<T>, IReadOnlySetMembers<T>
    {
        HashSet<T> AsNormalSet();
    }
}