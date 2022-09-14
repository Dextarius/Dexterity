using System;
using Core.Factors;
using Factors.Cores.DirectReactorCores;

namespace Factors.Cores
{
    public abstract class DirectResponse : DirectReactorCore
    {
        protected override long CreateOutcome()
        {
            long triggerFlags = ExecuteResponse();
           
            SubscribeToInputs();

            return triggerFlags;
        }

        protected abstract long ExecuteResponse();
    }
    

}