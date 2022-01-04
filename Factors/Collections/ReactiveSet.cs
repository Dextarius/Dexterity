using System;
using System.Collections.Generic;
using Core.Factors;
using Core.States;
using Factors.Exceptions;
using JetBrains.Annotations;
using static Core.Tools.Types;

namespace Factors.Collections
{
    public class ReactiveSet<T> : ReactiveCollection<ISetResult<T>, T>, ISet<T>, IReadOnlySetMembers<T>
    {
        #region Static Fields

        private static readonly string DefaultName = NameOf<ReactiveSet<T>>();

        #endregion
        
        
        #region Instance Methods
        
        public HashSet<T> AsNormalSet() => result.AsNormalSet();

        public bool IsProperSupersetOf(IEnumerable<T> other) => result.IsProperSupersetOf(other);
        public bool   IsProperSubsetOf(IEnumerable<T> other) => result.IsProperSubsetOf(other);
        public bool       IsSupersetOf(IEnumerable<T> other) => result.IsSupersetOf(other);
        public bool         IsSubsetOf(IEnumerable<T> other) => result.IsSubsetOf(other);
        public bool          SetEquals(IEnumerable<T> other) => result.SetEquals(other);
        public bool           Overlaps(IEnumerable<T> other) => result.Overlaps(other);
        
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