using System.Collections.Generic;

namespace Core.States
{
    public interface ICollectionState<TCollection, TValue> : IState<TCollection>  where TCollection : ICollection<TValue>
    {
        TCollection Collection { get; set; }
    }
}