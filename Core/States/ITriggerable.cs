using Core.Factors;

namespace Core.States
{
    public interface ITriggerable
    {
        bool Trigger();
        bool Trigger(IFactor triggeringFactor);
    }
}