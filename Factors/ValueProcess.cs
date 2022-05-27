using Core.Causality;
using Core.Factors;
using Core.Redirection;

namespace Factors
{
    public class ValueProcess<T> : IProcess<T>
    {
        private IValue<T> source;

        public T Execute() => source.Value;

        public ValueProcess(IValue<T> source)
        {
            this.source = source;

        }
    }
}