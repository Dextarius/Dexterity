using Core.States;

namespace Core.Factors
{
    public interface ITrigger 
    {
        bool Subscribe(IFactorSubscriber subscriberToAdd, bool isNecessary);
        void Unsubscribe(IFactorSubscriber subscriberToRemove);
    }
}