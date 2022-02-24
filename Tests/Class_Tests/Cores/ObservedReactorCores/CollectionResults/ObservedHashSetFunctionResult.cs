using Factors.Cores.ObservedReactorCores.CollectionResults;
using NUnit.Framework;

namespace Tests.Class_Tests.Cores.ObservedReactorCores.CollectionResults
{
    public class ObservedHashSetFunctionResults
    {
        [Test]
        public void WhenGivenAProcessThatReturnsACollection_ContainsAllOfThoseElements()
        {
            int[] numbers        = Tools.Tools.CreateRandomSizedArrayOfRandomNumbers();
            var   setBeingTested = new ObservedHashSetFunctionResult<int>(() => numbers);

            Assert.That(numbers.Length == setBeingTested.Count);

            foreach (int value in numbers)
            {
                Assert.That(setBeingTested.Contains(value), Is.True,
                    $"The {nameof(ObservedHashSetFunctionResult<int>)} did not contain on of the values from its source. ");
            }
        }
    }
}