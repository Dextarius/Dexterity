using System;

namespace Core.Causality
{
    public interface IUpdateQueue
    {
        Action EndQueue();
        void Dispose();
    }
}