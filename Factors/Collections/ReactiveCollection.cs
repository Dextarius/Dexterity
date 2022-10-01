using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Factors;
using Factors.Exceptions;
using JetBrains.Annotations;
using static Dextarius.Utilities.Types;
using Core.States;

namespace Factors.Collections
{
    public abstract class ReactiveCollection<TCore, TValue> : Reactor<TCore>, ICollection<TValue>, ICollection
        where TCore : ICollectionResult<TValue>
    {
        #region Properties

        public int Count => Collection.Count;
        
        protected TCore Collection
        {
            get
            {
                AttemptReaction();
                return core;
            }
        }

        //- TODO : public bool UsesRecycling { get; set; } 

        #endregion


        #region Instance Methods

        public void CopyTo(TValue[] array, int arrayIndex) => Collection.CopyTo(array, arrayIndex);

        public bool Contains(TValue item) => Collection.Contains(item);

        public override bool CoresAreNotEqual(TCore oldCore, TCore newCore) => newCore.CollectionEquals(oldCore);

        public IEnumerator<TValue> GetEnumerator() => Collection.GetEnumerator();

        #endregion


        #region Constructors

        protected ReactiveCollection(TCore collectionCore, string name = null) : base(collectionCore, name)
        {
        }

        #endregion


        #region Explicit Implementations

        bool   ICollection<TValue>.IsReadOnly => true;
        bool   ICollection.IsSynchronized     => false;
        object ICollection.SyncRoot           => throw new NotSupportedException(
                                                     $"{NameOf<ReactiveCollection<TCore, TValue>>()} " +
                                                      "does not support SyncRoot. ");
        //^ We never know when the collection is going to be replaced so returning it's SyncRoot would be meaningless.
        //  We can't guarantee it'll be the same object every time.  
        //- TODO : Come back to this later and decide if it's worthwhile/possible to implement SyncRoot.
        
        void        ICollection.CopyTo(Array array, int index) => Collection.CopyTo(array, index);
        void        ICollection<TValue>.Add(TValue item)       => throw new CannotModifyReactiveValueException();
        bool        ICollection<TValue>.Remove(TValue item)    => throw new CannotModifyReactiveValueException();
        void        ICollection<TValue>.Clear()                => throw new CannotModifyReactiveValueException();
        IEnumerator IEnumerable.GetEnumerator()                => GetEnumerator();


        #endregion

    }
}