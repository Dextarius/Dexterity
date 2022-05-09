using System;
using System.Collections;
using System.Collections.Generic;
using Core.Causality;
using Core.Factors;
using Core.States;
using JetBrains.Annotations;
using static Core.Tools.Types;


namespace Factors.Collections
{
    public  abstract class ProactiveCollection<TCore, TValue> : Factor<TCore>, ICollection<TValue>, ICollection
        where TCore : ICollectionCore<TValue>
    {
        #region Properties

        public int Count => core.Count;

        #endregion


        #region Instance Methods

        public void                Add(TValue item)                         => core.Add(item);
        public void                AddRange(IEnumerable<TValue> itemsToAdd) => core.AddRange(itemsToAdd);
        public void                AddRange(params TValue[] itemsToAdd)     => core.AddRange(itemsToAdd);
        public bool                Remove(TValue itemToRemove)              => core.Remove(itemToRemove);
        public bool                Contains(TValue item)                    => core.Contains(item);
        public void                Clear()                                  => core.Clear();
        public void                CopyTo(TValue[] array, int arrayIndex)   => core.CopyTo(array, arrayIndex);
        public IEnumerator<TValue> GetEnumerator()                          => core.GetEnumerator();

        #endregion

        
        #region Constructors

        protected ProactiveCollection(TCore collectionCore, string name = null) : base(collectionCore, name)
        {

        }

        #endregion
        

        #region Explicit Implementations

        void        ICollection.CopyTo(Array array, int index) => core.CopyTo(array, index);
        bool        ICollection.IsSynchronized                 => false;
        bool        ICollection<TValue>.IsReadOnly             => false;
        IEnumerator IEnumerable.GetEnumerator()                => GetEnumerator();
        object      ICollection.SyncRoot                       => 
            throw new NotSupportedException($"{NameOf<ProactiveCollection<TCore, TValue>>()} does not support SyncRoot. ");
        //- TODO : Come back to this later and decide if it's worthwhile/possible to implement SyncRoot.
        
        #endregion
        
        //- TODO : We should implement a mechanic where if a collection of factors updates,
        //         the message it sends to invalidate its dependents should include what action
        //         was taken on the collection (Add, Remove, etc) an we should make Reactive 
        //         collections that depend on them handle those different cases.  This might
        //         simplify the work we have to do for enabling Recycling considerably.
    }
}