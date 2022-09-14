using Core.Factors;

namespace Factors.Cores
{
    public abstract class ValueSelectorCore<TValue> : AggregateValueCore<TValue, INumericMod<TValue>> 
    {
        protected override TValue GenerateValue()
        {
            TValue selectedValue = BaseValue;
            
            foreach (var currentFactor in inputFactors)
            {
                if(selectedValue is null ||
                   IsBetterMatchThan(currentFactor.Value, selectedValue))
                {
                    selectedValue = currentFactor.Value;
                }
            }

            return selectedValue;
        }
        
        protected abstract bool IsBetterMatchThan(TValue firstFactor, TValue secondFactor);
    }
}