using System;
using System.Collections.Generic;
using Core.Factors;
using static Core.Tools.Numerics;

namespace Factors.Cores
{
    public class ModifiableDoubleCore : ModifiableCore<double>
    {
        protected override bool   ValuesAreDifferent(double first, double second)   => DoublesAreNotEqual(first, second);
        protected override double Multiply(double valueToModify, double multiplier) => valueToModify * multiplier;
        protected override double Add(double valueToModify, double amountToAdd)     => valueToModify + amountToAdd;
        
        public ModifiableDoubleCore(double initialBaseValue, IModTypeOrder modOrder) : 
            base(initialBaseValue, modOrder)
        {

        }

        public ModifiableDoubleCore(double initialBaseValue = default) : base(initialBaseValue)
        {
            
        }
    }
    
    public class ModifiableIntCore : ModifiableCore<int>
    {
        protected override bool ValuesAreDifferent(int first, int second)      => first != second;
        protected override int  Multiply(int valueToModify, double multiplier) => (int)(valueToModify * multiplier);
        protected override int  Add(int valueToModify, double amountToAdd)     => (int)(valueToModify + amountToAdd);
        
        public ModifiableIntCore(int initialBaseValue, IModTypeOrder modOrder) : 
            base(initialBaseValue, modOrder)
        {

        }

        public ModifiableIntCore(int initialBaseValue = default) : base(initialBaseValue)
        {
            
        }
    }
    
    public class ModifiableUintCore : ModifiableCore<uint>
    {
        protected override bool ValuesAreDifferent(uint first, uint second)     => first != second;
        protected override uint Multiply(uint valueToModify, double multiplier) => (uint)(valueToModify * multiplier);
        protected override uint Add(uint valueToModify, double amountToAdd)     => (uint)(valueToModify + amountToAdd);
        
        public ModifiableUintCore(uint initialBaseValue, IModTypeOrder modOrder) : 
            base(initialBaseValue, modOrder)
        {

        }

        public ModifiableUintCore(uint initialBaseValue = default) : base(initialBaseValue)
        {
            
        }
    }
    
    public class ModifiableTimeSpanCore : ModifiableCore<TimeSpan>
    {
        protected override bool     ValuesAreDifferent(TimeSpan first, TimeSpan second) => first != second;
        protected override TimeSpan Multiply(TimeSpan valueToModify, double multiplier) => valueToModify * multiplier;
        protected override TimeSpan Add(TimeSpan valueToModify, double amountToAdd)     => 
            valueToModify + TimeSpan.FromMilliseconds(amountToAdd);
        
        public ModifiableTimeSpanCore(TimeSpan initialBaseValue, IModTypeOrder modOrder) : 
            base(initialBaseValue, modOrder)
        {

        }

        public ModifiableTimeSpanCore(TimeSpan initialBaseValue = default) : base(initialBaseValue)
        {
            
        }
    }
}