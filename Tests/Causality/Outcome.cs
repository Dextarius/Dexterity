using Causality;
using Causality.States;
using Core.Causality;
using Core.Factors;
using NUnit.Framework;
using static Tests.Tools;

namespace Tests
{
    public class Outcomes
    {
        [Test]
        public void WhenManuallyInvalidated_IsInvalid()
        {
            Outcome  outcomeToTest = new Outcome();
            
            Assert.That(outcomeToTest.IsValid);
            outcomeToTest.Invalidate();
            Assert.False(outcomeToTest.IsValid);
        }
        
        [Test]
        public void WhenParentStateIsInvalidated_OutcomeIsInvalidated()
        {
            State    involvedState = new State();
            Outcome  outcomeToTest = new Outcome();
            IProcess process       = GetProcessThatCreatesADependencyOn(involvedState);

            Observer.ObserveInteractions(process, outcomeToTest);
            Assert.That(outcomeToTest.IsValid);
            involvedState.Invalidate();
            Assert.False(outcomeToTest.IsValid);
        }
    }
}