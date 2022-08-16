using Core.Factors;
using Core.States;

namespace DirectFactors
{
    public interface IDirectFactor<out T> : IDirectFactor, IValue<T>, INecessary
    {
        bool Subscribe(IFactorSubscriber<T> subscriberToAdd, bool isNecessary);
        void Unsubscribe(IFactorSubscriber<T> subscriberToRemove);
        void NotifyNecessary(IFactorSubscriber<T> necessarySubscriber);
        void NotifyNotNecessary(IFactorSubscriber<T> unnecessarySubscriber);
    }

    public interface IDirectFactor
    {
        bool Reconcile();
    }
}