using System.Collections.Generic;
using Core.Factors;
using Core.States;
using JetBrains.Annotations;

namespace Factors
{
    public class Aggregator<TValue> : ReactiveValue<TValue, IAggregateResult<TValue>>, IAggregateValue<TValue>
    {
        public bool   HasInputs      => core.HasInputs;
        public bool   NumberOfInputs => core.NumberOfInputs;
        
        public TValue BaseValue
        {
            get => core.BaseValue;
            set => core.BaseValue = value;
        }
        
        public bool Include(IFactor<TValue> factorToInclude) => core.Include(factorToInclude);
        public bool Remove(IFactor<TValue> factorToRemove)   => core.Remove(factorToRemove);  
        
        public void IncludeAll(IEnumerable<IFactor<TValue>> factorsToInclude)
        {
            foreach (var factor in factorsToInclude)
            {
                Include(factor);
            }
        }
        
        public void RemoveAll(IEnumerable<IFactor<TValue>> factorsToRemove)
        {
            foreach (var factor in factorsToRemove)
            {
                Remove(factor);
            }        
        }
        
        #region Operators

        public static implicit operator TValue(Aggregator<TValue> aggregator) => aggregator.Value;

        #endregion


        public Aggregator([NotNull] IAggregateResult<TValue> valueSource, string name = null) : base(valueSource, name)
        {
            
        }
    }
}