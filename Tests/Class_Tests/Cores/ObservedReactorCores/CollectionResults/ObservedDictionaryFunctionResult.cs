using System;
using System.Collections.Generic;
using Factors.Collections;
using Factors.Cores.ObservedReactorCores.CollectionResults;
using NUnit.Framework;

namespace Tests.Class_Tests.Cores.ObservedReactorCores.CollectionResults
{
    public class ObservedDictionaryFunctionResults
    {
        [Test]
        public void WhenGivenAProcessThatReturnsACollection_ContainsAllOfThoseElements()
        {
            int[]                                       keyNumbers         = Tools.Tools.CreateRandomSizedArrayOfRandomNumbers();
            int[]                                       valueNumbers       = Tools.Tools.CreateArrayOfRandomNumbers(keyNumbers.Length);
            Func< IEnumerable< KeyValuePair<int, int>>> collectionFunction = CreateKeyValuePairs;
            var                                         dictionaryCore     = new ObservedDictionaryFunctionResult<int, int>(collectionFunction);
            
            Assert.That(valueNumbers.Length  == keyNumbers.Length);
            Assert.That(dictionaryCore.Count == keyNumbers.Length);
            
            for (int i = 0; i < keyNumbers.Length; i++)
            {
                int key           = keyNumbers[i];
                int originalValue = valueNumbers[i];
                int actualValue   = dictionaryCore[key];
                
                TestContext.WriteLine(
                    $"Key => {key, 13}, Original Value => {originalValue, 13}, Actual Value {actualValue, 13}");
                TestContext.WriteLine();
                Assert.That(originalValue, Is.EqualTo(actualValue), 
                    $"One of the elements in the {nameof(ReactiveList<int>)} did not match the value at the same index of" +
                     " its source. ");
            }


            #region Local Functions

            IEnumerable< KeyValuePair<int, int>> CreateKeyValuePairs()
            {
                for (int i = 0; i < keyNumbers.Length; i++)
                {
                    int key   = keyNumbers[i];
                    int value = valueNumbers[i];

                    yield return new KeyValuePair<int, int>(key, value);
                }
            }

            #endregion
        }
        


    }
}