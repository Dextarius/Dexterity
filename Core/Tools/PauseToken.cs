using System;
using Core.States;

namespace Core.Tools
{
    public struct PauseToken : IDisposable
    {
        public readonly IPausable pausedObject;
        public          bool      wasUnpaused;
            
        public void Dispose()
        {
            if (wasUnpaused)
            {
                throw new InvalidOperationException($"{nameof(PauseToken)} was unpaused twice. ");
            }
            
            pausedObject?.Unpause();
            wasUnpaused = true;
        }

        public PauseToken(IPausable pausable)
        {
            pausedObject = pausable;
            wasUnpaused  = false;
        }
    }
}