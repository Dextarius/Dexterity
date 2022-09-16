using System;
using System.Collections.Generic;
using Core.States;
using Dextarius.Collections;
using static Core.Tools.Types;

namespace Factors.Cores.ProactiveCores
{
    public class ObservedProactiveHashSetCore<T> : ObservedProactiveCollectionCore<HashSet<T>, T>, ISetCore<T>
    {
        protected override bool AddItem(T item, out long notifyInvolvedFlags, out long additionalChangeFlags)
        {
            additionalChangeFlags = TriggerFlags.None;

            if (Collection.Add(item))
            {
                notifyInvolvedFlags = TriggerFlags.ItemRemoved;
                return true;
            }
            else
            {
                notifyInvolvedFlags = TriggerFlags.None;
                return false;
            }
        }

        protected override bool RemoveItem(T item, out long additionalChangeFlags)
        {
            additionalChangeFlags = TriggerFlags.None;
            return Collection.Remove(item);
        }

        public int RemoveWhere(Predicate<T> shouldRemoveItem)
        {
            int numberOfItemsRemoved = collection.RemoveWhere(shouldRemoveItem);

            if (numberOfItemsRemoved > 0)
            {
                OnCollectionChanged(TriggerFlags.ItemRemoved);
            }

            NotifyInvolved(TriggerFlags.TriggerWhenItemAdded | TriggerFlags.ItemReplaced);

            return numberOfItemsRemoved;
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            int  oldCount = collection.Count;
            bool elementsWereRemoved;

            collection.ExceptWith(other);
            elementsWereRemoved = collection.Count != oldCount;

            if (elementsWereRemoved)
            {
                OnCollectionChanged(TriggerFlags.ItemRemoved);
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
                OnCollectionChanged(TriggerFlags.ItemRemoved);
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
                OnCollectionChanged(TriggerFlags.ItemRemoved);
            }
        }

        public void UnionWith(IEnumerable<T> other)
        {
            int  oldCount = collection.Count;
            bool elementsWereAdded;

            collection.UnionWith(other);
            elementsWereAdded = collection.Count != oldCount;

            if (elementsWereAdded)
            {
                OnCollectionChanged(TriggerFlags.ItemAdded);
            }
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            bool meetsCriteria = Collection.IsProperSupersetOf(other);

            if (meetsCriteria)
            {
                NotifyInvolved(TriggerFlags.TriggerWhenItemRemoved | TriggerFlags.TriggerWhenItemReplaced);
            }
            else
            {
                NotifyInvolved(TriggerFlags.TriggerWhenItemAdded | TriggerFlags.TriggerWhenItemReplaced);
            }
            
            return meetsCriteria;
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            bool meetsCriteria = Collection.IsProperSubsetOf(other);

            if (meetsCriteria)
            {
                NotifyInvolved(TriggerFlags.TriggerWhenItemAdded | TriggerFlags.TriggerWhenItemReplaced);
            }
            else
            {
                NotifyInvolved(TriggerFlags.TriggerWhenItemRemoved | TriggerFlags.TriggerWhenItemReplaced);
            }
            
            return meetsCriteria;
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            bool meetsCriteria = Collection.IsSupersetOf(other);

            if (meetsCriteria)
            {
                NotifyInvolved(TriggerFlags.TriggerWhenItemRemoved | TriggerFlags.TriggerWhenItemReplaced);
            }
            else
            {
                NotifyInvolved(TriggerFlags.TriggerWhenItemAdded | TriggerFlags.TriggerWhenItemReplaced);
            }
            
            return meetsCriteria;
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            bool meetsCriteria = Collection.IsSubsetOf(other);

            if (meetsCriteria)
            {
                NotifyInvolved(TriggerFlags.TriggerWhenItemAdded | TriggerFlags.TriggerWhenItemReplaced);
            }
            else
            {
                NotifyInvolved(TriggerFlags.TriggerWhenItemRemoved | TriggerFlags.TriggerWhenItemReplaced);
            }
            
            return meetsCriteria;
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            bool meetsCriteria = Collection.SetEquals(other);
            
            NotifyInvolved(TriggerFlags.TriggerWhenItemAdded   | 
                           TriggerFlags.TriggerWhenItemRemoved | 
                           TriggerFlags.TriggerWhenItemReplaced);
            return meetsCriteria;
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            bool meetsCriteria = Collection.Overlaps(other);

            if (meetsCriteria)
            {
                NotifyInvolved(TriggerFlags.TriggerWhenItemRemoved | TriggerFlags.TriggerWhenItemReplaced);
            }
            else
            {
                NotifyInvolved(TriggerFlags.TriggerWhenItemAdded | TriggerFlags.TriggerWhenItemReplaced);
            }
            
            return meetsCriteria;
        }

        public HashSet<T> AsNormalSet()
        {
            var createdSet =  new HashSet<T>(Collection);

            NotifyInvolved(TriggerFlags.TriggerWhenItemAdded   | 
                           TriggerFlags.TriggerWhenItemRemoved | 
                           TriggerFlags.TriggerWhenItemReplaced);
            return createdSet;
        }
        
        public override bool CollectionEquals(IEnumerable<T> collectionToCompare)
        {
            var setEquals = collectionToCompare.ToHashSet();

            return this.collection.SetEquals(setEquals);
        }
        
        public void TrimExcess() => Collection.TrimExcess();


        #region Constructors

        protected ObservedProactiveHashSetCore(HashSet<T> hashSet) : base(hashSet)
        {

        }

        public ObservedProactiveHashSetCore(
            IEnumerable<T> collectionToCopy, IEqualityComparer<T> comparerForElements = null) :
            this(new HashSet<T>(collectionToCopy, comparerForElements))
        {

        }

        public ObservedProactiveHashSetCore(HashSet<T> setToCopy, IEqualityComparer<T> comparerForElements = null) :
            this(new HashSet<T>(setToCopy, comparerForElements ?? setToCopy.Comparer))
        {

        }

        public ObservedProactiveHashSetCore(IEqualityComparer<T> comparer) : this(new HashSet<T>(comparer))
        {
        }

        public ObservedProactiveHashSetCore() : this(new HashSet<T>())
        {
        }
        
        #endregion
    }
}