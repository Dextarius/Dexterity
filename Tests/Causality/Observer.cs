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
        private CausalObserver Observer => CausalObserver.ForThread;
        
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
            Response       testOutcome        = new Response(null, process);

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
            Response       testOutcome        = new Response(null, process);

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
            Response       innerOutcome           = new Response(null, innerProcess); //- Verify Correct 
            ActionProcess outerProcess           = new ActionProcess(OuterProcess);
            Response       outerOutcome           = new Response(null, outerProcess); //- Verify Correct 

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
        public void WhenPausedUsingPauseToken_ObservationsResumeAfterTokenIsDisposed()
        {
            CausalFactor       involvedCausalFactor      = new CausalFactor(null);
            IProcess    process            = new ActionProcess(PauseAndCheckIfObserving);
            Response     outcome            = new Response(null, process);
            IDisposable pauseToken         = null;
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
            CausalFactor    involvedCausalFactor = new CausalFactor(null);
            IProcess process       = new ActionProcess(PauseAndNotifyInvolved);
            Response  outcomeToTest = new Response(null, process);

            Assert.False(outcomeToTest.IsBeingInfluenced);
            Assert.False(involvedCausalFactor.HasDependents);
            
            Observer.ObserveInteractions(process, outcomeToTest);
            
            Assert.False(outcomeToTest.IsBeingInfluenced);
            Assert.False(involvedCausalFactor.HasDependents);

            void PauseAndNotifyInvolved()
            {
                using (Observer.PauseObservation())
                {
                    involvedCausalFactor.NotifyInvolved();
                }
            }
        }

        [Test]
        public void WhenObservationIsResumedAfterAPause_ConnectionsAreRegistered()
        {
            CausalFactor    involvedCausalFactor = new CausalFactor(null);
            IProcess process       = new ActionProcess(PauseAndAfterwardNotifyInvolved);
            Response  outcomeToTest = new Response(null, process);
            bool     wasPaused     = false;

            Assert.False(outcomeToTest.IsBeingInfluenced);
            Assert.False(involvedCausalFactor.HasDependents);
            outcomeToTest.React();
            Assert.That(wasPaused);
            Assert.That(outcomeToTest.IsBeingInfluenced);
            Assert.That(involvedCausalFactor.HasDependents);
            

            void PauseAndAfterwardNotifyInvolved()
            {
                using (Observer.PauseObservation())
                {
                    wasPaused = true;
                }
                
                involvedCausalFactor.NotifyInvolved();
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