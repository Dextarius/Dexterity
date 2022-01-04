using System.Runtime.CompilerServices;
using Core.Causality;
using Core.Factors;

namespace Tests.Tools.Mocks.Processes
{
    public class PeekValueProcess<T> : IProcess, IProcess<T>
    {
        private readonly IFactor<T> factor;
        
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public T Execute()
        {
            var value = factor.Peek();

            return value;
        }

        public PeekValueProcess(IFactor<T> factorWithValue)
        {
            factor = factorWithValue;
        }

        void IProcess.Execute() => Execute();
    }
}