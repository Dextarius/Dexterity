using System;

namespace Core.States
{
    public interface IFactorSubscriber : INecessary, ITriggerable, IDestabilizable
    {
    }
}