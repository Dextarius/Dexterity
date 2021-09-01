using System;

namespace Causality
{
    public struct PauseToken : IDisposable
    {
        public void Dispose()
        {
            Observer.ResumeObservation();
        }
        
        //- If we ever change the Observer to be fully instance based,
        //  we'll have to add a field to this to access that instance.
    }
}