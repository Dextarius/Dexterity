using System;
using System.Collections.Generic;
using System.Linq;
using Core.Factors;
using Core.States;
using Factors.Cores.DirectReactorCores;

namespace Factors.Cores
{
    public class ModifiableCore<T> : DirectResult<T>
    {
        private T baseValue;

        
        public T BaseValue
        {
            get => baseValue;
            set
            {
                if (ValuesAreDifferent(baseValue, value, out _))
                {
                    baseValue = value;
                    Trigger();
                }
            }
        }


        public override bool HasTriggers      => NumberOfTriggers > 0;
        public override int  NumberOfTriggers => modifiers?.Count ?? 0;
        
        protected override IEnumerable<IFactor> Triggers
        {
            get
            {
                if (modifiers != null) { return modifiers; }
                else                   { return Enumerable.Empty<IFactor>(); }
            }
        }
        
        protected override T GenerateValue() => baseValue;

        public bool BaseValueEquals(T valueToCompare) => ValuesAreDifferent(baseValue, valueToCompare, out _) is false;

        public ModifiableCore(T initialValue)
        {
            baseValue = initialValue;
        }
    }
}