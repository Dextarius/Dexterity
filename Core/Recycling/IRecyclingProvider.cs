using System;
using System.Collections.Generic;

namespace Core.Recycling
{
    public interface IRecyclingProvider
    {
        IRecycler<TRecycled> CreateRecycler<TRecycled>(IEnumerable<TRecycled> itemsToRecycle, 
                                                       IEnumerable<TRecycled> itemsToCompare);

        IRecycler<TRecycled> CreateRecycler<TRecycled>(IEnumerable<TRecycled>       itemsToRecycle, 
                                                       IEnumerable<TRecycled>       itemsToCompare, 
                                                       IEqualityComparer<TRecycled> comparerToDetermineIfRecyclable);
        

        IRecycler<TRecycled> CreateRecycler<TRecycled, TCompared>(IEnumerable<TRecycled>     itemsToRecycle, 
                                                                  IEnumerable<TCompared>     itemsToCompare, 
                                                                  IEqualityComparer<object>  comparerToDetermineIfRecyclable,
                                                                  Func<TCompared, TRecycled> functionToConstructValues);
    }
}