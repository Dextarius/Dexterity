using Core.States;

namespace Causality
{
    public interface IUpdateList
    {
        bool                        IsUpdating { get; }
        UpdateList.UpdateQueueToken QueueUpdates();
        void                        Update(IUpdateable updateable, int priority);
        void                        Update<TUpdate>(TUpdate updateable)  where TUpdate : IUpdateable, IPrioritizable;
    }
}