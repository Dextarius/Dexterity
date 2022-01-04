using Factors.Collections;
using NUnit.Framework;

namespace Tests
{
    public class ReactiveLists
    {
        [Test]
        public void WhenGivenAProcessThatReturnsACollection_ContainsAllOfThoseElements()
        {
            int[]             numbers         = Tools.Tools.CreateArrayOfRandomNumbers();
            ReactiveList<int> listBeingTested = new ReactiveList<int>(() => numbers);

            Assert.That(numbers.Length == listBeingTested.Count);
            
            for (int i = 0; i < numbers.Length; i++)
            {
                int sourceValue   = numbers[i];
                int reactiveValue = listBeingTested[i];
                
                TestContext.WriteLine($"Source Value   => {sourceValue, 13}");
                TestContext.WriteLine($"Reactive Value => {reactiveValue, 13}");
                TestContext.WriteLine();
                Assert.That(sourceValue, Is.EqualTo(reactiveValue), 
                    $"One of the elements in the {nameof(ReactiveList<int>)} did not match the value at the same index of it's source. ");
            }
        }
    }


}