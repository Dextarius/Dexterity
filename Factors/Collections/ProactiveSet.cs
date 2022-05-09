using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Core.Causality;
using Core.Factors;
using Core.States;
using static Core.Tools.Types;

namespace Factors.Collections
{
    public class ProactiveSet<T> : ProactiveCollection<ISetCore<T>, T>, ISet<T>
    {
        public     HashSet<T> AsNormalSet()                       => core.AsNormalSet();
        public new bool       Add(T item)                         => core.Add(item);
        public     void       TrimExcess()                        => core.TrimExcess();
        public     int        RemoveWhere(Predicate<T> predicate) => core.RemoveWhere(predicate);

        public void SymmetricExceptWith(IEnumerable<T> other) => core.SymmetricExceptWith(other);
        public bool  IsProperSupersetOf(IEnumerable<T> other) => core.IsProperSupersetOf(other);
        public bool    IsProperSubsetOf(IEnumerable<T> other) => core.IsProperSubsetOf(other);
        public void       IntersectWith(IEnumerable<T> other) => core.IntersectWith(other);
        public bool        IsSupersetOf(IEnumerable<T> other) => core.IsSupersetOf(other);
        public bool          IsSubsetOf(IEnumerable<T> other) => core.IsSubsetOf(other);
        public void          ExceptWith(IEnumerable<T> other) => core.ExceptWith(other);
        public void           UnionWith(IEnumerable<T> other) => core.UnionWith(other);
        public bool           SetEquals(IEnumerable<T> other) => core.SetEquals(other);
        public bool            Overlaps(IEnumerable<T> other) => core.Overlaps(other);


        #region Constructors
        
        public ProactiveSet(ISetCore<T> setCore, string name = null) : base(setCore, name)
        {
        }

        #endregion
        
    }
}    