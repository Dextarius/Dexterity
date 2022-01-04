using System.Runtime.CompilerServices;
using Core.Causality;
using Core.Factors;

namespace Tests.Tools.Mocks.Processes
{
    public class RetrieveValueProcess<T> : IProcess
    {
        private IFactor<T> factor;

        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void Execute()
        {
            var value = factor.Value;
        }

        public RetrieveValueProcess(IFactor<T> factorWithValue)
        {
            factor = factorWithValue;
        }
    }
}