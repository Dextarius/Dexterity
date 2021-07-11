using System;
using System.Collections;
using System.Collections.Generic;
using Causality;
using Causality.States;
using Core.Causality;
using Core.Factors;
using JetBrains.Annotations;
using static Core.Tools.Types;


namespace Factors.Collections
{
    public  abstract partial class ProactiveCollection<TCollection, TValue> : ProactiveValue<TCollection>, ICollection<TValue>, ICollection
        where TCollection : ICollection<TValue>
    {
        #region Instance Fields

        [NotNull]
        protected readonly IEqualityComparer<TValue> itemComparer;
        protected readonly object syncLock = new object();

        #endregion
        
        #region Properties

        public int  Count => Collection.Count;

        //- TODO : SetValue uses the Interlocked methods, but most of the collection based methods use locks.
        //         We should figure out which we want to use and stick to it.  
        //-        Since the collection never changes (so far), we probably only need to make sure we use the lock
        //         when accessing the collection.  Consider what we might screw up if it did change later.
        protected TCollection Collection
        {
            get => state.Value;
            set => SetValue(value); 
        }

        #endregion


        #region Instance Methods
        
        public void Add(TValue item)
        {
            TCollection         collection    = Collection;
            IState<TCollection> previousState = state;
            IState<TCollection> newState      = new State<TCollection>(collection);

            lock (syncLock)
            {
                state = newState;
                collection.Add(item);
            }

            previousState.Invalidate();
            Observer.NotifyChanged(previousState);
        }
        
        public void AddRange(IEnumerable<TValue> itemsToAdd)
        {
            TCollection         collection    = Collection; //- So that we don't call NotifyInvolved() for every addition
            IState<TCollection> previousState = state;
            IState<TCollection> newState      = new State<TCollection>(collection);

            lock (syncLock)
            {
                state = newState;
                
                foreach (var item in itemsToAdd)
                {
                    collection.Add(item);
                }
            }

            previousState.Invalidate();
            Observer.NotifyChanged(previousState);
        }

        public void AddRange(params TValue[] itemsToAdd) => AddRange((IEnumerable<TValue>)itemsToAdd);

        public bool Remove(TValue item)
        {
            TCollection         collection    = Collection;
            IState<TCollection> previousState = state;
            IState<TCollection> newState      = new State<TCollection>(collection);
            bool wasSuccessful;

            lock (syncLock)
            {
                state = newState;
                wasSuccessful = Collection.Remove(item);
            }

            previousState.Invalidate();
            Observer.NotifyChanged(previousState);
            
            return wasSuccessful;
        }

        public void Clear()
        {
            TCollection         collection    = Collection;
            IState<TCollection> previousState = state;
            IState<TCollection> newState      = new State<TCollection>(collection);

            lock (syncLock)
            {
                state = newState;
                collection.Clear();
            }

            previousState.Invalidate();
            Observer.NotifyChanged(previousState);
        }
        
        public IEnumerator<TValue> GetEnumerator()
        {
            IState<TCollection> involvedState = state; //- To make sure it's the same collection each we call NotifyInvolved()
            
            foreach (TValue element in involvedState.Value)
            {
                Observer.NotifyInvolved(involvedState);
                yield return element;
            }
        }

        public bool Contains(TValue item)
        {
            lock (syncLock)
            {
                return Collection.Contains(item);
            }
        }

        public void CopyTo(TValue[] array, int arrayIndex) => Collection.CopyTo(array, arrayIndex);
        
        
        //- TODO : Implement Recycling. Perhaps write something more robust/reliable in the meantime.
        protected override bool ValuesAreDifferent(TCollection firstValue, TCollection secondValue)
        {
            return firstValue.Equals(secondValue) == false;
        }
        
        #endregion

        
        #region Constructors
        
        public ProactiveCollection(TCollection collection, IEqualityComparer<TValue> comparerForValues, string name = null) : base(collection, name)
        {
            itemComparer = comparerForValues?? EqualityComparer<TValue>.Default;  
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
    }
}