using System.Collections.Generic;
using Core.Factors;

namespace Core.States
{
    public interface IAggregator<T> 
    {
        bool HasInputs      { get; }
        bool NumberOfInputs { get; }
        
        bool Include(IFactor<T> factorToInclude);
        bool Remove(IFactor<T> factorToRemove);        
        void IncludeAll(IEnumerable<IFactor<T>> factorsToInclude);
        void RemoveAll(IEnumerable<IFactor<T>> factorsToRemove);
    }
}