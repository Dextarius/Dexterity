using Core.Factors;

namespace Core.States
{
    public interface ITriggerable
    {
        bool Trigger();
       // bool Trigger(IFactor triggeringFactor, out bool removeSubscription);
        bool Trigger(IFactor triggeringFactor, long triggerFlags, out bool removeSubscription);
    }
    
    public interface IMultiTriggerable
    {
        bool Trigger();
    }
}