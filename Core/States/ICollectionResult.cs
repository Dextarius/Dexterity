using System.Collections.Generic;

namespace Core.States
{
    public interface ICollectionResult<TCollection, TValue> : IResult
        where TCollection : ICollection<TValue>
    {
        TCollection Collection { get; }

        IEnumerator<TValue> GetEnumerator();
        TCollection         Peek();
        void                CopyTo(TValue[] array, int arrayIndex);
        bool                Contains(TValue item);
    }
}