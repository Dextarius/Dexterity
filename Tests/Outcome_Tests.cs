using System;
using Core.Causality;
using Core.Factors;
using Core.States;
using NUnit.Framework;
using Tests.Tools.Mocks;

namespace Tests
{
    public class Outcome_Tests<TOutcomeFactory> 
        where TOutcomeFactory : new()
    {
        private TOutcomeFactory resultFactory = new TOutcomeFactory();

        
        [Test]
        public void WhenCreated_IsBeingInfluencedIsFalse()
        {
            IOutcome resultBeingTested = resultFactory.CreateInstance();

            Assert.That(resultBeingTested.HasTriggers, Is.False);
        }
        
        [Test]
        public void WhenCreated_NumberOfInfluencesIsZero()
        {
            IOutcome resultBeingTested = resultFactory.CreateInstance();

            Assert.That(resultBeingTested.NumberOfTriggers, Is.Zero);
        }
        
        [Test]
        public void WhenConstructed_IsInvalid()
        {
            IProcess process  = new ActionProcess(DoNothing);
            IOutcome result1 = resultFactory.CreateInstance();
            IOutcome result2 = resultFactory.CreateInstance_WhoseUpdateCalls(process);
            IOutcome result3 = resultFactory.CreateInstance_WhoseUpdateInvolves(result1);

            Assert.That(result1.IsValid, Is.False);
            Assert.That(result2.IsValid, Is.False);
            Assert.That(result3.IsValid, Is.False);
        }
        
        [Test] //- This one is mostly to make sure that when we queue updates, those updates are actually run.
        public void WhenFactorInvalidatesDependents_DoesNotPreventUpdatesFromExecuting()
        {
            IFactor factorBeingTested = factory.CreateInstance();
            var     dependent         = new MockFactorSubscriber();

            dependent.MakeValid();
            dependent.MakeNecessary();
            factorBeingTested.Subscribe(dependent);
            
            Assert.That(dependent.IsValid, Is.True);

            factorBeingTested.TriggerSubscribers();

            Assert.That(dependent.IsValid,    Is.True);
            Assert.That(dependent.WasUpdated, Is.True);
        }
    }