using Core.Factors;

namespace Factors.Cores
{
    // public abstract class FactorSelectorCore<TFactor> :  AggregatorCore<TFactor, TFactor> 
    //     where TFactor : IFactor
    // {
    //     protected override TFactor GenerateValue()
    //     {
    //         TFactor selectedFactor = default;
    //         
    //         foreach (var currentFactor in inputFactors)
    //         {
    //             if(selectedFactor is null ||
    //                IsBetterMatchThan(currentFactor, selectedFactor))
    //             {
    //                 selectedFactor = currentFactor;
    //             }
    //         }
    //
    //         return selectedFactor;
    //     }
    //     
    //     protected abstract bool IsBetterMatchThan(TFactor firstFactor, TFactor secondFactor);
    // }
}