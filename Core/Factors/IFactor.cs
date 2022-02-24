using Core.Redirection;
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
        void NotifyNecessary();
        void NotifyNotNecessary();
        
        //- Most of the Factor's will throw an error if they have no subscribers when NotifyNecessary() is called,
        //  because NotifyNecessary is supposed to communicate that one of its subscribers relies on it.
        //  Perhaps it's a mistake in the interface that non-subscribers can call the method.  
        //- TODO : Consider making Subscribe() return some sort of 'subscription' object and put
        //         the method on there instead.  This may make unsubscribing simpler as well.
    }

    public interface IFactor<out T> : IFactor, IValue<T>
    {
        T Peek();   
    }
}