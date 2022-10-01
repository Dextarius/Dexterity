using System;
using System.Collections.Generic;
using Core.Factors;
using Core.States;
using Factors.Exceptions;
using JetBrains.Annotations;
using static Dextarius.Utilities.Types;

namespace Factors.Collections
{
    public class ReactiveSet<T> : ReactiveCollection<ISetResult<T>, T>, ISet<T>, IReadOnlySetMembers<T>
    {
        #region Static Fields

        private static readonly string DefaultName = NameOf<ReactiveSet<T>>();

        #endregion
        
        
        #region Instance Methods
        
        public HashSet<T> AsNormalSet() => Collection.AsNormalSet();

        public bool IsProperSupersetOf(IEnumerable<T> other) => Collection.IsProperSupersetOf(other);
        public bool   IsProperSubsetOf(IEnumerable<T> other) => Collection.IsProperSubsetOf(other);
        public bool       IsSupersetOf(IEnumerable<T> other) => Collection.IsSupersetOf(other);
        public bool         IsSubsetOf(IEnumerable<T> other) => Collection.IsSubsetOf(other);
        public bool          SetEquals(IEnumerable<T> other) => Collection.SetEquals(other);
        public bool           Overlaps(IEnumerable<T> other) => Collection.Overlaps(other);
        
        #endregion


        #region Constructors

        public ReactiveSet([NotNull] ISetResult<T> collectionSource, string name = null) : 
            base(collectionSource, name ?? DefaultName)
        {
        }

        #endregion
        
        
        #region Explicit Implementations

        void ISet<T>.SymmetricExceptWith(IEnumerable<T> other) => throw new CannotModifyReactiveValueException();
        void ISet<T>.IntersectWith(IEnumerable<T> other)       => throw new CannotModifyReactiveValueException();
        void ISet<T>.ExceptWith(IEnumerable<T> other)          => throw new CannotModifyReactiveValueException();
        void ISet<T>.UnionWith(IEnumerable<T> other)           => throw new CannotModifyReactiveValueException();
        bool ISet<T>.Add(T item)                               => throw new CannotModifyReactiveValueException();

        #endregion

    }
}