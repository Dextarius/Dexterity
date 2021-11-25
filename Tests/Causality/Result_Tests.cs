using System;
using Causality.Processes;
using Causality.States;
using Core.Causality;
using Core.Factors;
using Core.States;
using Factors;
using NUnit.Framework;
using Tests.Causality.Factories;
using Tests.Causality.Interfaces;
using Tests.Causality.Mocks;
using static Tests.Tools;

namespace Tests.Causality
{
    [TestFixture(typeof(Response     ), typeof(Response_Factory    ))]
    [TestFixture(typeof(Reaction     ), typeof(ReactionFactory     ))]
    [TestFixture(typeof(Result<int>  ), typeof(Result_Int_Factory  ))]
    [TestFixture(typeof(Reactive<int>), typeof(Reactive_Int_Factory))]

    public class Result_Tests<TResult, TResultFactory> 
        where TResult        : IResult
        where TResultFactory : IResultFactory<TResult>, new()
    {
        #region Instance Fields

        private TResultFactory resultFactory = new TResultFactory();

        #endregion


        #region Tests

        [Test]

        public void WhenCreated_IsBeingInfluencedIsFalse()
        {
            IResult resultBeingTested = resultFactory.CreateInstance();

            Assert.That(resultBeingTested.IsBeingInfluenced, Is.False);
        }
        
        [Test]
        public void WhenCreated_NumberOfInfluencesIsZero()
        {
            IResult resultBeingTested = resultFactory.CreateInstance();

            Assert.That(resultBeingTested.NumberOfInfluences, Is.Zero);
        }
        
        [Test]
        public void WhenConstructed_IsInvalid()
        {
            IProcess process = new ActionProcess(DoNothing);
            TResult  result1 = resultFactory.CreateInstance();
            TResult  result2 = resultFactory.CreateInstance_WhoseUpdateCalls(process);
            TResult  result3 = resultFactory.CreateInstance_WhoseUpdateInvolves(result1);

            Assert.That(result1.IsValid, Is.False);
            Assert.That(result2.IsValid, Is.False);
            Assert.That(result3.IsValid, Is.False);
        }
        
        [Test]
        public void AfterReacting_IsValid()
        {
            TResult resultToTest = resultFactory.CreateInstance();
            
            Assert.That(resultToTest.IsValid, Is.False);
            resultToTest.React();
            Assert.That(resultToTest.IsValid, Is.True);
        }
        
        [Test]
        public void AfterInvalidateIsCalled_IsInvalid()
        {
            IResult resultToTest = resultFactory.CreateInstance();

            resultToTest.React();
            Assert.That(resultToTest.IsValid, Is.True);
            resultToTest.Invalidate(null);
            Assert.That(resultToTest.IsValid, Is.False);
        }

        [Test]
        public void IfResultIsInvalidWhenNotifyingItsInvolved_DependencyIsStillCreated()
        {
            TResult parentResult = resultFactory.CreateInstance();
            TResult childResult  = resultFactory.CreateInstance_WhoseUpdateInvolves(parentResult);

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
            
            TResult parentResult = resultFactory.CreateInstance();
            TResult childResult  = resultFactory.CreateInstance_WhoseUpdateInvolves(parentResult);

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
            TResult parentResult = resultFactory.CreateInstance();
            TResult childResult  = resultFactory.CreateInstance_WhoseUpdateInvolves(parentResult);

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
            var     process      = new IncrementingProcess();
            TResult resultToTest = resultFactory.CreateInstance_WhoseUpdateCalls(process);
            
            
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
            IResult  grandParentResult       = null;
            IResult  parentResult            = null;
            IResult  resultBeingTested       = null;
            IProcess grandParentProcess      = ActionProcess.CreateFrom(GrandparentTests);
            IProcess parentProcess           = ActionProcess.CreateFrom(ParentTests);
            IProcess childProcess            = ActionProcess.CreateFrom(ChildResultTests);

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