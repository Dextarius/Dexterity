using System.Collections.Generic;

namespace Core.Factors
{
    public interface IModTypeOrder
    {
        IEnumerable<NumericModType> ModTypesByPriority { get; }
    }
}