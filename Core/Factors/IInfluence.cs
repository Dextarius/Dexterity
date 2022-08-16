using Core.States;

namespace Core.Factors
{
    public interface IInfluence
    {
        bool HasSubscribers          { get; }
        bool HasNecessarySubscribers { get; }
        int  NumberOfSubscribers     { get; }
        
        void TriggerSubscribers(IFactor triggeringFactor);
        bool DestabilizeSubscribers(IFactor triggeringFactor);
        bool Subscribe(IFactorSubscriber subscriberToAdd, bool isNecessary);
        bool Unsubscribe(IFactorSubscriber subscriberToRemove);
        void NotifyNecessary(IFactorSubscriber necessarySubscriber);
        void NotifyNotNecessary(IFactorSubscriber unnecessarySubscriber);
    }
}