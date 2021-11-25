using System;

namespace Core.States
{
    // public interface IInteraction : IDependent, IInfluenceable, IUpdateable, IPrioritizable, INecessary
    // {
    //     WeakReference<IInteraction> WeakReference { get; }
    // }
    
    public interface IDependency : IDependent, INecessary
    {
        WeakReference<IDependency> WeakReference { get; }
    }
}