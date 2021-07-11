using System;
using Core.Causality;
using Core.Factors;
using JetBrains.Annotations;

namespace Causality.Processes
{
    public class ActionProcess : IProcess
    {
        [NotNull]
        private readonly Action actionToTake;

        public void Execute() => actionToTake();

        public ActionProcess(Action actionToExecute)
        {
            actionToTake = actionToExecute?? throw new ArgumentNullException(nameof(actionToExecute));
        }
    }
}