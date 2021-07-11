using System;

namespace Causality
{
    public struct PauseToken : IDisposable
    {
        public void Dispose()
        {
            Observer.ResumeObservation();
        }
    }
}