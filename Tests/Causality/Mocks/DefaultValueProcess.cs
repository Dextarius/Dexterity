using Core.Causality;

namespace Tests.Causality.Mocks
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