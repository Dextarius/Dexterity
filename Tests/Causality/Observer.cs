using System;
using NUnit.Framework;
using Causality;
using Causality.Processes;
using Causality.States;
using Core.Causality;
using Core.Factors;
using static Tests.Tools;


namespace Tests.Causality
{
    public class Observers
    {
        [Test]
        public void WhenNotifiedThatANullObjectIsInvolved_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => Observer.NotifyInvolved(null));
        }

        [Test]
        public void WhenNotifiedThatANullObjectHasChanged_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => Observer.NotifyChanged(null));
        }

        [Test]
        public void WhenObservationNotInProgress_IsCurrentlyObservingIsFalse()
        {
            Assert.That(Observer.IsCurrentlyObserving is false);
        }

        [Test]
        public void WhenObservationInProgress_IsCurrentlyObservingIsTrue()
        {
            bool          processWasExecuted = false;
            ActionProcess process            = new ActionProcess(AssertIsObserving);
            Outcome       testOutcome        = new Outcome();

            Observer.ObserveInteractions(process, testOutcome);
            Assert.That(processWasExecuted, $"The process that tests {nameof(Observer.IsCurrentlyObserving)} did not run. ");
            TestContext.WriteLine($"Process was Executed => {processWasExecuted}");
            

            void AssertIsObserving()
            {
                bool isObserving = Observer.IsCurrentlyObserving;

                Assert.That(isObserving);
                TestContext.WriteLine($"Is Observing => {isObserving}");
                processWasExecuted = true;
            }
        }

        [Test]
        public void WhenObservationIsPaused_IsCurrentlyObservingIsFalse()
        {
            bool          processWasExecuted = false;
            ActionProcess process            = new ActionProcess(AssertIsNotObserving);
            Outcome       testOutcome        = new Outcome();

            Observer.ObserveInteractions(process, testOutcome);
            Assert.That(processWasExecuted, $"The process that tests {nameof(Observer.IsCurrentlyObserving)} did not run. ");
            TestContext.WriteLine($"Process was Executed => {processWasExecuted}");
            

            void AssertIsNotObserving()
            {
                bool isObserving = true;

                using (Observer.PauseObservation())
                {
                    isObserving = Observer.IsCurrentlyObserving;
                }

                Assert.False(isObserving);
                TestContext.WriteLine($"Is Observing => {isObserving}");
                processWasExecuted = true;
            }
        }

        [Test]
        public void WhenNestedProcessIsFinishedBeingObserved_ObservationReturnsToOuterProcess()
        {
            int           testStarted            = 1;
            int           enteredOuterProcess    = 2;
            int           enteredInnerProcess    = 3;
            int           returnedToOuterProcess = 4;
            int           currentPhase           = 0;
            ActionProcess innerProcess           = new ActionProcess(InnerProcess);
            Outcome       innerOutcome           = new Outcome();
            Outcome       outerOutcome           = new Outcome();
            ActionProcess outerProcess           = new ActionProcess(OuterProcess);

            currentPhase = testStarted;
            Observer.ObserveInteractions(outerProcess, outerOutcome);
            Assert.That(currentPhase, Is.EqualTo(returnedToOuterProcess));


            void OuterProcess()
            {
                Assert.That(currentPhase, Is.EqualTo(testStarted));
                currentPhase = enteredOuterProcess;
                TestContext.WriteLine("Entered outer process. ");
                Observer.ObserveInteractions(innerProcess, innerOutcome);
                Assert.That(currentPhase, Is.EqualTo(enteredInnerProcess));
                currentPhase = returnedToOuterProcess;
                TestContext.WriteLine("Returned to outer process. ");
            }
            
            void InnerProcess()
            {
                Assert.That(currentPhase, Is.EqualTo(enteredOuterProcess));
                currentPhase = enteredInnerProcess;
                TestContext.WriteLine("Entered inner process. ");
            }
        }

        [Test]
        public void WhenAStateNotifiesItsInvolvedDuringObservation_OutcomeIsDependentOnState()
        {
            State         involvedState = new State();
            Outcome       outcomeToTest = new Outcome();
            ActionProcess process       = new ActionProcess(InvolveState);

            Assert.False(outcomeToTest.IsBeingAffected);
            Assert.False(involvedState.IsConsequential);
            
            Observer.ObserveInteractions(process, outcomeToTest);
            
            Assert.That(outcomeToTest.IsBeingAffected);
            Assert.That(involvedState.IsConsequential);
            

            void InvolveState()
            {
                Observer.NotifyInvolved(involvedState);
            }
        }

        [Test]
        public void IfStateIsInvalidWhenNotifyingItsInvolved_NoDependencyIsCreated()
        {
            State    stateToTest = new State();
            Outcome  outcome     = new Outcome();
            IProcess process     = GetProcessThatCreatesADependencyOn(stateToTest);

            stateToTest.Invalidate();
            Assert.False(outcome.IsBeingAffected);
            Assert.False(stateToTest.IsConsequential);

            Observer.ObserveInteractions(process, outcome);

            Assert.False(outcome.IsBeingAffected);
            Assert.False(stateToTest.IsConsequential);
        }
        
        [Test]
        public void IfOutcomeIsAlreadyInvalidWhenAStateNotifiesItsInvolved_NoDependencyIsCreated()
        {
            State    involvedState = new State();
            Outcome  outcomeToTest = new Outcome();
            IProcess process       = GetProcessThatCreatesADependencyOn(involvedState);

            outcomeToTest.Invalidate();
            Assert.False(outcomeToTest.IsBeingAffected);
            Assert.False(involvedState.IsConsequential);
            
            Observer.ObserveInteractions(process, outcomeToTest);
            
            Assert.False(outcomeToTest.IsBeingAffected);
            Assert.False(involvedState.IsConsequential);
        }
        
        [Test]
        public void IfInvolvedStateIsInvalid_ObservedOutcomeIsInvalidated()
        {
            State    involvedState = new State();
            Outcome  outcomeToTest = new Outcome();
            IProcess process       = GetProcessThatCreatesADependencyOn(involvedState);
            
            involvedState.Invalidate();
            Assert.That(outcomeToTest.IsValid);
            Observer.ObserveInteractions(process, outcomeToTest);
            Assert.False(outcomeToTest.IsValid);
        }

        [Test]
        public void WhenPausedUsingPauseToken_ObservationsResumeAfterTokenIsDisposed()
        {
            State       involvedState   = new State();
            Outcome     outcome         = new Outcome();
            IProcess    process         = new ActionProcess(PauseAndCheckIfObserving);
            IDisposable pauseToken      = null;
            bool        processWasExecuted = false;

            Observer.ObserveInteractions(process, outcome);
            Assert.That(processWasExecuted);
            

            void PauseAndCheckIfObserving()
            {
                Assert.That(Observer.IsCurrentlyObserving);
                pauseToken = Observer.PauseObservation();
                Assert.False(Observer.IsCurrentlyObserving);
                pauseToken.Dispose();
                Assert.That(Observer.IsCurrentlyObserving);
                processWasExecuted = true;
            }
            
        }

        [Test]
        public void IfObservationIsPausedWhileInProgress_NoDependenciesAreCreated()
        {
            State    involvedState = new State();
            Outcome  outcomeToTest = new Outcome();
            IProcess process       = new ActionProcess(PauseAndNotifyInvolved);

            Assert.False(outcomeToTest.IsBeingAffected);
            Assert.False(involvedState.IsConsequential);
            Observer.ObserveInteractions(process, outcomeToTest);
            Assert.False(outcomeToTest.IsBeingAffected);
            Assert.False(involvedState.IsConsequential);

            void PauseAndNotifyInvolved()
            {
                using (Observer.PauseObservation())
                {
                    involvedState.NotifyInvolved();
                }
            }
        }

        [Test]
        public void WhenObservationIsResumedAfterAPause_ConnectionsAreRegistered()
        {
            State    involvedState = new State();
            Outcome  outcomeToTest = new Outcome();
            IProcess process       = new ActionProcess(PauseAndAfterwardNotifyInvolved);
            bool     wasPaused     = false;

            Assert.False(outcomeToTest.IsBeingAffected);
            Assert.False(involvedState.IsConsequential);
            Observer.ObserveInteractions(process, outcomeToTest);
            Assert.That(wasPaused);
            Assert.That(outcomeToTest.IsBeingAffected);
            Assert.That(involvedState.IsConsequential);
            

            void PauseAndAfterwardNotifyInvolved()
            {
                using (Observer.PauseObservation())
                {
                    wasPaused = true;
                }
                
                involvedState.NotifyInvolved();
            }
        }

        //[Test]
        //public void SetInMotion_WhenGivenATrackerAlreadyInMotion_ReturnsFalse()
        //{
        //
        //}
        
        //[Test]
        //public void SetInMotion_WhenGivenATrackerNotInPlay_SetsTheTrackerInMotion()
        //{
        //
        //}
        
        //public void SetInMotion_AfterPuttingAConsequenceInMotion_EnsuresThatTrackerIsNoLongerTriggered()
        //{
        //
        //}
        
        //public void SetInMotion_AfterPuttingAConsequenceInMotion_EnsuresThatTrackerIsMarkedAsInMotion()
        //{
        //
        //}
    }
}