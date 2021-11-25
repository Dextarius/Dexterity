using System.Runtime.CompilerServices;
using Core.Causality;
using Core.States;

namespace Tests.Causality.Mocks
{
    public class RetrieveValueProcess<T> : IProcess
    {
        private IState<T> factor;
        
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void Execute()
        {
            var value = factor.Value;
        }

        public RetrieveValueProcess(IState<T> factorWithValue)
        {
            factor = factorWithValue;
        }
    }
}