using Core.Causality;

namespace Tests.Tools.Mocks.Processes
{
    public class DefaultValueProcess<T> : IProcess<T>
    {
        private IProcess innerProcess;
        
        public T Execute()
        {
            innerProcess?.Execute();
            return default(T);
        }
        
        public DefaultValueProcess(IProcess innerProcessToUse = null)
        {
            innerProcess = innerProcessToUse;
        }
    }
}