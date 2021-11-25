using System;

namespace Core.States
{
    public interface IWeaklyDependable<TDependent>  where TDependent : class
    {
        bool HasDependents { get; }

        bool     AddDependent(WeakReference<TDependent> dependent);
        void ReleaseDependent(WeakReference<TDependent> dependent);
    }
}