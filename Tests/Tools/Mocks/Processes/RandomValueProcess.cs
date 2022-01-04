using Core.Causality;
using Tests.Tools.Interfaces;

namespace Tests.Tools.Mocks.Processes
{
    public class RandomValueProcess<T> : IProcess<T>
    {
        private IRandomGenerator<T> valueGenerator;
        private IProcess            innerProcess;
        private T                   previousValue;

        public T Execute()
        {
            var randomValue = valueGenerator.CreateRandomValueNotEqualTo(previousValue);

            innerProcess?.Execute();
            previousValue = randomValue;

            return randomValue;
        }
        
        public RandomValueProcess(IRandomGenerator<T> generatorToUse, IProcess innerProcessToUse = null)
        {
            valueGenerator = generatorToUse;
            innerProcess   = innerProcessToUse;
        }
    }
}