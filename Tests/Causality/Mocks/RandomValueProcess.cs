using Core.Causality;
using Tests.Causality.Interfaces;

namespace Tests.Causality.Mocks
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