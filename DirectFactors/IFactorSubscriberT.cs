using Core.Factors;
using Core.States;

namespace DirectFactors
{
    public interface IFactorSubscriber<in T> : IFactorSubscriber
    {
        void ValueChanged(IDirectFactor<T> factor, T oldValue, T newValue, out bool removeSubscription);
    }
    
    public interface IFactorSubscriber : IDestabilizable
    {
        
    }
}