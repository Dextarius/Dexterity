using System;

namespace Core.Causality
{
    // public interface IUpdateQueue
    // {
    //     Action EndQueue();
    //     void Dispose();
    // }
    
    public interface IUpdateQueue : IQueue
    {
        void RunUpdates();
    }
}