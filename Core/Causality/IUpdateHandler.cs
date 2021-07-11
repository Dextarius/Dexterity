using System;

namespace Core.Causality
{
    public interface IUpdateHandler
    {
        void         RequestUpdate(Action requestedUpdate);
        IUpdateQueue RequestQueuing();
    }
}