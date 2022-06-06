using System.Collections.Generic;
using Core.Factors;

namespace Factors.Cores.DirectReactorCores
{
    public class DirectRelayCore<TValue> : DirectResult<TValue>
    {
        private readonly IFactor<TValue> valueSource;

        #region Properties

        public override int NumberOfTriggers => 1;
        
        protected override IEnumerable<IFactor> Triggers
        {
            get
            {
                yield return valueSource;
            }
        }

        #endregion

        protected override TValue GenerateValue() => valueSource.Value;

        public DirectRelayCore(IFactor<TValue> factorToGetValueOf)
        {
            valueSource = factorToGetValueOf;
        }
    }
}