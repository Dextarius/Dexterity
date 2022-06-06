using Core.Causality;
using Core.Factors;
using Factors;
using Factors.Cores;
using Factors.Cores.ObservedReactorCores;
using NUnit.Framework;
using Tests.Tools;
using Tests.Tools.Interfaces;
using Tests.Tools.Mocks;
using Tests.Tools.Mocks.Processes;
using static Tests.Tools.Tools;

namespace Tests.Class_Tests
{
    public class ReactorCores<TCore, TFactory>
        where TCore    : ReactorCore
        where TFactory : IFactory<TCore>, IMultiConstructorFactory<TCore>, new()
    {
        #region Instance Fields

        private TFactory factory = new TFactory();

        #endregion


        #region Tests
        
        [Test]
        public void WhenCreated_HasTriggers_IsFalse()
        {
            TCore[] coresToTest = factory.CallAllConstructors();

            foreach (var core in coresToTest)
            {
                Assert.That(core.HasTriggers, Is.False);
            }
        }
        
        [Test]
        public void WhenCreated_NumberOfTriggers_IsZero()
        {
            TCore[] coresToTest = factory.CallAllConstructors();

            foreach (var core in coresToTest)
            {
                Assert.That(core.NumberOfTriggers, Is.Zero);
            }
        }
        
        [Test]
        public void WhenCreated_IsReacting_IsFalse()
        {
            TCore[] coresToTest = factory.CallAllConstructors();
            
            foreach (var core in coresToTest)
            {
                Assert.That(core.IsReacting, Is.False);
            }
        }

        [Test]
        public void WhenCreated_HasBeenTriggered_IsTrue()
        {
            TCore[] coresToTest = factory.CallAllConstructors();
            
            foreach (var core in coresToTest)
            {
                Assert.That(core.HasBeenTriggered, Is.True);
            }
        }

        [Test]
        public void IfInvalidatedWhenNotReflexive_DoesNotUpdateValue()
        {
            int            numberOfExecutions = 0;
            Proactive<int> proactive          = new Proactive<int>(42);
            Reactive<int>  reactiveToTest     = new Reactive<int>(IncrementNumExecutionsAndReturn);

            Assert.That(numberOfExecutions, Is.Zero);
            Assert.That(reactiveToTest.IsReflexive, Is.False, 
                $"The value of {nameof(reactiveToTest.IsReflexive)} was {reactiveToTest.IsReflexive}");

            int triggerProcess = reactiveToTest.Value;
            TestContext.WriteLine("Initial value calculated.");

            Assert.That(numberOfExecutions, Is.EqualTo(1));

            proactive.Value = 13;
            TestContext.WriteLine("Proactive value updated.");
            
            Assert.That(numberOfExecutions, Is.EqualTo(1));
            
            triggerProcess = reactiveToTest.Value;
            TestContext.WriteLine("Reactive value updated.");

            Assert.That(numberOfExecutions, Is.EqualTo(2));


            int IncrementNumExecutionsAndReturn()
            {
                numberOfExecutions++;
                TestContext.WriteLine("The process was executed.");

                return proactive.Value;
            }
        }
        
        // [Test]
        public void WhenUpdateStarts_IsNotUnstable()
        {
            
        }
        
        // [Test]
        public void WhenUpdateStarts_IsNotStabilizing()
        {
            
        }
        
        // [Test]
        public void WhenTriggeringDependent_IfAllNecessaryDependentsRemoveThemselves_NotifiesTriggersItIsNotNecessary()
        {
            
        }
        
        // [Test]
        // public void WhenCreated_WeakReference_ReturnsAReferenceToTheReactorCore()
        // {
        //     TCore[] coresToTest = factory.CallAllConstructors();
        //     
        //     foreach (var core in coresToTest)
        //     {
        //         core.WeakReference.TryGetTarget(out var referenceTarget);
        //         Assert.That(referenceTarget, Is.SameAs(core));
        //     }
        // }
        
        // [Test]
        // public void WhenCreated_DoesNotReactDuringConstruction()
        // {
        //     bool  processExecuted = false;
        //     TCore coreBeingTested = factory.CreateInstance();
        //     
        //     Assert.That(processExecuted, Is.False, "The reactive executed its process during construction.");
        //     TestContext.WriteLine($"The process was executed prior to retrieving value => {processExecuted}");
        //     
        //     int triggerProcess = coreBeingTested.Value;
        //     
        //     Assert.That(processExecuted, "The method used to conduct this test does not mark that the process was executed.");
        //     TestContext.WriteLine($"The process was executed after retrieving value => {processExecuted}");
        //
        //     int UpdateBoolAndReturn()
        //     {
        //         processExecuted = true;
        //         return 42;
        //     }
        // }
        //
        // [Test]
        // public void AfterBeingTriggered_HasBeenTriggered_IsFalse()
        // {
        //
        // }

        // [Test]
        // public void WhileUpdating_IsUpdatingReturnsTrue()
        // {
        //     TCore coreBeingTested = factory.CreateInstance();
        //
        //     using (manipulator.PutReactiveIntoUpdatingState())
        //     {
        //         Assert.That(manipulator.Reactive.IsReacting, 
        //             $"{NameOf<Reactive<int>>()}.{nameof(Reactive<int>.IsReacting)} was false during its update. ");
        //         TestContext.WriteLine($"{nameof(Reactive<int>.IsReacting)} was {manipulator.Reactive.IsReacting}. ");
        //     }
        // }
        
        
        // [Test]
        // public void WhenStabilizedWhileInvalid_AttemptsToStabilizeParentsBeforeUpdating()
        // {
        //     IReactor grandParentResult  = null;
        //     IReactor parentResult       = null;
        //     IReactor resultBeingTested  = null;
        //     IProcess grandParentProcess = ObservedActionResponse.CreateFrom(GrandparentTests);
        //     IProcess parentProcess      = ObservedActionResponse.CreateFrom(ParentTests);
        //     IProcess childProcess       = ObservedActionResponse.CreateFrom(ChildResultTests);
        //
        //     grandParentResult  = factory.CreateInstance_WhoseUpdateCalls(grandParentProcess);
        //     parentResult       = factory.CreateInstance_WhoseUpdateCalls(parentProcess);
        //     resultBeingTested  = factory.CreateInstance_WhoseUpdateCalls(childProcess);
        //
        //     grandParentResult.ForceReaction();
        //     parentResult.ForceReaction();
        //     resultBeingTested.ForceReaction();
        //
        //     Assert.That(grandParentResult.CanReact, Is.True);
        //     Assert.That(grandParentResult.IsStable, Is.True);
        //     Assert.That(resultBeingTested.IsStable, Is.True);
        //     Assert.That(parentResult.IsStable,      Is.True);
        //
        //     grandParentResult.Trigger(null);
        //
        //     Assert.That(parentResult.IsStable,      Is.False);
        //     Assert.That(resultBeingTested.IsStable, Is.False);
        //
        //     resultBeingTested.Reconcile();
        //
        //     Assert.That(grandParentResult.CanReact, Is.True);
        //     Assert.That(grandParentResult.IsStable, Is.True);
        //     Assert.That(resultBeingTested.IsStable, Is.True);
        //     Assert.That(parentResult.IsStable,      Is.True);
        //     
        //     return;
        //
        //
        //     #region Local Functions
        //
        //     void GrandparentTests()
        //     {
        //         Assert.That(parentResult.IsStable,      Is.False);
        //         Assert.That(resultBeingTested.IsStable, Is.False);
        //         TestContext.WriteLine("GrandParent Updated");
        //     }
        //
        //     void ParentTests()
        //     {
        //         Assert.That(grandParentResult.IsStable, Is.True);
        //         Assert.That(grandParentResult.CanReact, Is.True);
        //         Assert.That(resultBeingTested.IsStable, Is.False);
        //         grandParentResult.NotifyInvolved();
        //         TestContext.WriteLine("Parent Updated");
        //     }
        //     
        //     void ChildResultTests()
        //     {
        //         Assert.That(parentResult.IsStable,      Is.True);
        //         Assert.That(grandParentResult.IsStable, Is.True);
        //         parentResult.NotifyInvolved();
        //         TestContext.WriteLine("Child Updated");
        //     }
        //
        //     #endregion
        // }
        
        // [Test]
        // public void AfterReacting_DoesNotReactAgainWithoutBeingInvalidated()
        // {
        //     var      process      = new IncrementingProcess();
        //     TReactor resultToTest = factory.CreateInstance_WhoseUpdateCalls(process);
        //     
        //     Assert.That(resultToTest.CanReact,         Is.False);
        //     Assert.That(process.NumberOfTimesExecuted, Is.EqualTo(0));
        //
        //     resultToTest.ForceReaction();
        //     
        //     Assert.That(process.NumberOfTimesExecuted, Is.EqualTo(1));
        //
        //     resultToTest.Reconcile();
        //     resultToTest.Reconcile();
        //     resultToTest.Reconcile();
        //     resultToTest.Reconcile();
        //     
        //     Assert.That(process.NumberOfTimesExecuted, Is.EqualTo(1));
        // }

        
        // [Test] //- This one is mostly to make sure that when we queue updates, those updates are actually run.
        // public void WhenFactorInvalidatesDependents_DoesNotPreventUpdatesFromExecuting()
        // {
        //     IFactor factorBeingTested = factory.CreateInstance();
        //     var     dependent         = new MockFactorSubscriber();
        //
        //     dependent.ResetHasBeenTriggeredToFalse();
        //     dependent.MakeNecessary();
        //     factorBeingTested.Subscribe(dependent);
        //     
        //     Assert.That(dependent.HasBeenTriggered, Is.True);
        //
        //     factorBeingTested.TriggerSubscribers();
        //
        //     Assert.That(dependent.HasBeenTriggered, Is.True);
        //     Assert.That(dependent.WasUpdated,       Is.True);
        // }
        
     // // [Test]
     //    public void WhenUpdateStarts_IsValid()
     //    {
     //        ReactiveManipulator<int> manipulator = new ReactiveManipulator<int>(42);
     //    
     //        using (manipulator.PutReactiveIntoUpdatingState())
     //        {
     //            Assert.That(manipulator.Reactive.IsValid, Is.False,
     //                $"{NameOf<Reactive<int>>()}.{nameof(Reactive<int>.IsValid)} was true during its update. ");
     //            TestContext.WriteLine($"{nameof(Reactive<int>.IsValid)} was {manipulator.Reactive.IsValid}. ");
     //        }
     //    } 


     #region Outdated Tests

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
        
        
        #endregion
    }
}