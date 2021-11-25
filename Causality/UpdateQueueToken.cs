using System;
using Causality.States;
using Core.Tools;

namespace Causality
{
    public partial class UpdateList : IUpdateList
    {
        public class UpdateQueueToken : IDisposable 
        {
            private readonly bool       callerHasOwnershipOfQueue;
            private readonly UpdateList updateQueue;
            
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
                        updateQueue.StopQueueingUpdates();
                    }
                }
            }
            
            internal UpdateQueueToken(bool callerHasOwnership, UpdateList queue)
            {
                callerHasOwnershipOfQueue = callerHasOwnership;
                updateQueue = queue;
            }
        }
    }
}