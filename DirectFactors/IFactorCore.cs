using Core.Factors;
using Core.States;

namespace DirectFactors
{
    public interface IFactorCore<T> : IDisposable, IVersioned, IPrioritizedUpdate
    {
        
    }
}