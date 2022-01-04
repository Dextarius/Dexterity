using System;
using System.Collections;
using System.Collections.Generic;
using Core.Causality;
using Core.Factors;
using Core.States;
using Factors.Outcomes.Influences;
using JetBrains.Annotations;
using static Core.Tools.Types;


namespace Factors.Collections
{
    public  abstract class ProactiveCollection<TCollection, TValue> : Proactor, ICollection<TValue>, ICollection
        where TCollection : ICollectionState<TValue>
    {
        #region Instance Fields

        protected readonly TCollection collection;
        
        #endregion
        
        
        #region Properties

        protected override IFactor Influence => collection;
        public             int     Count     => collection.Count;

        #endregion


        #region Instance Methods

        public void                Add(TValue item)                         => collection.Add(item);
        public void                AddRange(IEnumerable<TValue> itemsToAdd) => collection.AddRange(itemsToAdd);
        public void                AddRange(params TValue[] itemsToAdd)     => collection.AddRange(itemsToAdd);
        public bool                Remove(TValue itemToRemove)              => collection.Remove(itemToRemove);
        public bool                Contains(TValue item)                    => collection.Contains(item);
        public void                Clear()                                  => collection.Clear();
        public void                CopyTo(TValue[] array, int arrayIndex)   => collection.CopyTo(array, arrayIndex);
        public IEnumerator<TValue> GetEnumerator()                          => collection.GetEnumerator();

        #endregion

        
        #region Constructors

        protected ProactiveCollection(TCollection collectionState, string name = null) : base(name)
        {
            collection = collectionState;
        }

        #endregion
        

        #region Explicit Implementations

        void        ICollection.CopyTo(Array array, int index) => collection.CopyTo(array, index);
        bool        ICollection.IsSynchronized                 => false;
        bool        ICollection<TValue>.IsReadOnly             => false;
        IEnumerator IEnumerable.GetEnumerator()                => GetEnumerator();
        object      ICollection.SyncRoot                       => 
            throw new NotSupportedException($"{NameOf<ProactiveCollection<TCollection, TValue>>()} does not support SyncRoot. ");
        //- TODO : Come back to this later and decide if it's worthwhile/possible to implement SyncRoot.
        
        #endregion
        
        //- TODO : We should implement a mechanic where if a collection of factors updates,
        //         the message it sends to invalidate its dependents should include what action
        //         was taken on the collection (Add, Remove, etc) an we should make Reactive 
        //         collections that depend on them handle those different cases.  This might
        //         simplify the work we have to do for enabling Recycling considerably.
    }
}