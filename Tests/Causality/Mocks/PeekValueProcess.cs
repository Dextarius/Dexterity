using System.Runtime.CompilerServices;
using Core.Causality;
using Core.States;

namespace Tests.Causality.Mocks
{
    public class PeekValueProcess<T> : IProcess, IProcess<T>
    {
        private IState<T> factor;
        
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public T Execute()
        {
            var value = factor.Peek();

            return value;
        }

        public PeekValueProcess(IState<T> factorWithValue)
        {
            factor = factorWithValue;
        }

        void IProcess.Execute() => Execute();
    }
}