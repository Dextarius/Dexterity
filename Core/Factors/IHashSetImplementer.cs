using System;
using System.Collections.Generic;

namespace Factors.Collections
{
    public interface IHashSetImplementer<T> : ICollectionImplementer<T>
    {
        HashSet<T> AsNormalSet();
        void       ExceptWith(IEnumerable<T> other);
        void       IntersectWith(IEnumerable<T> other);
        bool       IsProperSupersetOf(IEnumerable<T> other);
        bool       IsProperSubsetOf(IEnumerable<T> other);
        bool       IsSupersetOf(IEnumerable<T> other);
        bool       IsSubsetOf(IEnumerable<T> other);
        bool       Overlaps(IEnumerable<T> other);
        int        RemoveWhere(Predicate<T> shouldRemoveItem);
        bool       SetEquals(IEnumerable<T> other);
        void       SymmetricExceptWith(IEnumerable<T> other);
        void       TrimExcess();
        void       UnionWith(IEnumerable<T> other);
    }
}