using System;
using System.Collections.Generic;
using Core.States;
using static Core.Tools.Types;

namespace Factors.Cores.ProactiveCores
{ 
    public class ObservedHashSetCore<T> : ObservedCollectionCore<HashSet<T>, T>, ISetCore<T>
    {
        public HashSet<T> AsNormalSet() => new HashSet<T>(Collection);

        public new bool Add(T item)
        {
            bool wasSuccessful = collection.Add(item);
            
            if (wasSuccessful)
            {
                OnCollectionChanged();
            }
            
            NotifyInvolved();

            return wasSuccessful;
        }
        
        public int RemoveWhere(Predicate<T> predicate)
        {
            int elementsRemoved = collection.RemoveWhere(predicate);

            if (elementsRemoved > 0)
            {
                OnCollectionChanged();
            }
            
            NotifyInvolved();

            return elementsRemoved;
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            int  oldCount = collection.Count;
            bool elementsWereRemoved;
            
            collection.ExceptWith(other);
            elementsWereRemoved = collection.Count != oldCount;

            if (elementsWereRemoved)
            {
                OnCollectionChanged();
            }
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            int  oldCount = collection.Count;
            bool elementsWereRemoved;
            
            collection.IntersectWith(other);
            elementsWereRemoved = collection.Count != oldCount;

            if (elementsWereRemoved)
            {
                OnCollectionChanged();
            }
        }
        
        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            int  oldCount = collection.Count;
            bool elementsWereRemoved;
            
            collection.SymmetricExceptWith(other);
            elementsWereRemoved = collection.Count != oldCount;

            if (elementsWereRemoved)
            {
                OnCollectionChanged();
            }
        }
        
        public void UnionWith(IEnumerable<T> other)
        {
            int  oldCount = collection.Count;
            bool elementsWereRemoved;
            
            collection.UnionWith(other);
            elementsWereRemoved = collection.Count != oldCount;

            if (elementsWereRemoved)
            {
                OnCollectionChanged();
            }
        }
        
        public bool IsProperSupersetOf(IEnumerable<T> other) => Collection.IsProperSupersetOf(other);
        public bool   IsProperSubsetOf(IEnumerable<T> other) => Collection.IsProperSubsetOf(other);
        public bool       IsSupersetOf(IEnumerable<T> other) => Collection.IsSupersetOf(other);
        public bool         IsSubsetOf(IEnumerable<T> other) => Collection.IsSubsetOf(other);
        public bool          SetEquals(IEnumerable<T> other) => Collection.SetEquals(other);
        public bool           Overlaps(IEnumerable<T> other) => Collection.Overlaps(other);

        public void TrimExcess() => Collection.TrimExcess();

        #region Constructors

        protected ObservedHashSetCore(
            ICollection<T> collectionToCopy, IEqualityComparer<T> comparerForElements = null, string name = null) :
                base(new HashSet<T>(collectionToCopy, comparerForElements), name ?? NameOf<ObservedHashSetCore<T>>())
        {
            
        }
        
        public ObservedHashSetCore(string name = null) : this(null, null, name)
        {
        }

        public ObservedHashSetCore(IEqualityComparer<T> comparer, string name = null) : 
            this(new HashSet<T>(comparer), comparer ?? EqualityComparer<T>.Default, name)
        {
        }

        public ObservedHashSetCore(HashSet<T> setToUse, string name = null) : 
            this(setToUse, setToUse.Comparer, name ?? NameOf<ObservedHashSetCore<T>>())
        {
        }

        #endregion
    }
}