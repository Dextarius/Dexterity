using System;

namespace Core.Causality
{
    public readonly struct UpdateQueueToken : IDisposable 
    {
        private readonly IUpdateQueue updateQueue;
        private readonly bool         callerHasOwnershipOfQueue;

        public void Dispose()
        {
            if (callerHasOwnershipOfQueue)
            {
                try
                {
                    updateQueue.RunUpdates();
                }
                finally
                {
                    updateQueue.StopQueuing();
                }
            }
        }
            
        public UpdateQueueToken(bool callerHasOwnership, IUpdateQueue queue)
        {
            callerHasOwnershipOfQueue = callerHasOwnership;
            updateQueue = queue;
        }
    }
}