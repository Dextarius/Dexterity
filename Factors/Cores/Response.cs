using System;
using Core.Factors;
using Factors.Cores.DirectReactorCores;

namespace Factors.Cores
{
    public abstract class DirectResponse : DirectReactorCore
    {
        protected override bool CreateOutcome()
        {
            bool wasSuccessful = ExecuteResponse();
           
            SubscribeToInputs();

            return wasSuccessful;
        }

        protected abstract bool ExecuteResponse();
    }
}