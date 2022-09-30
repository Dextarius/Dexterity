using System;
using System.Collections.Generic;
using System.Linq;
using Core.States;

namespace Factors.Cores.ProactiveCores
{
    public class DirectProactiveHashSetCore<T> : DirectProactiveCollectionCore<HashSet<T>, T>
    {
        #region Instance Methods

        protected override bool AddItem(T itemToAdd, out long additionalNotifyFlags, out long additionalChangeFlags)
        {
            collection.Add(itemToAdd);
            additionalChangeFlags = TriggerFlags.None;
            additionalNotifyFlags = TriggerFlags.None;
            return true;
        }
        
        protected override bool RemoveItem(T item, out long additionalTriggerFlags)
        {
            additionalTriggerFlags = TriggerFlags.None;
            return Collection.Remove(item);
        }

        public int RemoveWhere(Predicate<T> predicate)
        {
            if (predicate is null) { throw new ArgumentNullException(nameof(predicate)); }

            int numberOfItemsRemoved = 0;

            foreach (var item in collection.ToList())
            {
                if (predicate(item))
                {
                    collection.Remove(item);
                    OnItemRemoved(item);
                    numberOfItemsRemoved++;
                }
            }

            return numberOfItemsRemoved;
        }
        
        public void ExceptWith(IEnumerable<T> itemsToRemove)
        {
            foreach (var item in itemsToRemove)
            {
                if (collection.Remove(item))
                {
                    OnItemRemoved(item);
                }
            }
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            var itemsToRemove = new HashSet<T>(collection);
            
            itemsToRemove.ExceptWith(other);
            
            foreach (var item in itemsToRemove)
            {
                if (collection.Remove(item))
                {
                    OnItemRemoved(item);
                }
            }
            
            //- Not an optimal way to do this, but it'll work for now.
        }
        
        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            foreach (var item in other)
            {
                if (collection.Remove(item))
                {
                    OnItemRemoved(item);
                }
                else
                {
                    collection.Add(item);
                    OnItemAdded(item);
                }
            }
        }
        
        public void UnionWith(IEnumerable<T> other)
        {
            foreach (var item in other)
            {
                if (collection.Add(item))
                {
                    OnItemAdded(item);
                }
            }
        }
        
        public bool IsProperSupersetOf(IEnumerable<T> other) => Collection.IsProperSupersetOf(other);
        public bool   IsProperSubsetOf(IEnumerable<T> other) => Collection.IsProperSubsetOf(other);
        public bool       IsSupersetOf(IEnumerable<T> other) => Collection.IsSupersetOf(other);
        public bool         IsSubsetOf(IEnumerable<T> other) => Collection.IsSubsetOf(other);
        public bool          SetEquals(IEnumerable<T> other) => Collection.SetEquals(other);
        public bool           Overlaps(IEnumerable<T> other) => Collection.Overlaps(other);

        public HashSet<T> AsNormalSet() => new HashSet<T>(Collection);
        public void       TrimExcess()  => Collection.TrimExcess();

        #endregion

        
        #region Constructors

        protected DirectProactiveHashSetCore(HashSet<T> hashSet) : base(hashSet)
        {
        }
        
        public DirectProactiveHashSetCore(IEnumerable<T>       collectionToCopy, 
                                          IEqualityComparer<T> comparerForElements = null) :
                this(new HashSet<T>(collectionToCopy, comparerForElements))
        {
        }
        
        public DirectProactiveHashSetCore(HashSet<T> setToCopy, IEqualityComparer<T> comparerForElements = null) : 
            this(new HashSet<T>(setToCopy, comparerForElements ?? setToCopy.Comparer))
        {
        }
        
        public DirectProactiveHashSetCore(IEqualityComparer<T> comparer) : this(new HashSet<T>(comparer))
        {
        }

        public DirectProactiveHashSetCore() : this(new HashSet<T>())
        {
        }
        
        #endregion
    }
}