using System;

namespace Core.States
{
    public interface IFactorSubscriber : ITriggerable, IDestabilizable
    {
        // void UpdatePriorityChanged(IFactor changedFactor)
    }
}