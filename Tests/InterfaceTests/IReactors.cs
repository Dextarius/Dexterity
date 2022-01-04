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
            
            Assert.That(resultToTest.IsValid, Is.False);
            resultToTest.React();
            Assert.That(resultToTest.IsValid, Is.True);
        }
        
        [Test]
        public void AfterInvalidateIsCalled_IsInvalid()
        {
            IReactor resultToTest = resultFactory.CreateInstance();

            resultToTest.React();
            Assert.That(resultToTest.IsValid, Is.True);
            resultToTest.Invalidate(null);
            Assert.That(resultToTest.IsValid, Is.False);
        }

        [Test]
        public void IfResultIsInvalidWhenNotifyingItsInvolved_DependencyIsStillCreated()
        {
            TReactor parentResult = resultFactory.CreateInstance();
            TReactor childResult  = resultFactory.CreateInstance_WhoseUpdateInvolves(parentResult);

            Assert.That(childResult.IsBeingInfluenced, Is.False);
            Assert.That(parentResult.HasDependents,    Is.False);
            
            parentResult.Invalidate(null);
            Assert.That(parentResult.IsValid, Is.False);
            childResult.React();

            Assert.That(parentResult.IsValid,          Is.False);
            Assert.That(childResult.IsBeingInfluenced, Is.True);
            Assert.That(parentResult.HasDependents,    Is.True);
            
        }

        //- This needs to go into a different set of tests
        [Test]
        public void WhenDependentOnAnotherResultAndThatResultIsInvalidated_BecomesUnstable()
        {
            TReactor parentResult = resultFactory.CreateInstance();
            TReactor childResult  = resultFactory.CreateInstance_WhoseUpdateInvolves(parentResult);

            parentResult.React();
            
            Assert.That(childResult.IsBeingInfluenced, Is.False);
            Assert.That(parentResult.HasDependents,    Is.False);
            Assert.That(parentResult.IsValid,          Is.True);

            childResult.React();
            
            Assert.That(childResult.IsBeingInfluenced, Is.True);
            Assert.That(parentResult.HasDependents,    Is.True);
            
            parentResult.Invalidate(null);

            Assert.That(parentResult.IsValid, Is.False);
            Assert.That(childResult.IsStable, Is.False);
            Assert.That(childResult.IsValid,  Is.True);
        }

        [Test]
        public void WhenInvalidated_DestabilizesDependents()
        {
          //TParent rootFactor   = parentFactory.CreateInstance();
            TReactor parentResult = resultFactory.CreateInstance();
            TReactor childResult  = resultFactory.CreateInstance_WhoseUpdateInvolves(parentResult);

            parentResult.React();
            
            Assert.That(childResult.IsBeingInfluenced, Is.False);
            Assert.That(parentResult.HasDependents,    Is.False);
            
            childResult.React();
            
            Assert.That(childResult.IsBeingInfluenced, Is.True);
            Assert.That(parentResult.HasDependents,    Is.True);
            Assert.That(parentResult.IsValid,          Is.True);
            Assert.That(childResult.IsValid,           Is.True);
            Assert.That(childResult.IsStable,          Is.True);

            parentResult.Invalidate(null);
            
            Assert.That(parentResult.IsValid, Is.False);
            Assert.That(childResult.IsValid,  Is.True);
            Assert.That(childResult.IsStable, Is.False);
        }

        [Test]
        public void AfterReacting_DoesNotReactAgainWithoutBeingInvalidated()
        {
            var      process      = new IncrementingProcess();
            TReactor resultToTest = resultFactory.CreateInstance_WhoseUpdateCalls(process);
            
            
            Assert.That(resultToTest.IsValid,          Is.False);
            Assert.That(process.NumberOfTimesExecuted, Is.EqualTo(0));

            resultToTest.React();
            
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

            grandParentResult.React();
            parentResult.React();
            resultBeingTested.React();

            Assert.That(grandParentResult.IsValid,  Is.True);
            Assert.That(grandParentResult.IsStable, Is.True);
            Assert.That(resultBeingTested.IsStable, Is.True);
            Assert.That(parentResult.IsStable,      Is.True);

            grandParentResult.Invalidate(null);

            Assert.That(parentResult.IsStable,      Is.False);
            Assert.That(resultBeingTested.IsStable, Is.False);

            resultBeingTested.Reconcile();

            Assert.That(grandParentResult.IsValid,  Is.True);
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
                Assert.That(grandParentResult.IsValid,  Is.True);
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