using Core.States;

namespace Core.Factors
{
    public interface IFactor : IPrioritizable, INameable, INecessary 
    {
        bool HasSubscribers      { get; }
        int  NumberOfSubscribers { get; }
        
        void TriggerSubscribers();
        bool Subscribe(IFactorSubscriber subscriberToAdd);
        void Unsubscribe(IFactorSubscriber subscriberToRemove);
        bool Reconcile();
    }

    public interface IFactor<out T> : IFactor
    {
        T Value { get; }

        T Peek();   
    }
}