using System;
using System.Collections;
using System.Collections.Generic;
using static Core.Tools.Types;


namespace Core.Redirection
{
    //- For use when an object wants to provide an instance of a Collection, but consumers aren't supposed to be
    //      accessing it directly, i.e the Keys collection of a Reactive Dictionary
    public abstract class Conservator<TValue> : ICollection<TValue>, ICollection
    {
        //- TODO : Revisit these and make sure they work as intended. Write should include most of the others.
        
        [Flags]
        public enum Operations
        {
            None     = 0,
            Add      = 1,
            Remove   = 2,
            Clear    = 4, 
            Contains = 16,
            CopyTo   = 32,
            Write    = 64,
            All      = int.MaxValue
        }

        #region Instance Fields

        private readonly Operations validOperations;
        private readonly bool       isReadOnly;

        #endregion


        #region Properties

        public abstract ICollection<TValue> ManagedCollection { get; }

        public int Count { get { OnAccessed();
                                 return ManagedCollection.Count; } }
        
        public bool IsReadOnly
        {
            get
            {
                OnAccessed();
                return isReadOnly;
            }
        }

        #endregion
        
        
        #region Instance Methods

        protected abstract void OnAccessed();
        protected abstract void OnModified();


        public bool Contains(TValue item)
        {
            OnAccessed();
            return ManagedCollection.Contains(item);
        }

        public void Add(TValue item)
        {
            if ((validOperations & Operations.Add) == Operations.Add)
            {
                ManagedCollection.Add(item);
                OnModified();
            }
            else
            {
                throw new NotSupportedException($"A process attempted to add an element to a {NameOf<Conservator<TValue>>()} " +
                                                $"whose list of supported operations does not include " +
                                                $"{nameof(Operations)}.{Operations.CopyTo}. ");
            }
        }

        public void Clear()
        {
            if ((validOperations & Operations.Clear) == Operations.Clear)
            {
                ManagedCollection.Clear();
                OnModified();
            }
            else
            {
                throw new NotSupportedException($"A process attempted to clear a {NameOf<Conservator<TValue>>()} " +
                                                $"whose list of supported operations does not include " +
                                                $"{nameof(Operations)}.{Operations.Clear}. ");
            }
        }

        public void CopyTo(TValue[] array, int arrayIndex)
        {
            if ((validOperations & Operations.CopyTo) == Operations.CopyTo)
            {
                OnAccessed();
                ManagedCollection.CopyTo(array, arrayIndex);
            }
            else
            {
                throw new NotSupportedException($"A process attempted to copy a {NameOf<Conservator<TValue>>()} " +
                                                $"whose list of supported operations does not include " +
                                                $"{nameof(Operations)}.{Operations.CopyTo}. ");
            }
        }

        public bool Remove(TValue item)
        {
            if ((validOperations & Operations.Remove) == Operations.Remove)
            {
                bool result = ManagedCollection.Remove(item);

                OnModified();

                return result;
            }
            else
            {
                throw new NotSupportedException($"A process attempted to remove an element from a {NameOf<Conservator<TValue>>()} " +
                                                $"whose list of supported operations does not include "           +
                                                $"{nameof(Operations)}.{Operations.Remove}. ");
            }
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            foreach (TValue element in ManagedCollection)
            {
                OnAccessed();
                yield return element;
            }
        }

        public void CopyTo(Array array, int index)
        {
            if (array is TValue[] castArray)
            {
                CopyTo(castArray, index);
            }
            else
            {
                TryToCopyUsingICollection(array, index);
            }
        }
        
        
        private void TryToCopyUsingICollection(Array array, int index)
        {
            if ((validOperations & Operations.CopyTo) == Operations.CopyTo)
            {
                if (ManagedCollection is ICollection castCollection)
                {
                    OnAccessed();
                    castCollection.CopyTo(array, index);
                }
                else
                {
                    throw new NotSupportedException($"The collection managed by this {NameOf<Conservator<TValue>>()} " +
                                                    $"does not implement {nameof(ICollection)} meaning so it cannot support " +
                                                    $"copying to the type {nameof(Array)}. ");
                }
            }
            else
            {
                throw new NotSupportedException($"A process attempted to copy a {NameOf<Conservator<TValue>>()} " +
                                                $"whose list of supported operations does not include "           +
                                                $"{nameof(Operations)}.{Operations.CopyTo}. ");
            }

        }

        #endregion


        #region Constructors

        public Conservator(Operations operationsSupported)
        {
            validOperations = operationsSupported;
            isReadOnly      = (validOperations & Operations.Write) != Operations.Write;
        }

        #endregion


        #region Explicit Implementations

        bool ICollection.IsSynchronized
        {
            get
            {
                OnAccessed();
                
                if (ManagedCollection is ICollection castCollection) { return castCollection.IsSynchronized; }
                else                                                 { return false; }
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                OnAccessed();
                
                if (ManagedCollection is ICollection castCollection)
                {
                    return castCollection.SyncRoot;
                }
                else
                {
                    throw new NotSupportedException(
                        $"The collection managed by this {NameOf<Conservator<TValue>>()} " +
                        $"does not implement {nameof(ICollection)} meaning a {nameof(ICollection.SyncRoot)} " +
                        $"cannot be provided.");
                }
            }
        }
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

    }
}