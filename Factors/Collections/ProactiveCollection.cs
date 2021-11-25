using System;
using System.Collections;
using System.Collections.Generic;
using Causality;
using Causality.States;
using Core.Causality;
using Core.Factors;
using Core.States;
using JetBrains.Annotations;
using static Core.Tools.Types;


namespace Factors.Collections
{
    public  abstract partial class ProactiveCollection<TCollection, TValue> : Proactor, ICollection<TValue>, ICollection
        where TCollection : ICollection<TValue>
    {
        #region Instance Fields

        [NotNull]
        protected readonly IEqualityComparer<TValue>  itemComparer;
        protected          IMutableState<TCollection> state;

        //- TODO : We may want to rethink our use of the itemComparer.  Quite a few of the collections that inherit
        //         this have various behaviors determined by how their items are compared (HashSets are a good example).
        
        #endregion
        
        #region Properties

        public int Count => Collection.Count;
        
        protected TCollection Collection
        {
            get => state.Value;
            set => state.Value = value; 
        }

        

        #endregion


        #region Instance Methods
        
        public void Add(TValue item)
        {
            TCollection collection = state.Peek();  
            //- We use Peek() because we don't want the state to mark itself as involved right before we change it,
            // that would automatically invalidate anything that used this method.
            
            collection.Add(item);
            state.OnChanged();
            state.NotifyInvolved();
        }

        public void AddRange(IEnumerable<TValue> itemsToAdd)
        {
            TCollection collection = state.Peek();  

            foreach (var item in itemsToAdd)
            {
                collection.Add(item);
            }

            state.OnChanged();
            state.NotifyInvolved();
        }

        public void AddRange(params TValue[] itemsToAdd) => AddRange((IEnumerable<TValue>)itemsToAdd);

        public bool Remove(TValue item)
        {
            TCollection collection    = state.Peek();
            bool        wasSuccessful = collection.Remove(item);
            
            if (wasSuccessful)
            {
                state.OnChanged();
            }
            
            state.NotifyInvolved();
            
            return wasSuccessful;
        }

        public void Clear()
        {
            TCollection collection = state.Peek();

            if (collection.Count > 0)
            {
                collection.Clear();
                state.OnChanged();
            }
        }
        
        public IEnumerator<TValue> GetEnumerator()
        {
            IState<TCollection> involvedState = state; //- To make sure it's the same collection each we call NotifyInvolved()
            
            foreach (TValue element in involvedState.Value)
            {
                state.NotifyInvolved();
                yield return element;
            }
            
            //- TODO : Do we really want to call NotifyInvolved() on every iteration?
        }

        public bool Contains(TValue item) => Collection.Contains(item);

        public void CopyTo(TValue[] array, int arrayIndex) => Collection.CopyTo(array, arrayIndex);

        protected override IFactor GetFactorImplementation() => state;
        
        // public void SafeAdd(TValue item)
        // {
        //     TCollection         collection    = Collection;
        //     IState<TCollection> previousState = state;
        //     IState<TCollection> newState      = new State<TCollection>(collection);
        //
        //     lock (syncLock)
        //     {
        //         state = newState;
        //         collection.Add(item);
        //     }
        //
        //     previousState.Invalidate();
        //     Observer.NotifyChanged(previousState);
        // }
        
        // public void SafeAddRange(IEnumerable<TValue> itemsToAdd)
        // {
        //     TCollection         collection    = Collection; //- So that we don't call NotifyInvolved() for every addition
        //     IState<TCollection> previousState = state;
        //     IState<TCollection> newState      = new State<TCollection>(collection);
        //
        //     lock (syncLock)
        //     {
        //         state = newState;
        //         
        //         foreach (var item in itemsToAdd)
        //         {
        //             collection.Add(item);
        //         }
        //     }
        //
        //     previousState.Invalidate();
        //     Observer.NotifyChanged(previousState);
        // }
        //
        // public bool Remove(TValue item)
        // {
        //     TCollection         collection    = Collection;
        //     IState<TCollection> previousState = state;
        //     IState<TCollection> newState      = new State<TCollection>(collection);
        //     bool                wasSuccessful;
        //
        //     lock (syncLock)
        //     {
        //         state = newState;
        //         wasSuccessful = Collection.Remove(item);
        //     }
        //
        //     previousState.Invalidate();
        //     Observer.NotifyChanged(previousState);
        //     
        //     return wasSuccessful;
        // }
        //
        // public void Clear()
        // {
        //     TCollection         collection    = Collection;
        //     IState<TCollection> previousState = state;
        //     IState<TCollection> newState      = new State<TCollection>(collection);
        //
        //     lock (syncLock)
        //     {
        //         state = newState;
        //         collection.Clear();
        //     }
        //
        //     previousState.Invalidate();
        //     Observer.NotifyChanged(previousState);
        // }
        //
        // public IEnumerator<TValue> GetEnumerator()
        // {
        //     IState<TCollection> involvedState = state; //- To make sure it's the same collection each we call NotifyInvolved()
        //     
        //     foreach (TValue element in involvedState.Value)
        //     {
        //         Observer.NotifyInvolved(involvedState);
        //         yield return element;
        //     }
        // }
        //
        // public bool Contains(TValue item)
        // {
        //     lock (syncLock)
        //     {
        //         return Collection.Contains(item);
        //     }
        // }
        
        
        #endregion

        
        #region Constructors
        
        public ProactiveCollection(TCollection collection, IEqualityComparer<TValue> comparerForValues, string name = null) : 
            base(name)
        {
            itemComparer = comparerForValues?? EqualityComparer<TValue>.Default;
            state = new State<TCollection>(this, collection);
        }

        #endregion
        

        #region Operators

        // public static implicit operator TCollection(ProactiveCollection<TCollection, TValue> ProactiveCollection) => 
        //     ProactiveCollection.outcome.Value;
        //- TODO : Do we want this?  How likely are people to use it on accident?
        //- We probably shouldn't since they'll be able to modify the collection. Maybe just return a conservator implementing IList?

        #endregion
        
        
        #region Explicit Implementations


        void        ICollection.CopyTo(Array array, int index) => ((ICollection)Collection).CopyTo(array, index);
        bool        ICollection.IsSynchronized                 => false;
        bool        ICollection<TValue>.IsReadOnly             => true;
        IEnumerator IEnumerable.GetEnumerator()                => GetEnumerator();
        object      ICollection.SyncRoot                       => throw new NotSupportedException(
                                                                     $"{NameOf<ProactiveCollection<TCollection, TValue>>()} does not support SyncRoot. ");
        //^ We never know when the collection is going to be replaced so returning it's SyncRoot would be meaningless.
        //  We can't guarantee it'll be the same object every time.  
        //- TODO : Come back to this later and decide if it's worthwhile/possible to implement SyncRoot.
        
        #endregion
        
        
        //- TODO : We should implement a mechanic where if a collection of factors updates,
        //         the message it sends to invalidate its dependents should include what action
        //         was taken on the collection (Add, Remove, etc) an we should make Reactive 
        //         collections that depend on them handle those different cases.  This might
        //         simplify the work we have to do for enabling Recycling considerably.
    }
}