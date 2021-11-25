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
        public new bool Add(T item)
        {
            HashSet<T> collection    = state.Peek();
            bool       wasSuccessful = collection.Add(item);
            
            if (wasSuccessful)
            {
                state.OnChanged();
            }
            
            return wasSuccessful;
        }
        
        public int RemoveWhere(Predicate<T> predicate)
        {
            HashSet<T> collection = state.Peek();
            int        elementsRemoved = collection.RemoveWhere(predicate);

            if (elementsRemoved > 0)
            {
                state.OnChanged();
            }

            return elementsRemoved;
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            HashSet<T> collection = state.Peek();
            int        oldCount   = collection.Count;
            bool       elementsWereRemoved;
            
            collection.ExceptWith(other);
            elementsWereRemoved = collection.Count != oldCount;

            if (elementsWereRemoved)
            {
                state.OnChanged();
            }
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            HashSet<T> collection = state.Peek();
            int        oldCount   = collection.Count;
            bool       elementsWereRemoved;
            
            collection.IntersectWith(other);
            elementsWereRemoved = collection.Count != oldCount;

            if (elementsWereRemoved)
            {
                state.OnChanged();
            }
        }
        
        public void  SymmetricExceptWith(IEnumerable<T> other)
        {
            HashSet<T> collection = state.Peek();
            int        oldCount   = collection.Count;
            bool       elementsWereRemoved;
            
            collection.SymmetricExceptWith(other);
            elementsWereRemoved = collection.Count != oldCount;

            if (elementsWereRemoved)
            {
                state.OnChanged();
            }

        }
        
        public void UnionWith(IEnumerable<T> other)
        {
            HashSet<T> collection = state.Peek();
            int        oldCount   = collection.Count;
            bool       elementsWereRemoved;
            
            collection.UnionWith(other);
            elementsWereRemoved = collection.Count != oldCount;

            if (elementsWereRemoved)
            {
                state.OnChanged();
            }
        }
        
        public bool IsProperSupersetOf(IEnumerable<T> other) => Collection.IsProperSupersetOf(other);
        public bool   IsProperSubsetOf(IEnumerable<T> other) => Collection.IsProperSubsetOf(other);
        public bool       IsSupersetOf(IEnumerable<T> other) => Collection.IsSupersetOf(other);
        public bool         IsSubsetOf(IEnumerable<T> other) => Collection.IsSubsetOf(other);
        public bool          SetEquals(IEnumerable<T> other) => Collection.SetEquals(other);
        public bool           Overlaps(IEnumerable<T> other) => Collection.Overlaps(other);

        public void         TrimExcess() => Collection.TrimExcess();

        #region Constructors

        public ProactiveSet(string name = null) : this(new HashSet<T>(), null, name)
        {
            
        }
        
        public ProactiveSet(IEqualityComparer<T> comparer, string name = null) : 
            this(new HashSet<T>(comparer), comparer ?? EqualityComparer<T>.Default, name)
        {
        }
        
        public ProactiveSet(ICollection<T> collectionToCopy, IEqualityComparer<T> comparer = null, string name = null) : 
            this(new HashSet<T>(collectionToCopy, comparer), comparer, name)
        {
        }

        public ProactiveSet(HashSet<T> setToUse, string name = null) : 
            this(setToUse, setToUse.Comparer, name ?? NameOf<ProactiveSet<T>>())
        {
            
        }

        protected ProactiveSet(HashSet<T> setToUse, IEqualityComparer<T> comparer = null, string name = null) :
            base(setToUse, comparer, name ?? NameOf<ProactiveSet<T>>())
        {
            
        }
        #endregion
        
    }
}    