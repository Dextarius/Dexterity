using System;
using System.Collections.Generic;
using Core.Factors;
using Factors.Cores.DirectReactorCores;

namespace Factors.Cores
{
    public abstract class AggregatorCore<TValue, TFactor> : DirectResult<TValue>
        where TFactor : IFactor
    {
        protected HashSet<TFactor> inputFactors = new HashSet<TFactor>();
        
        public override int NumberOfTriggers => inputFactors.Count;
        
        protected override IEnumerable<IFactor> Triggers
        {
            get
            {
                foreach (var inputValue in inputFactors)
                {
                    yield return inputValue;
                }
            }
        }
        
        public bool Include(TFactor factorToInclude)
        {
            if (factorToInclude is null) { throw new ArgumentNullException(nameof(factorToInclude)); }

            if (inputFactors.Add(factorToInclude))
            {
                AddTrigger(factorToInclude, false);
                Trigger();
                return true;
            }
            else return false;
        }
        
        public bool Remove(TFactor factorToRemove)
        {
            if (factorToRemove != null &&
                inputFactors.Remove(factorToRemove))
            {
                RemoveTrigger(factorToRemove);
                Trigger();
                return true;
            }
            else return false;
        }
        
        // protected abstract void OnInputAdded(TFactor factorAdded);
        // protected abstract void OnInputRemoved(TFactor factorRemoved);
    }
}