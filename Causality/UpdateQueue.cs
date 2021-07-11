using System;
using Core.Causality;

namespace Causality
{
    public class UpdateQueue : IUpdateQueue
    {
        /// <summary>
        ///     A list of updates that have been requested since this queue was created.
        /// </summary>
        private Action queuedUpdates;

        internal void AddUpdate(Action updateToAdd) => queuedUpdates += updateToAdd;

        public Action EndQueue()
        {
            UpdateHandler.NotifyQueueEnded(this);
            
            return queuedUpdates;
        }

        public void Dispose()
        {
            queuedUpdates = null;
        }
    }
}