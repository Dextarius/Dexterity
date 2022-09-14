using System.Collections.Generic;
using Core.Factors;

namespace Factors
{
    public class DefaultModTypeOrder : IModTypeOrder
    {
        public IEnumerable<NumericModType> ModTypesByPriority
        {
            get
            {
                yield return NumericModType.Multiplicative;
                yield return NumericModType.Additive;
                yield return NumericModType.Flat;
                yield return NumericModType.ConstantValue;
            }
        }
    }
}