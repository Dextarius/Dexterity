using System;
using Core.Factors;

namespace Factors.Modifiers.Cores
{
    public class DoubleRangeLimiterModCore : RangeModifierCore<double>
    {
        protected override bool IsGreaterThan(double baseValue, double valueToCheck) => valueToCheck > baseValue;
        protected override bool IsLessThan(double baseValue, double valueToCheck)    => valueToCheck < baseValue;

        public DoubleRangeLimiterModCore(IFactor<double> minimumValueFactor, IFactor<double> maximumValueFactor) :
            base(minimumValueFactor, maximumValueFactor)
        {
        }
    }

    public class IntRangeLimiterModCore : RangeModifierCore<int>
    {
        protected override bool IsGreaterThan(int baseValue, int valueToCheck) => valueToCheck > baseValue;
        protected override bool IsLessThan(int baseValue, int valueToCheck)    => valueToCheck < baseValue;

        public IntRangeLimiterModCore(IFactor<int> minimumValueFactor, IFactor<int> maximumValueFactor) :
            base(minimumValueFactor, maximumValueFactor)
        {
            
        }
    }

    public class UIntRangeLimiterModCore : RangeModifierCore<uint>
    {
        protected override bool IsGreaterThan(uint baseValue, uint valueToCheck) => valueToCheck > baseValue;
        protected override bool IsLessThan(uint baseValue, uint valueToCheck)    => valueToCheck < baseValue;

        public UIntRangeLimiterModCore(IFactor<uint> minimumValueFactor, IFactor<uint> maximumValueFactor) :
            base(minimumValueFactor, maximumValueFactor)
        {
        }
    }

    public class TimeSpanRangeLimiterModCore : RangeModifierCore<TimeSpan>
    {
        protected override bool IsGreaterThan(TimeSpan baseValue, TimeSpan valueToCheck) => valueToCheck > baseValue;
        protected override bool IsLessThan(TimeSpan baseValue, TimeSpan valueToCheck)    => valueToCheck < baseValue;

        public TimeSpanRangeLimiterModCore(IFactor<TimeSpan> minimumValueFactor, IFactor<TimeSpan> maximumValueFactor) : 
            base(minimumValueFactor, maximumValueFactor)
        {
        }
    }
}
