using System.Collections.Generic;

namespace Core.States
{
    public interface IReadOnlySetMembers<in T>
    { 
        bool IsProperSupersetOf(IEnumerable<T> other);
        bool   IsProperSubsetOf(IEnumerable<T> other);
        bool       IsSupersetOf(IEnumerable<T> other);
        bool         IsSubsetOf(IEnumerable<T> other);
        bool          SetEquals(IEnumerable<T> other);
        bool           Overlaps(IEnumerable<T> other);
    }
}