using System;
using System.Collections.Generic;
using Core.Factors;
using static Core.Tools.Numerics;

namespace Factors.Cores
{
    public class DoubleControllerModCore : ValueControllerModCore<double>
    {
        protected override double Multiply(double valueToModify, double multiplier) => valueToModify * multiplier;

        protected override double Add(double valueToModify, double amountToAdd) => valueToModify + amountToAdd;
        
        protected override bool ValuesAreDifferent(double first, double second, out long triggerFlags)
        {
            if (DoublesAreNotEqual(first, second))
            {
                triggerFlags = TriggerFlags.Default;
                return true;
            }
            else
            {
                triggerFlags = TriggerFlags.None;
                return false;
            }
        }
        
        protected override double ApplyMaximum(double valueToModify, double maximum)
        {
            if (valueToModify > maximum)
            {
                return maximum;
            }
            else return valueToModify;
        }
        
        protected override double ApplyMinimum(double valueToModify, double minimum)
        {
            if (valueToModify < minimum)
            {
                return minimum;
            }
            else return valueToModify;        
        }
        
        public DoubleControllerModCore(double initialBaseValue, IModTypeOrder modOrder) : base(initialBaseValue, modOrder)
        {
        }

        public DoubleControllerModCore(double initialBaseValue = default) : base(initialBaseValue)
        {
        }
    }
    
    
    public class IntControllerModCore : ValueControllerModCore<int>
    {
        protected override int Multiply(int valueToModify, double multiplier) => (int)(valueToModify * multiplier);

        protected override int Add(int valueToModify, int amountToAdd) => (valueToModify + amountToAdd);
        
        protected override bool ValuesAreDifferent(int first, int second, out long triggerFlags)
        {
            if (first != second)
            {
                triggerFlags = TriggerFlags.Default;
                return true;
            }
            else
            {
                triggerFlags = TriggerFlags.None;
                return false;
            }
        }
        
        protected override int ApplyMaximum(int valueToModify, int maximum)
        {
            if (valueToModify > maximum)
            {
                return maximum;
            }
            else return valueToModify;
        }
        
        protected override int ApplyMinimum(int valueToModify, int minimum)
        {
            if (valueToModify < minimum)
            {
                return minimum;
            }
            else return valueToModify;        
        }
        
        public IntControllerModCore(int initialBaseValue, IModTypeOrder modOrder) : base(initialBaseValue, modOrder)
        {
        }

        public IntControllerModCore(int initialBaseValue = default) : base(initialBaseValue)
        {
        }
    }
    
    
    public class UIntControllerModCore : ValueControllerModCore<uint>

    {
        protected override bool ValuesAreDifferent(uint first, uint second, out long triggerFlags)
        {
            if (first != second)
            {
                triggerFlags = TriggerFlags.Default;
                return true;
            }
            else
            {
                triggerFlags = TriggerFlags.None;
                return false;
            }
        }

        protected override uint Multiply(uint valueToModify, double multiplier) => (uint) (valueToModify * multiplier);

        protected override uint Add(uint valueToModify, uint amountToAdd) => valueToModify + amountToAdd;
        
        protected override uint ApplyMaximum(uint valueToModify, uint maximum)
        {
            if (valueToModify > maximum)
            {
                return maximum;
            }
            else return valueToModify;
        }
        
        protected override uint ApplyMinimum(uint valueToModify, uint minimum)
        {
            if (valueToModify < minimum)
            {
                return minimum;
            }
            else return valueToModify;        
        }
        
        public UIntControllerModCore(uint initialBaseValue, IModTypeOrder modOrder) : base(initialBaseValue, modOrder)
        {
        }

        public UIntControllerModCore(uint initialBaseValue = default) : base(initialBaseValue)
        {
        }
    }
    
    
    public class TimeSpanControllerModCore : ValueControllerModCore<TimeSpan>
    {
        protected override bool ValuesAreDifferent(TimeSpan first, TimeSpan second, out long triggerFlags)
        {
            if (first != second)
            {
                triggerFlags = TriggerFlags.Default;
                return true;
            }
            else
            {
                triggerFlags = TriggerFlags.None;
                return false;
            }
        }

        protected override TimeSpan Multiply(TimeSpan valueToModify, double multiplier) => 
            new TimeSpan((long)(valueToModify.Ticks * multiplier));
        
        protected override TimeSpan Add(TimeSpan valueToModify, TimeSpan amountToAdd) => 
            valueToModify + amountToAdd;
        
        protected override TimeSpan ApplyMaximum(TimeSpan valueToModify, TimeSpan maximum)
        {
            if (valueToModify > maximum)
            {
                return maximum;
            }
            else return valueToModify;
        }
        
        protected override TimeSpan ApplyMinimum(TimeSpan valueToModify, TimeSpan minimum)
        {
            if (valueToModify < minimum)
            {
                return minimum;
            }
            else return valueToModify;        
        }
        
        public TimeSpanControllerModCore(TimeSpan initialBaseValue, IModTypeOrder modOrder) : 
            base(initialBaseValue, modOrder)
        {
        }

        public TimeSpanControllerModCore(TimeSpan initialBaseValue = default) : base(initialBaseValue)
        {
        }
    }
}