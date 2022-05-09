using System;
using System.Collections.Generic;

namespace Core.States
{ 
    public interface ISetCore<T> : ICollectionCore<T>, IReadOnlySetMembers<T>
    {
        new bool       Add(T item);
            int        RemoveWhere(Predicate<T> predicate);
            void       ExceptWith(IEnumerable<T> other);
            void       IntersectWith(IEnumerable<T> other);
            void       SymmetricExceptWith(IEnumerable<T> other);
            void       UnionWith(IEnumerable<T> other);
            void       TrimExcess();
            HashSet<T> AsNormalSet();
    }
}