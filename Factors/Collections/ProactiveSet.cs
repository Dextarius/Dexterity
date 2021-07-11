using System;
using System.Collections.Generic;
using Causality;
using Causality.States;
using Core.Causality;
using Core.Factors;
using static Core.Tools.Types;

namespace Factors.Collections
{
    public class ProactiveSet<T> : ProactiveCollection<HashSet<T>, T>, ISet<T>
    {
        //- TODO : Should we be grabbing the value of collection before we are inside the lock in these methods? 
        //         It might lead to bugs if we ever start changing the value of Collection.
        
        public new bool Add(T item)
        {
            HashSet<T>         collection = Collection;
            IState<HashSet<T>> newState   = new State<HashSet<T>>(collection);
            IState<HashSet<T>> previousState;
            bool               wasSuccessful;

            lock (syncLock)
            {
                previousState = state;
                state = newState;
                wasSuccessful = collection.Add(item);
            }

            if (wasSuccessful)
            {
                previousState.Invalidate();
                Observer.NotifyChanged(previousState);
            }
            
            return wasSuccessful;
        }
        
        public int RemoveWhere(Predicate<T> predicate)
        {
            HashSet<T>         collection = Collection;
            IState<HashSet<T>> newState   = new State<HashSet<T>>(collection);
            IState<HashSet<T>> previousState;
            int                elementsRemoved;
 
            lock (syncLock)
            {
                previousState = state;
                state = newState;
                elementsRemoved = collection.RemoveWhere(predicate);
            }

            if (elementsRemoved > 0)
            {
                previousState.Invalidate();
                Observer.NotifyChanged(previousState);
            }

            return elementsRemoved;
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            HashSet<T>         collection = Collection;
            IState<HashSet<T>> newState   = new State<HashSet<T>>(collection);
            IState<HashSet<T>> previousState;
            bool               elementsWereRemoved;

            lock (syncLock)
            {
                int oldCount = collection.Count;

                previousState = state;
                state = newState;
                collection.ExceptWith(other);
                elementsWereRemoved = (collection.Count != oldCount);
            }

            if (elementsWereRemoved)
            {
                previousState.Invalidate();
                Observer.NotifyChanged(previousState); 
            }
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            HashSet<T>         collection = Collection;
            IState<HashSet<T>> newState   = new State<HashSet<T>>(collection);
            IState<HashSet<T>> previousState;
            bool               elementsWereRemoved;

            lock (syncLock)
            {
                int oldCount = collection.Count;

                previousState = state;
                state = newState;
                collection.IntersectWith(other);
                elementsWereRemoved = (collection.Count != oldCount);
            }

            if (elementsWereRemoved)
            {
                previousState.Invalidate();
                Observer.NotifyChanged(previousState); 
            }
        }
        
        public void  SymmetricExceptWith(IEnumerable<T> other)
        {
            HashSet<T>         collection    = Collection;
            IState<HashSet<T>> previousState = state;
            IState<HashSet<T>> newState      = new State<HashSet<T>>(collection);
            
            lock (syncLock)
            {
                state = newState;
                collection.SymmetricExceptWith(other);
            }

            previousState.Invalidate();
            Observer.NotifyChanged(previousState); 
        }
        
        public void UnionWith(IEnumerable<T> other)
        {
            HashSet<T>         collection = Collection;
            IState<HashSet<T>> newState   = new State<HashSet<T>>(collection);
            IState<HashSet<T>> previousState;

            lock (syncLock)
            {
                previousState = state;
                state = newState;
                collection.UnionWith(other);
            }

            previousState.Invalidate();
            Observer.NotifyChanged(previousState); 
        }
        
        public bool IsProperSupersetOf(IEnumerable<T> other) => Collection.IsProperSupersetOf(other);
        public bool   IsProperSubsetOf(IEnumerable<T> other) => Collection.IsProperSubsetOf(other);
        public bool       IsSupersetOf(IEnumerable<T> other) => Collection.IsSupersetOf(other);
        public bool         IsSubsetOf(IEnumerable<T> other) => Collection.IsSubsetOf(other);
        public bool          SetEquals(IEnumerable<T> other) => Collection.SetEquals(other);
        public bool           Overlaps(IEnumerable<T> other) => Collection.Overlaps(other);

        public void         TrimExcess() => Collection.TrimExcess();

        #region Constructors

        
        public ProactiveSet(IEqualityComparer<T> comparer = null, string name = null) : 
            this(new HashSet<T>(), comparer, name)
        {
            
        }
        
        public ProactiveSet(ICollection<T> collectionToUse, IEqualityComparer<T> comparer = null, string name = null) : 
            this(new HashSet<T>(collectionToUse), comparer, name)
        {
            
        }

        public ProactiveSet(HashSet<T> listToUse, IEqualityComparer<T> comparer = null, string name = null) : 
            base(listToUse, comparer, name ?? NameOf<ProactiveSet<T>>())
        {
            
        }

        #endregion
        
    }
}    