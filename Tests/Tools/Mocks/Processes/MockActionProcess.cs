using System;
using Core.Causality;
using JetBrains.Annotations;

namespace Tests.Tools.Mocks.Processes
{
    public class MockActionProcess : IProcess
    {
        [NotNull]
        protected readonly Action actionToExecute;

        public void Execute() => actionToExecute();
        
        public MockActionProcess(Action action)
        {
            actionToExecute = action;
        }
    }
}