using System;
using System.Collections.Generic;
using System.Linq;
using Core.States;
using Dextarius.Collections;
using static Factors.TriggerFlags;

namespace Factors.Collections
{
    public class HashSetImplementer<T> : CollectionImplementer<HashSet<T>, T, ICollectionOwner<T>>, 
                                         ISet<T>, IHashSetImplementer<T>
    {
        protected override bool AddItem(T item, out long notifyInvolvedFlags, out long additionalChangeFlags)
        {
            additionalChangeFlags = TriggerFlags.None;
            notifyInvolvedFlags   = TriggerFlags.None;
            return Collection.Add(item);
        }

        protected override bool RemoveItem(T item, out long additionalChangeFlags)
        {
            additionalChangeFlags = TriggerFlags.None;
            return Collection.Remove(item);
        }
        
        public int RemoveWhere(Predicate<T> shouldRemoveItem)
        {
            var removedItems = new List<T>();

            foreach (var item in Collection)
            {
                if (shouldRemoveItem(item))
                {
                    removedItems.Add(item);
                    collection.Remove(item);
                }
            }

            if (removedItems.Count > 0)
            {
                Owner.OnMultipleItemsRemoved(removedItems);
                Owner.OnCollectionChanged(ItemRemoved);
            }
            
            Owner.NotifyInvolved(TriggerWhenItemAdded | TriggerWhenItemReplaced);
            
            return removedItems.Count;
        }

        public void ExceptWith(IEnumerable<T> itemsToRemove)
        {
            var removedItems = new List<T>();
            
            foreach (var item in itemsToRemove)
            {
                if (collection.Remove(item))
                {
                    removedItems.Add(item);
                }
            }

            if (removedItems.Count > 0)
            {
                Owner.OnMultipleItemsRemoved(removedItems);
                Owner.OnCollectionChanged(ItemRemoved);
            }
        }
        
        public void IntersectWith(IEnumerable<T> other)
        {
            if (other is null) { throw new ArgumentNullException(nameof(other)); }
            
            if (this.Count is not 0)
            {
                long       triggerFlags = TriggerFlags.None;
                HashSet<T> removedItems;

                if (other == this)
                {
                    removedItems = new HashSet<T>(this);
                    collection.Clear();
                }
                else
                {
                    removedItems = other.ToHashSet();
                    
                    collection.RemoveWhere(potentialMatch =>
                    {
                        if (removedItems.Contains(potentialMatch))
                        {
                            removedItems.Remove(potentialMatch);
                            return false;
                        }
                        else return true;
                    });

                    if (removedItems.Count > 0)
                    {
                        triggerFlags = ItemRemoved;
                        Owner.OnMultipleItemsRemoved(removedItems);
                        Owner.OnCollectionChanged(triggerFlags);
                    }
                }
            }
            
            //- Not an optimal way to do this, but it'll work for now.
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            var  removedItems = new List<T>();
            var  addedItems   = new List<T>();
            long triggerFlags = TriggerFlags.None;

            foreach (var item in other)
            {
                if (collection.Remove(item))
                {
                    removedItems.Add(item);
                }
                else
                {
                    collection.Add(item);
                    addedItems.Add(item); 
                }
            }

            if (removedItems.Count > 0)
            {
                triggerFlags |= ItemRemoved;
                Owner.OnMultipleItemsRemoved(removedItems);
            }

            if (addedItems.Count > 0)
            {
                triggerFlags |= ItemAdded;
                Owner.OnMultipleItemsAdded(removedItems);
            }

            if (triggerFlags is not TriggerFlags.None)
            {
                Owner.OnCollectionChanged(triggerFlags);
            }
        }

        public void UnionWith(IEnumerable<T> other)
        {
            var addedItems = new List<T>();

            foreach (var item in other)
            {
                if (collection.Add(item))
                {
                    addedItems.Add(item);
                }
            }
            
            if (addedItems.Count > 0)
            {
                Owner.OnCollectionChanged(ItemAdded);
            }
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            bool meetsCriteria = Collection.IsProperSupersetOf(other);

            if (meetsCriteria) { NotifyInvolved(TriggerWhenItemReplaced | TriggerWhenItemRemoved); }
            else               { NotifyInvolved(TriggerWhenItemReplaced | TriggerWhenItemAdded);  }
            
            return meetsCriteria;
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            bool meetsCriteria = Collection.IsProperSubsetOf(other);

            if (meetsCriteria) { NotifyInvolved(TriggerWhenItemReplaced | TriggerWhenItemAdded);   }
            else               { NotifyInvolved(TriggerWhenItemReplaced | TriggerWhenItemRemoved); }
            
            return meetsCriteria;
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            bool meetsCriteria = Collection.IsSupersetOf(other);

            if (meetsCriteria) { NotifyInvolved(TriggerWhenItemReplaced | TriggerWhenItemRemoved); }
            else               { NotifyInvolved(TriggerWhenItemReplaced | TriggerWhenItemAdded); }
            
            return meetsCriteria;
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            bool meetsCriteria = Collection.IsSubsetOf(other);

            if (meetsCriteria) { NotifyInvolved(TriggerWhenItemReplaced | TriggerWhenItemAdded);   }
            else               { NotifyInvolved(TriggerWhenItemReplaced | TriggerWhenItemRemoved); }
            
            return meetsCriteria;
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            bool meetsCriteria = Collection.SetEquals(other);
            
            NotifyInvolved(TriggerWhenItemAdded | TriggerWhenItemRemoved | TriggerWhenItemReplaced);
            return meetsCriteria;
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            bool meetsCriteria = Collection.Overlaps(other);

            if (meetsCriteria) { NotifyInvolved(TriggerWhenItemReplaced | TriggerWhenItemRemoved); }
            else               { NotifyInvolved(TriggerWhenItemReplaced | TriggerWhenItemAdded);   }
            
            return meetsCriteria;
        }

        public HashSet<T> AsNormalSet()
        {
            var createdSet =  new HashSet<T>(Collection);

            NotifyInvolved(TriggerWhenItemAdded | TriggerWhenItemRemoved | TriggerWhenItemReplaced);
            return createdSet;
        }
        
        public override bool CollectionEquals(IEnumerable<T> collectionToCompare)
        {
            var setEquals = collectionToCompare.ToHashSet();

            return this.collection.SetEquals(setEquals);
        }
        
        public void TrimExcess() => Collection.TrimExcess();


        #region Constructors

        protected HashSetImplementer(ICollectionOwner<T> owner, HashSet<T> hashSet) : base(hashSet, owner)
        {
        }

        public HashSetImplementer(ICollectionOwner<T>  owner,
                                  IEnumerable<T>       collectionToCopy, 
                                  IEqualityComparer<T> comparerForElements = null) :
            this(owner, new HashSet<T>(collectionToCopy, comparerForElements))
        {
        }

        public HashSetImplementer(ICollectionOwner<T>  owner,
                                  HashSet<T>           setToCopy, 
                                  IEqualityComparer<T> comparerForElements = null) :
            this(owner, new HashSet<T>(setToCopy, comparerForElements ?? setToCopy.Comparer))
        {
        }

        public HashSetImplementer(ICollectionOwner<T> owner, IEqualityComparer<T> comparer) : 
            this(owner, new HashSet<T>(comparer))
        {
        }
        
        #endregion
    }
}