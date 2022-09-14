using System;
using Core.Factors;

namespace Factors.Cores
{
    public class HighestTimeSpanMod_SelectorCore : ValueSelectorCore<INumericMod<TimeSpan>>
    {
        protected override bool IsBetterMatchThan(INumericMod<TimeSpan> firstMod, INumericMod<TimeSpan> secondMod) => 
           firstMod.ModPriority < secondMod.ModPriority  &&  firstMod.Value > secondMod.Value;
    }

    public class LowestTimeSpanMod_SelectorCore : ValueSelectorCore<INumericMod<TimeSpan>>
    {
        protected override bool IsBetterMatchThan(INumericMod<TimeSpan> firstMod, INumericMod<TimeSpan> secondMod) => 
            firstMod.ModPriority < secondMod.ModPriority  &&  firstMod.Value < secondMod.Value;
    }
    
    public class HighestDoubleMod_SelectorCore : ValueSelectorCore<INumericMod<double>>
    {
        protected override bool IsBetterMatchThan(INumericMod<double> firstMod, INumericMod<double> secondMod) => 
            firstMod.ModPriority < secondMod.ModPriority  &&  firstMod.Value > secondMod.Value;
    }
    

    public class LowestDoubleMod_SelectorCore : ValueSelectorCore<INumericMod<double>>
    {
        protected override bool IsBetterMatchThan(INumericMod<double> firstMod, INumericMod<double> secondMod) => 
            firstMod.ModPriority < secondMod.ModPriority  &&  firstMod.Value < secondMod.Value;
    }
    
    public class HighestIntMod_SelectorCore : ValueSelectorCore<INumericMod<int>>
    {
        protected override bool IsBetterMatchThan(INumericMod<int> firstMod, INumericMod<int> secondMod) => 
            firstMod.ModPriority < secondMod.ModPriority  &&  firstMod.Value > secondMod.Value;
    }
    

    public class LowestIntMod_SelectorCore : ValueSelectorCore<INumericMod<int>>
    {
        protected override bool IsBetterMatchThan(INumericMod<int> firstMod, INumericMod<int> secondMod) => 
            firstMod.ModPriority < secondMod.ModPriority  &&  firstMod.Value < secondMod.Value;
    }
}