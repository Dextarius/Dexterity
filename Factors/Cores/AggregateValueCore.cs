using System;
using Core.Factors;
using Core.States;

namespace Factors.Cores
{
    //- DistilledValue?
    public abstract class AggregateValueCore<TValue, TFactor> : AggregatorCore<TValue, TFactor>
        where TFactor : IFactor<TValue>
    {
        private TValue baseValue;
        
        public TValue BaseValue
        {
            get => baseValue;
            set
            {
                baseValue = value;
                Trigger();
            }
        }
    }
}