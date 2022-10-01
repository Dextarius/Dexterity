using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Core.States;
using Dextarius.Collections;
using Factors.Collections;
using static Dextarius.Utilities.Types;

namespace Factors.Cores.ProactiveCores
{
    public class ObservedProactiveHashSetCore<T> : ObservedProactiveCollectionCore<HashSet<T>, T, IHashSetImplementer<T>>, 
                                                   ISetCore<T>
    {
        #region Instance Methods
        
        public void SymmetricExceptWith(IEnumerable<T> other) => implementer.SymmetricExceptWith(other);
        public bool  IsProperSupersetOf(IEnumerable<T> other) => implementer.IsProperSupersetOf(other);
        public bool    IsProperSubsetOf(IEnumerable<T> other) => implementer.IsProperSubsetOf(other);
        public void       IntersectWith(IEnumerable<T> other) => implementer.IntersectWith(other);
        public bool        IsSupersetOf(IEnumerable<T> other) => implementer.IsSupersetOf(other);
        public void          ExceptWith(IEnumerable<T> other) => implementer.ExceptWith(other);
        public bool          IsSubsetOf(IEnumerable<T> other) => implementer.IsSubsetOf(other);
        public bool           SetEquals(IEnumerable<T> other) => implementer.SetEquals(other);
        public void           UnionWith(IEnumerable<T> other) => implementer.UnionWith(other);
        public bool            Overlaps(IEnumerable<T> other) => implementer.Overlaps(other);

        public HashSet<T> AsNormalSet() => implementer.AsNormalSet();
        public void       TrimExcess()  => implementer.TrimExcess();
        
        public int RemoveWhere(Predicate<T> shouldRemoveItem) => implementer.RemoveWhere(shouldRemoveItem);

        #endregion


        #region Constructors
        
        protected ObservedProactiveHashSetCore(IHashSetImplementer<T> implementation)
        {
            implementer = implementation;
        }

        protected ObservedProactiveHashSetCore(HashSet<T> hashSet)
        {
            implementer = new HashSetImplementer<T>(this, hashSet);
        }

        public ObservedProactiveHashSetCore(
            IEnumerable<T> collectionToCopy, IEqualityComparer<T> comparerForElements = null) :
            this(new HashSet<T>(collectionToCopy, comparerForElements))
        {
        }

        public ObservedProactiveHashSetCore(HashSet<T> setToCopy, IEqualityComparer<T> comparerForElements = null) :
            this(new HashSet<T>(setToCopy, comparerForElements ?? setToCopy.Comparer))
        {
        }

        public ObservedProactiveHashSetCore(IEqualityComparer<T> comparer) : this(new HashSet<T>(comparer))
        {
        }

        public ObservedProactiveHashSetCore() : this(new HashSet<T>())
        {
        }
        
        #endregion
    }
}