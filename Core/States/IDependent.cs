using System;

namespace Core.States
{
    public interface IDependent : INecessary, IInvalidatable, IDestabilizable
    {
        WeakReference<IDependent> WeakReference { get; }
    }
}