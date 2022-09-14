using Core.States;

namespace Core.Factors
{
    public interface IInfluence
    {
        bool HasSubscribers               { get; }
        int  NumberOfSubscribers          { get; }
        bool HasNecessarySubscribers      { get; }
        int  NumberOfNecessarySubscribers { get; }

        void TriggerSubscribers(IFactor triggeringFactor, long triggerFlags);
        bool DestabilizeSubscribers(IFactor triggeringFactor);
        bool Subscribe(IFactorSubscriber subscriberToAdd, bool isNecessary);
        bool Unsubscribe(IFactorSubscriber subscriberToRemove);
        void NotifyNecessary(IFactorSubscriber necessarySubscriber);
        void NotifyNotNecessary(IFactorSubscriber unnecessarySubscriber);
    }
}