using System;
using Core.Causality;

namespace Core
{
    public class DummyExecutionProvider : IUpdateExecutionProvider
    {
        public Action<Action> GetUpdateExecutionProcess() => ExecuteAction;

        private void ExecuteAction(Action actionToExecute) => actionToExecute?.Invoke();
    }
}