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
    //- What if we made it so that when a Factor added a CollectionFactor as a 
    //  trigger, they specified what operations (Add,Move,Remove,etc) were 
    //  relevant to the dependency they had (i.e. a dependency on the Count property
    //  of the collection would want to know if something was added or removed
    //  but not if an element just moved to a different index in the list).
    //
    //  Then when the collection changed something, it would pass an enum as an
    //  argument to the Trigger() method indicating what type of change had occurred.
    
    //  This could work with ObservedFactors as well, by having the collection
    //  call a version of NotifyInvolved() that passed different enum values
    //  based on what types of operations would invalidate the involved
    //  aspect (i.e. if the dependent Reactor had called Contains() on the
    //  collection and it returned false, the collection would pass an enum value
    //  indicating that when the dependency was Triggered() later, that 
    //  trigger was only relevant if the enum argument passed to the Trigger() method 
    //  indicated that the collection had Added a new element, since the fact that the
    //  collection doesn't contain that requested element won't change if the collection
    //  moves an existing element to another index, or removes an element from the collection).

    
    public abstract class ProactiveCollection<TCore, TValue> : Proactor<TCore>, ICollectionFactor<TValue>, 
                                                               ICollection<TValue>, ICollection
        where TCore : IProactiveCollectionCore<TValue>, IProactorCore
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

        public override bool CoresAreNotEqual(TCore oldCore, TCore newCore) => newCore.CollectionEquals(oldCore);

        #endregion

        
        #region Constructors

        protected ProactiveCollection(TCore collectionCore, string name = null) : base(collectionCore, name)
        {

        }

        #endregion
        

        #region Explicit Implementations

        void ICollectionFactor<TValue>.CopyTo(Array array, int index) => core.CopyTo(array, index);
        bool       ICollection<TValue>.IsReadOnly                     => false;
        void               ICollection.CopyTo(Array array, int index) => core.CopyTo(array, index);
        bool               ICollection.IsSynchronized                 => false;
        IEnumerator        IEnumerable.GetEnumerator()                => GetEnumerator();
        object             ICollection.SyncRoot                       => 
            throw new NotSupportedException($"{NameOf<ProactiveCollection<TCore, TValue>>()} does not support SyncRoot. ");
        //- TODO : Come back to this later and decide if it's worthwhile/possible to implement SyncRoot.
        
        #endregion
        
        //- TODO : We should implement a mechanic where if a collection of factors updates,
        //         the message it sends to invalidate its dependents should include what action
        //         was taken on the collection (Add, Remove, etc) and we should make Reactive 
        //         collections that depend on them handle those different cases.  This might
        //         simplify the work we have to do for enabling Recycling considerably.
    }

    
    public interface IAdvancedCollectionCreator<TCollection, TValue>  where TCollection : ICollection<TValue>
    {
        TCollection WithoutElementsWhere(Func<bool> predicate);
        TCollection CombinedWith(ICollectionFactor<TValue> otherCollection);
        TCollection IntersectedWith(ICollectionFactor<TValue> predicate);
        TCollection Subtracting(ICollectionFactor<TValue> collectionToSubtract);
    }
}