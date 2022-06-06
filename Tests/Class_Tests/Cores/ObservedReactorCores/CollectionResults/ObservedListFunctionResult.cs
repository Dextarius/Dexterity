using System.Collections.Generic;
using Factors.Collections;
using Factors.Cores.ObservedReactorCores.CollectionResults;
using NUnit.Framework;

namespace Tests.Class_Tests.Cores.ObservedReactorCores.CollectionResults
{
    public class ObservedListFunctionResults
    {
        [Test]
        public void WhenGivenAProcessThatReturnsACollection_ContainsAllOfThoseElements()
        {
            int[] numbers         = Tools.Tools.CreateRandomSizedArrayOfRandomNumbers();
            var   coreBeingTested = new ObservedListFunctionResult<int>(() => numbers);
            var   list            = new ReactiveList<int>(coreBeingTested);

            Assert.That(numbers.Length == list.Count);

            for (int i = 0; i < numbers.Length; i++)
            {
                int originalValue = numbers[i];
                int actualValue   = list[i];

                TestContext.WriteLine($"Original Value => {originalValue,13}");
                TestContext.WriteLine($"Actual Value   => {actualValue,13}");
                TestContext.WriteLine();
                Assert.That(originalValue, Is.EqualTo(actualValue),
                    $"One of the elements in the {nameof(ObservedListFunctionResult<int>)} did not " +
                     "match the value at the same index of its source. ");
            }
        }
    }
}