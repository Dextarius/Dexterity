using System;
using System.Collections.Generic;
using Causality.Processes;
using Core.Causality;
using Core.Factors;
using Factors.Exceptions;
using JetBrains.Annotations;

namespace Factors.Collections
{
    public class ReactiveSet<T> : ReactiveCollection<HashSet<T>, T>, ISet<T>
    {
        #region Static Properties

        public static IEqualityComparer<T> DefaultValueComparer { get; } = EqualityComparer<T>.Default;

        #endregion


        #region Instance Methods

        protected override HashSet<T> CreateCollectionFromElements(IEnumerable<T> elements) => new HashSet<T>(elements);

        public bool IsProperSupersetOf(IEnumerable<T> other) => Collection.IsProperSupersetOf(other);
        public bool   IsProperSubsetOf(IEnumerable<T> other) => Collection.IsProperSubsetOf(other);
        public bool       IsSupersetOf(IEnumerable<T> other) => Collection.IsSupersetOf(other);
        public bool         IsSubsetOf(IEnumerable<T> other) => Collection.IsSubsetOf(other);
        public bool          SetEquals(IEnumerable<T> other) => Collection.SetEquals(other);
        public bool           Overlaps(IEnumerable<T> other) => Collection.Overlaps(other);

        #endregion


        #region Constructors

        public ReactiveSet([NotNull] Func<IEnumerable<T>> functionToGenerateItems, string nameToGive = null) : 
            this(functionToGenerateItems, DefaultValueComparer, nameToGive)
        {
            
        }
        
        public ReactiveSet([NotNull] Func<IEnumerable<T>> functionToGenerateItems, 
                           IEqualityComparer<T> comparer, string nameToGive = null) : 
            this(FunctionalProcess.CreateFrom(functionToGenerateItems), comparer, nameToGive)
        {
        }

        public ReactiveSet([NotNull] IProcess<IEnumerable<T>> processToGenerateItems, string nameToGive = null) :
            this(processToGenerateItems, DefaultValueComparer, nameToGive)
        {
        }

        public ReactiveSet([NotNull] IProcess<IEnumerable<T>> processToGenerateItems, 
                           IEqualityComparer<T> comparerForValues, string name = null) : 
            base(processToGenerateItems, comparerForValues, name)
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