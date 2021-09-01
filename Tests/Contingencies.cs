using Factors;
using NUnit.Framework;
using static Tests.Tools;

namespace Tests
{
    public class Contingencies
    {
        [SetUp]
        public void Setup()
        {
        }
        
        [Test]
        public void WhenConstructed_ValueMatchesProcessOutput([Values(true, false)] bool valueToTest)
        {
            Proactive<bool> sourceValue     = new Proactive<bool>(valueToTest);
            Contingency     testContingency = new Contingency(sourceValue, DoNothing);
            
            Assert.That(sourceValue.Value,     Is.EqualTo(valueToTest));
            Assert.That(testContingency.Value, Is.EqualTo(valueToTest));
        }
        
        
        [Test]
        public void WhenDependencyChanges_ValueChanges([Values(true, false)] bool initialValue)
        {
            Proactive<bool> sourceValue     = new Proactive<bool>(initialValue);
            Contingency     testContingency = new Contingency(sourceValue, DoNothing);
            bool            changedValue    = !initialValue;
            
            Assert.That(sourceValue.Value,     Is.EqualTo(initialValue));
            Assert.That(testContingency.Value, Is.EqualTo(initialValue));

            sourceValue.Value = changedValue;

            Assert.That(sourceValue.Value,     Is.EqualTo(changedValue));
            Assert.That(testContingency.Value, Is.EqualTo(changedValue));
        }
        
        [Test]
        public void WhenValueIsFalse_DoesNotExecutesProcess([Random(1)] int initialValue)
        {
            int             testValue                       = initialValue;
            Proactive<bool> shouldExecute                   = new Proactive<bool>(false);
            Contingency     ifShouldExecute_ChangeTestValue = new Contingency(shouldExecute, ChangeTestValue);

            Assert.That(ifShouldExecute_ChangeTestValue.Value, Is.False);
            Assert.That(shouldExecute.Value,                   Is.False);

            ifShouldExecute_ChangeTestValue.TryExecute();
            
            Assert.That(testValue, Is.EqualTo(initialValue));

            return;

            
            void ChangeTestValue() => testValue++;
        }
        
        [Test]
        public void WhenValueIsTrue_ExecutesProcess([Random(1)] int initialValue, [Random(1)] int successValue)
        {
            if (successValue == initialValue) { successValue++; }
            
            int             testValue                    = initialValue;
            Proactive<bool> shouldExecute                = new Proactive<bool>(false);
            Contingency     ifShouldExecute_SetTestValue = new Contingency(shouldExecute, SetTestValue);

            Assert.That(ifShouldExecute_SetTestValue.Value, Is.False);
            Assert.That(shouldExecute.Value,                Is.False);
            Assert.That(testValue,                          Is.EqualTo(initialValue));
            
            shouldExecute.Value = true;
            
            Assert.That(ifShouldExecute_SetTestValue.Value, Is.True);
            Assert.That(shouldExecute.Value,                Is.True);

            ifShouldExecute_SetTestValue.TryExecute();
            
            Assert.That(testValue, Is.EqualTo(successValue));

            return;

            
            void SetTestValue() => testValue = successValue;
        }
        
        
        [Test]
        public void WhenNotImpulsive_DoesNotExecuteProcessUntilRequested([Random(1)] int initialValue, [Random(1)] int changedValue)
        {
            if (changedValue == initialValue) { changedValue++; }
            
            int             testValue                    = initialValue;
            Proactive<bool> shouldExecute                = new Proactive<bool>(false);
            Contingency     ifShouldExecute_SetTestValue = new Contingency(shouldExecute, SetTestValue);

            Assert.That(ifShouldExecute_SetTestValue.IsImpulsive, Is.False);
            Assert.That(ifShouldExecute_SetTestValue.Value,       Is.False);
            Assert.That(shouldExecute.Value,                      Is.False);
            Assert.That(testValue,                                Is.EqualTo(initialValue));
            
            shouldExecute.Value = true;
            
            Assert.That(ifShouldExecute_SetTestValue.Value, Is.True);
            Assert.That(shouldExecute.Value,                Is.True);
            Assert.That(testValue,                          Is.EqualTo(initialValue));

            return;

            
            void SetTestValue() => testValue = changedValue;
        }
        
        //[Test]
        public void WhenImpulsive_ExecutesProcessAutomatically([Random(1)] int initialValue, [Random(1)] int changedValue)
        {
            if (changedValue == initialValue) { changedValue++; }
            
            int             testValue                    = initialValue;
            Proactive<bool> shouldExecute                = new Proactive<bool>(false);
            Contingency     ifShouldExecute_SetTestValue = new Contingency(shouldExecute, SetTestValue);

            Assert.That(ifShouldExecute_SetTestValue.IsImpulsive, Is.False);
            Assert.That(ifShouldExecute_SetTestValue.Value,       Is.False);
            Assert.That(shouldExecute.Value,                      Is.False);
            Assert.That(testValue,                                Is.EqualTo(initialValue));
            
            shouldExecute.Value = true;
            
            Assert.That(ifShouldExecute_SetTestValue.Value, Is.True);
            Assert.That(shouldExecute.Value,                Is.True);
            Assert.That(testValue,                          Is.EqualTo(initialValue));

            return;

            
            void SetTestValue() => testValue = changedValue;
        }
        
        
    }
}