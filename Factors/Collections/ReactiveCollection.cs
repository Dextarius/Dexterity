using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Factors;
using Factors.Exceptions;
using JetBrains.Annotations;
using static Core.Tools.Types;
using Core.States;

namespace Factors.Collections
{
    public abstract class ReactiveCollection<TResult, TValue> : Reactor, ICollection<TValue>, ICollection
        where TResult : ICollectionResult<TValue>
    {
        #region Instance Fields

        [NotNull]
        protected readonly TResult result;

        #endregion


        #region Properties

        protected override IOutcome Outcome => result;
        public             int      Count   => result.Count;
        
        //- TODO : public bool UsesRecycling { get; set; } 

        #endregion


        #region Instance Methods
        public void                CopyTo(TValue[] array, int arrayIndex) => result.CopyTo(array, arrayIndex);
        public bool                Contains(TValue item)                  => result.Contains(item);
        public IEnumerator<TValue> GetEnumerator()                        => result.GetEnumerator();

        #endregion


        #region Constructors

        protected ReactiveCollection(TResult collectionResult, string name = null) : base(name)
        {
            result = collectionResult;
        }

        #endregion


        #region Explicit Implementations

        bool   ICollection<TValue>.IsReadOnly => true;
        bool   ICollection.IsSynchronized     => false;
        object ICollection.SyncRoot           => throw new NotSupportedException(
                                                     $"{NameOf<ReactiveCollection<TResult, TValue>>()} " +
                                                      "does not support SyncRoot. ");
        //^ We never know when the collection is going to be replaced so returning it's SyncRoot would be meaningless.
        //  We can't guarantee it'll be the same object every time.  
        //- TODO : Come back to this later and decide if it's worthwhile/possible to implement SyncRoot.
        
        void        ICollection.CopyTo(Array array, int index) => result.CopyTo(array, index);
        void        ICollection<TValue>.Add(TValue item)        => throw new CannotModifyReactiveValueException();
        bool        ICollection<TValue>.Remove(TValue item)     => throw new CannotModifyReactiveValueException();
        void        ICollection<TValue>.Clear()                 => throw new CannotModifyReactiveValueException();
        IEnumerator IEnumerable.GetEnumerator()                 => GetEnumerator();


        #endregion

    }
}