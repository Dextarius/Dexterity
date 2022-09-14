using System;
using Factors.Observer;
using NUnit.Framework;
using Tests.Tools.Mocks;
using Tests.Tools.Mocks.Processes;

namespace Tests.ObservedObjects
{
    public class Observers
    {
        private CausalObserver Observer => CausalObserver.ForThread;
        
        [Test]
        public void WhenNotifiedThatANullObjectIsInvolved_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => Observer.NotifyInvolved(null, 0));
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
            bool              processWasExecuted = false;
            MockActionProcess process            = new MockActionProcess(AssertIsObserving);
            MockObserved      observedObject     = new MockObserved();

            Observer.ObserveInteractions(process, observedObject);
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
            bool              processWasExecuted = false;
            MockActionProcess process            = new MockActionProcess(AssertIsNotObserving);
            MockObserved      observedObject     = new MockObserved();

            Observer.ObserveInteractions(process, observedObject);
            Assert.That(processWasExecuted, $"The process that tests {nameof(Observer.IsCurrentlyObserving)} did not run. ");
            TestContext.WriteLine($"Process was Executed => {processWasExecuted}");
            

            void AssertIsNotObserving()
            {
                bool isObserving;

                using (Observer.PauseObservation())
                {
                    isObserving = Observer.IsCurrentlyObserving;
                }

                Assert.That(isObserving, Is.False);
                TestContext.WriteLine($"Is Observing => {isObserving}");
                processWasExecuted = true;
            }
        }

        [Test]
        public void WhenNestedProcessIsFinishedBeingObserved_ObservationReturnsToOuterProcess()
        {
            int               testStarted            = 1;
            int               enteredOuterProcess    = 2;
            int               enteredInnerProcess    = 3;
            int               returnedToOuterProcess = 4;
            int               currentPhase           = 0;
            MockActionProcess innerProcess           = new MockActionProcess(InnerProcess); 
            MockObserved      innerObservedObject    = new MockObserved(); 
            MockActionProcess outerProcess           = new MockActionProcess(OuterProcess);
            MockObserved      outerObservedObject    = new MockObserved(); 

            currentPhase = testStarted;
            Observer.ObserveInteractions(outerProcess, outerObservedObject);
            Assert.That(currentPhase, Is.EqualTo(returnedToOuterProcess));


            void OuterProcess()
            {
                Assert.That(currentPhase, Is.EqualTo(testStarted));
                currentPhase = enteredOuterProcess;
                TestContext.WriteLine("Entered outer process. ");
                Observer.ObserveInteractions(innerProcess, innerObservedObject);
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
            MockActionProcess process            = new MockActionProcess(PauseAndCheckIfObserving);
            MockObserved      observedObject     = new MockObserved();
            IDisposable       pauseToken         = null;
            bool              processWasExecuted = false;

            Observer.ObserveInteractions(process, observedObject);
            Assert.That(processWasExecuted);
            

            void PauseAndCheckIfObserving()
            {
                Assert.That(Observer.IsCurrentlyObserving);
                pauseToken = Observer.PauseObservation();
                Assert.That(Observer.IsCurrentlyObserving, Is.False);
                pauseToken.Dispose();
                Assert.That(Observer.IsCurrentlyObserving);
                processWasExecuted = true;
            }
        }

        [Test]
        public void IfNotifiedAnObjectIsInvolvedDuringObservation_CallsNotifyInfluencedByOnObservedObject()
        {
            MockInvolvedFactor involvedFactor = new MockInvolvedFactor(Observer);
            MockActionProcess  process        = new MockActionProcess(involvedFactor.NotifyInvolved);
            MockObserved       observedObject = new MockObserved();
            
            Assert.That(observedObject.WasInfluenced, Is.False);
            Observer.ObserveInteractions(process, observedObject);
            Assert.That(observedObject.WasInfluenced, Is.True);
        }

        [Test]
        public void IfObservationIsPausedWhileInProgress_NoDependenciesAreCreated()
        {
            MockInvolvedFactor involvedFactor = new MockInvolvedFactor(Observer);
            MockActionProcess  process        = new MockActionProcess(PauseAndNotifyInvolved);
            MockObserved       observedObject = new MockObserved();

            Assert.That(observedObject.WasInfluenced, Is.False);
            Assert.That(involvedFactor.HasSubscribers, Is.False);
            
            Observer.ObserveInteractions(process, observedObject);
            
            Assert.That(observedObject.WasInfluenced, Is.False);
            Assert.That(involvedFactor.HasSubscribers, Is.False);

            
            void PauseAndNotifyInvolved()
            {
                using (Observer.PauseObservation())
                {
                    involvedFactor.NotifyInvolved();
                }
            }
        }

        [Test]
        public void WhenObservationIsResumedAfterAPause_ConnectionsAreRegistered()
        {
            MockInvolvedFactor involvedFactor = new MockInvolvedFactor(Observer);
            MockActionProcess  process        = new MockActionProcess(PauseAndUnpauseThenNotifyInvolved);
            MockObserved       observedObject = new MockObserved();
            bool               wasPaused      = false;

            Assert.That(observedObject.WasInfluenced, Is.False);

            Observer.ObserveInteractions(process, observedObject);
            
            Assert.That(wasPaused);
            Assert.That(observedObject.WasInfluenced, Is.True);
            
            
            void PauseAndUnpauseThenNotifyInvolved()
            {
                using (Observer.PauseObservation())
                {
                    wasPaused = true;
                }
                
                involvedFactor.NotifyInvolved();
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