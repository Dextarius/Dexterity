using System.Collections.Generic;
using JetBrains.Annotations;

namespace Core.Recycling
{
    public interface IRecycler<TRecycled>
    {
        bool            CollectionsWereEqual { get; }
        
        List<TRecycled> RecycleItems();
        TResults        RecycleItems<TResults>([NotNull] TResults collectionToPutResultsIn)  where TResults : ICollection<TRecycled>;
    }
}