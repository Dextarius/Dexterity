using System;

namespace Core.Redirection
{
    public abstract class ReadOnlyConservator<TSource, TValue> : Conservator<TValue>
    {
        #region Instance Fields

        protected readonly TSource collectionSource;

        #endregion

        #region Instance Methods

        protected override void OnAccessed() { }

        protected override void OnModified() => 
            throw new NotSupportedException($"{GetCollectionDescription()} cannot be modified directly. ");
        
        protected abstract string GetCollectionDescription();

        #endregion


        #region Constructors

        protected ReadOnlyConservator(TSource creator, Operations operationsSupported = Operations.CopyTo) : 
            base(operationsSupported)
        {
            collectionSource = creator;
        }

        #endregion
    }
}