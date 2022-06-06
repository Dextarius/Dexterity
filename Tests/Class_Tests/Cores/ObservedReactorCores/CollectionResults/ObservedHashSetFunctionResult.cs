using Factors.Collections;
using Factors.Cores.ObservedReactorCores.CollectionResults;
using NUnit.Framework;

namespace Tests.Class_Tests.Cores.ObservedReactorCores.CollectionResults
{
    public class ObservedHashSetFunctionResults
    {
        [Test]
        public void WhenGivenAProcessThatReturnsACollection_ContainsAllOfThoseElements()
        {
            int[] numbers         = Tools.Tools.CreateRandomSizedArrayOfRandomNumbers();
            var   coreBeingTested = new ObservedHashSetFunctionResult<int>(() => numbers);
            var   set             = new ReactiveSet<int>(coreBeingTested);

            Assert.That(numbers.Length == set.Count);

            foreach (int value in numbers)
            {
                Assert.That(set.Contains(value), Is.True,
                    $"The {nameof(ObservedHashSetFunctionResult<int>)} did not contain on of the values from its source. ");
            }
        }
    }
}