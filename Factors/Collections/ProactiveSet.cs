using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Core.Causality;
using Core.Factors;
using Core.States;
using static Core.Tools.Types;

namespace Factors.Collections
{
    public class ProactiveSet<T> : ProactiveCollection<ISetState<T>, T>, ISet<T>
    {
        public new bool       Add(T item)                         => collection.Add(item);
        public     HashSet<T> AsNormalSet()                       => collection.AsNormalSet();
        public     int        RemoveWhere(Predicate<T> predicate) => collection.RemoveWhere(predicate);
        public     void       TrimExcess()                        => collection.TrimExcess();

        public void SymmetricExceptWith(IEnumerable<T> other) => collection.SymmetricExceptWith(other);
        public bool  IsProperSupersetOf(IEnumerable<T> other) => collection.IsProperSupersetOf(other);
        public bool    IsProperSubsetOf(IEnumerable<T> other) => collection.IsProperSubsetOf(other);
        public void       IntersectWith(IEnumerable<T> other) => collection.IntersectWith(other);
        public bool        IsSupersetOf(IEnumerable<T> other) => collection.IsSupersetOf(other);
        public bool          IsSubsetOf(IEnumerable<T> other) => collection.IsSubsetOf(other);
        public void          ExceptWith(IEnumerable<T> other) => collection.ExceptWith(other);
        public void           UnionWith(IEnumerable<T> other) => collection.UnionWith(other);
        public bool           SetEquals(IEnumerable<T> other) => collection.SetEquals(other);
        public bool            Overlaps(IEnumerable<T> other) => collection.Overlaps(other);


        #region Constructors
        
        public ProactiveSet(ISetState<T> setState, string name = null) : base(setState, name)
        {
        }

        #endregion
        
    }
}    