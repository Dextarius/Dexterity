using System;
using System.Collections.Generic;

namespace Core.Recycling
{
    public interface IRecyclingQueue<TRecycled> : IDisposable
    {
        bool AllItemsUsedWereRecycled { get; }
        bool NewItemsIsNotEmpty       { get; }
        bool OldItemsIsNotEmpty       { get; }
        
        void      Advance();
        TRecycled GetNextResult();
        void      AddAnyRemainingNewItems<TCollection>(TCollection results) where TCollection : ICollection<TRecycled>;
    }
}