using System;

namespace Core.States
{
    public interface IFactorSubscriber : INecessary, ITriggerable, IDestabilizable
    {
        WeakReference<IFactorSubscriber> WeakReference { get; }
    }
}