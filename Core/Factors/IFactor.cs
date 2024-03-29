﻿using Core.Redirection;
using Core.States;

namespace Core.Factors
{
    public interface IFactor : INameable, IPrioritizedUpdate, IVersioned
    {
        bool Subscribe(IFactorSubscriber subscriberToAdd, bool isNecessary);
        void Unsubscribe(IFactorSubscriber subscriberToRemove);
        void NotifyNecessary(IFactorSubscriber necessarySubscriber);
        void NotifyNotNecessary(IFactorSubscriber unnecessarySubscriber);
        bool Reconcile();
        
        //- Most of the Factor's will throw an error if they have no subscribers when NotifyNecessary() is called,
        //  because NotifyNecessary is supposed to communicate that one of its subscribers relies on it.
        //  Perhaps it's a mistake for the interface to allow non-subscribers to call the method.  
        //- TODO : Consider making Subscribe() return some sort of 'subscription' object and put
        //         the method on there instead.  This may make unsubscribing simpler as well.
    }

    public interface IFactor<T> : IFactor,  IValue<T>, IValueEquatable<T>
    {
        T Peek();
    }
}