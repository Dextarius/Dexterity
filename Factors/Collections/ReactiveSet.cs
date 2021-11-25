using System;
using System.Collections.Generic;
using Causality.Processes;
using Causality.States.CollectionStates;
using Core.Causality;
using Core.Factors;
using Core.States;
using Factors.Exceptions;
using JetBrains.Annotations;

namespace Factors.Collections
{
    public class ReactiveSet<T> : ReactiveCollection<IHashsetResult<T>, HashSet<T>, T>, ISet<T>
    {
        #region Instance Methods
        
        //- TODO : We should probably move these into the HashsetOutcome class. 

        public bool IsProperSupersetOf(IEnumerable<T> other) => Collection.IsProperSupersetOf(other);
        public bool   IsProperSubsetOf(IEnumerable<T> other) => Collection.IsProperSubsetOf(other);
        public bool       IsSupersetOf(IEnumerable<T> other) => Collection.IsSupersetOf(other);
        public bool         IsSubsetOf(IEnumerable<T> other) => Collection.IsSubsetOf(other);
        public bool          SetEquals(IEnumerable<T> other) => Collection.SetEquals(other);
        public bool           Overlaps(IEnumerable<T> other) => Collection.Overlaps(other);

        #endregion


        #region Constructors

        public ReactiveSet([NotNull] Func<IEnumerable<T>> functionToGenerateItems, string nameToGive = null) : 
            this(functionToGenerateItems, null, nameToGive)
        {
        }
        
        public ReactiveSet([NotNull] Func<IEnumerable<T>> functionToGenerateItems, 
                           IEqualityComparer<T> comparerForItems, string nameToGive = null) : 
            this(FunctionalProcess.CreateFrom(functionToGenerateItems), comparerForItems, nameToGive)
        {
        }

        public ReactiveSet([NotNull] IProcess<IEnumerable<T>> processToGenerateItems, string nameToGive = null) :
            this(processToGenerateItems, null, nameToGive)
        {
        }

        public ReactiveSet([NotNull] IProcess<IEnumerable<T>> processToGenerateItems, 
                           IEqualityComparer<T> comparerForItems, string name = null) : 
            base(name)
        {
            outcome = new HashSetResult<T>(this, processToGenerateItems, comparerForItems);
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