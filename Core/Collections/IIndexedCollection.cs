using System.Collections;

namespace Core.Collections
{
    public interface IIndexedReferenceCollection<T> : ICollection
    {
       ref T this[int index] { get; }
    }
}