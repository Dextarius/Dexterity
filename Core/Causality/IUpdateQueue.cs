using System;

namespace Core.Causality
{
    public interface IUpdateQueue : IQueue
    {
        void RunUpdates();
    }
}