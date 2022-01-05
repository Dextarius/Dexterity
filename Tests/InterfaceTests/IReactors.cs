using Core.Causality;
using Core.Factors;
using Factors;
using Factors.Outcomes.ObservedOutcomes;
using NUnit.Framework;
using Tests.Tools.Factories;
using Tests.Tools.Interfaces;
using Tests.Tools.Mocks.Processes;

namespace Tests
{
    [TestFixture(typeof(ObservedResponse   ), typeof(Response_Factory    ))]
    [TestFixture(typeof(Reaction           ), typeof(ReactionFactory     ))]
    [TestFixture(typeof(ObservedResult<int>), typeof(Reactor_Int_Factory  ))]
    [TestFixture(typeof(Reactive<int>      ), typeof(Reactive_Int_Factory))]

    public class IReactors<TReactor, TReactorFactory> 
        where TReactor        : IReactor
        where TReactorFactory : IReactorFactory<TReactor>, new()
    {
        #region Instance Fields

        private TReactorFactory resultFactory = new TReactorFactory();

        #endregion


        #region Tests
        
        [Test]
        public void AfterReacting_IsValid()
        {
            TReactor resultToTest = resultFactory.CreateInstance();
            
            Assert.That(resultToTest.CanReact, Is.False);
            resultToTest.ForceReaction();
            Assert.That(resultToTest.CanReact, Is.True);
        }
        
        [Test]
        public void AfterInvalidateIsCalled_IsInvalid()
        {
            IReactor resultToTest = resultFactory.CreateInstance();

            resultToTest.ForceReaction();
            Assert.That(resultToTest.CanReact, Is.True);
            resultToTest.Trigger(null);
            Assert.That(resultToTest.CanReact, Is.False);
        }

        [Test]
        public void IfResultIsInvalidWhenNotifyingItsInvolved_DependencyIsStillCreated()
        {
            TReactor parentResult = resultFactory.CreateInstance();
            TReactor childResult  = resultFactory.CreateInstance_WhoseUpdateInvolves(parentResult);

            Assert.That(childResult.HasTriggers, Is.False);
            Assert.That(parentResult.HasSubscribers,    Is.False);
            
            parentResult.Trigger(null);
            Assert.That(parentResult.CanReact, Is.False);
            childResult.ForceReaction();

            Assert.That(parentResult.CanReact,          Is.False);
            Assert.That(childResult.HasTriggers, Is.True);
            Assert.That(parentResult.HasSubscribers,    Is.True);
            
        }

        //- This needs to go into a different set of tests
        [Test]
        public void WhenDependentOnAnotherResultAndThatResultIsInvalidated_BecomesUnstable()
        {
            TReactor parentResult = resultFactory.CreateInstance();
            TReactor childResult  = resultFactory.CreateInstance_WhoseUpdateInvolves(parentResult);

            parentResult.ForceReaction();
            
            Assert.That(childResult.HasTriggers, Is.False);
            Assert.That(parentResult.HasSubscribers,    Is.False);
            Assert.That(parentResult.CanReact,          Is.True);

            childResult.ForceReaction();
            
            Assert.That(childResult.HasTriggers, Is.True);
            Assert.That(parentResult.HasSubscribers,    Is.True);
            
            parentResult.Trigger(null);

            Assert.That(parentResult.CanReact, Is.False);
            Assert.That(childResult.IsStable, Is.False);
            Assert.That(childResult.CanReact,  Is.True);
        }

        [Test]
        public void WhenInvalidated_DestabilizesDependents()
        {
          //TParent rootFactor   = parentFactory.CreateInstance();
            TReactor parentResult = resultFactory.CreateInstance();
            TReactor childResult  = resultFactory.CreateInstance_WhoseUpdateInvolves(parentResult);

            parentResult.ForceReaction();
            
            Assert.That(childResult.HasTriggers, Is.False);
            Assert.That(parentResult.HasSubscribers,    Is.False);
            
            childResult.ForceReaction();
            
            Assert.That(childResult.HasTriggers, Is.True);
            Assert.That(parentResult.HasSubscribers,    Is.True);
            Assert.That(parentResult.CanReact,          Is.True);
            Assert.That(childResult.CanReact,           Is.True);
            Assert.That(childResult.IsStable,          Is.True);

            parentResult.Trigger(null);
            
            Assert.That(parentResult.CanReact, Is.False);
            Assert.That(childResult.CanReact,  Is.True);
            Assert.That(childResult.IsStable, Is.False);
        }

        [Test]
        public void AfterReacting_DoesNotReactAgainWithoutBeingInvalidated()
        {
            var      process      = new IncrementingProcess();
            TReactor resultToTest = resultFactory.CreateInstance_WhoseUpdateCalls(process);
            
            
            Assert.That(resultToTest.CanReact,          Is.False);
            Assert.That(process.NumberOfTimesExecuted, Is.EqualTo(0));

            resultToTest.ForceReaction();
            
            Assert.That(process.NumberOfTimesExecuted, Is.EqualTo(1));

            resultToTest.Reconcile();
            resultToTest.Reconcile();
            resultToTest.Reconcile();
            resultToTest.Reconcile();
            
            Assert.That(process.NumberOfTimesExecuted, Is.EqualTo(1));
        }
        
        [Test]
        public void WhenStabilizedWhileInvalid_AttemptsToStabilizeParentsBeforeUpdating()
        {
            IReactor grandParentResult  = null;
            IReactor parentResult       = null;
            IReactor resultBeingTested  = null;
            IProcess grandParentProcess = ObservedActionResponse.CreateFrom(GrandparentTests);
            IProcess parentProcess      = ObservedActionResponse.CreateFrom(ParentTests);
            IProcess childProcess       = ObservedActionResponse.CreateFrom(ChildResultTests);

            grandParentResult  = resultFactory.CreateInstance_WhoseUpdateCalls(grandParentProcess);
            parentResult       = resultFactory.CreateInstance_WhoseUpdateCalls(parentProcess);
            resultBeingTested  = resultFactory.CreateInstance_WhoseUpdateCalls(childProcess);

            grandParentResult.ForceReaction();
            parentResult.ForceReaction();
            resultBeingTested.ForceReaction();

            Assert.That(grandParentResult.CanReact,  Is.True);
            Assert.That(grandParentResult.IsStable, Is.True);
            Assert.That(resultBeingTested.IsStable, Is.True);
            Assert.That(parentResult.IsStable,      Is.True);

            grandParentResult.Trigger(null);

            Assert.That(parentResult.IsStable,      Is.False);
            Assert.That(resultBeingTested.IsStable, Is.False);

            resultBeingTested.Reconcile();

            Assert.That(grandParentResult.CanReact,  Is.True);
            Assert.That(grandParentResult.IsStable, Is.True);
            Assert.That(resultBeingTested.IsStable, Is.True);
            Assert.That(parentResult.IsStable,      Is.True);
            
            return;


            #region Local Functions

            void GrandparentTests()
            {
                Assert.That(parentResult.IsStable,      Is.False);
                Assert.That(resultBeingTested.IsStable, Is.False);
                TestContext.WriteLine("GrandParent Updated");
            }

            void ParentTests()
            {
                Assert.That(grandParentResult.IsStable, Is.True);
                Assert.That(grandParentResult.CanReact,  Is.True);
                Assert.That(resultBeingTested.IsStable, Is.False);
                grandParentResult.NotifyInvolved();
                TestContext.WriteLine("Parent Updated");
            }
            
            void ChildResultTests()
            {
                Assert.That(parentResult.IsStable,      Is.True);
                Assert.That(grandParentResult.IsStable, Is.True);
                parentResult.NotifyInvolved();
                TestContext.WriteLine("Child Updated");
            }

            #endregion
        }

        #endregion
        
        //- Test - Both NotifyInfluences()
        //- Test - AddDependent()
        //- Test - HasInfluences
        //- Test - NumberOfInfluences



        #region Outdated Tests

        // [Test]
        // public void IfParentFactorIsInvalidWhenAccessed_ResultIsInvalidated()
        // {
        //     Factor    involvedFactor = new Factor(null);
        //     IProcess process         = GetProcessThatCreatesADependencyOn(involvedFactor);
        //     Result  resultToTest     = new Result(null, process);
        //
        //     involvedFactor.Invalidate();
        //     Assert.That(resultToTest.IsInvalid);
        //     resultToTest.Recalculate();
        //     Assert.False(resultToTest.IsInvalid);
        // }
        //
        //
        
        //- There isn't much point in this, the current version of Result is designed to throw if invalidated
        //  during an update.
        // [Test]
        // public void IfResultIsAlreadyInvalidWhenWhenAccessingParentFactor_NoDependencyIsCreated()
        // {
        //     Factor    involvedFactor = new Factor(null);
        //     IProcess process       = GetProcessThatCreatesADependencyOn(involvedFactor);
        //     Result  resultToTest = new Result(null, process);
        //
        //     resultToTest.Invalidate();
        //     Assert.False(resultToTest.IsBeingAffected);
        //     Assert.False(involvedFactor.IsConsequential);
        //     
        //     Observer.ObserveInteractions(process, resultToTest);
        //     
        //     Assert.False(resultToTest.IsBeingAffected);
        //     Assert.False(involvedFactor.IsConsequential);
        // }
        
        // [Test] 
        // public void Reaction_IfAlreadyReacting_DoesNotExecuteAction()
        // {
        //
        // }

        #endregion
    }
}