using System.Collections.Generic;

namespace Core.States
{
    public interface IListResult<T> : ICollectionResult<List<T>, T>
    {
        T this[int index] { get; }

        int IndexOf(T item);
    }
}