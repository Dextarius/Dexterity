using Core.Factors;
using Core.States;
using Factors;
using Factors.Outcomes.ObservedOutcomes;
using NUnit.Framework;
using Tests.Tools.Factories;
using Tests.Tools.Interfaces;
using Tests.Tools.Mocks.Processes;
using static Tests.Tools.Tools;

namespace Tests.Integration
{
    [TestFixture(typeof(ObservedResponse     ), typeof(Response_Factory    ), typeof(CausalFactor), typeof(CausalFactor_Factory))]
    [TestFixture(typeof(Reaction     ), typeof(ReactionFactory     ), typeof(CausalFactor), typeof(CausalFactor_Factory))]
    [TestFixture(typeof(Result<int>  ), typeof(Reactor_Int_Factory  ), typeof(CausalFactor), typeof(CausalFactor_Factory))]
    [TestFixture(typeof(Reactive<int>), typeof(Reactive_Int_Factory), typeof(CausalFactor), typeof(CausalFactor_Factory))]
    
    public class StateAndResult_InteractionTests<TResult, TResultFactory, TParent, TParentFactory> 
        where TResult        : IResult
        where TResultFactory : IReactorFactory<TResult>, new()
        where TParent        : IFactor
        where TParentFactory : IFactory<TParent>, new()
    {
        #region Instance Fields

        private TParentFactory parentFactory = new();
        private TResultFactory resultFactory = new();

        #endregion


        #region Tests

        [Test]
        public void WhenAFactorNotifiesThatItIsInvolvedDuringUpdateProcess_ResultIsDependentOnInvolvedFactor()
        {
            var involvedFactor = parentFactory.CreateInstance();
            var resultToTest   = resultFactory.CreateInstance_WhoseUpdateInvolves(involvedFactor);

            Assert.False(resultToTest.HasTriggers);
            Assert.False(involvedFactor.HasSubscribers);

            resultToTest.ForceReaction();
            
            Assert.True(resultToTest.HasTriggers);
            Assert.True(involvedFactor.HasSubscribers);
        }
        
        [Test]
        public void WhenParentFactorInvalidatesDependents_DependentResultsAreInvalidated()
        {
            IFactor involvedFactor = parentFactory.CreateInstance();
            IResult resultToTest   = resultFactory.CreateInstance_WhoseUpdateInvolves(involvedFactor);

            resultToTest.ForceReaction();
            Assert.That(resultToTest.IsValid, Is.True);
            
            involvedFactor.TriggerSubscribers();
            Assert.That(resultToTest.IsValid, Is.False);
        }
        
        [Test]
        public void WhenDependentIsReflexive_ParentIsNecessary()
        {
            var parentFactor    = parentFactory.CreateInstance();
            var dependentResult = resultFactory.CreateInstance_WhoseUpdateInvolves(parentFactor);

            Assert_React_CreatesExclusiveDependencyBetween(parentFactor, dependentResult);
            Assert.That(parentFactor.IsNecessary,    Is.False);
            Assert.That(dependentResult.IsReflexive, Is.False);

            dependentResult.IsReflexive = true;

            Assert.That(parentFactor.IsNecessary,    Is.True);
            Assert.That(dependentResult.IsReflexive, Is.True);
        }
        

        
        [Test]
        public void WhenParentLosesAllNecessaryDependents_IsNoLongerNecessary()
        {
            var parentFactor    = parentFactory.CreateInstance();
            var dependentResult = resultFactory.CreateInstance_WhoseUpdateInvolves(parentFactor);

            Assert_React_CreatesExclusiveDependencyBetween(parentFactor, dependentResult);

            dependentResult.IsReflexive = true;

            Assert.That(parentFactor.IsNecessary,    Is.True);
            Assert.That(dependentResult.IsReflexive, Is.True);
            
            dependentResult.IsReflexive = false;
            
            Assert.That(parentFactor.IsNecessary,    Is.False);
            Assert.That(dependentResult.IsReflexive, Is.False);
        }


        
        [Test]
        public void WhenParentWithNecessaryDependentsIsInvalidated_DependentResultsAreUpdated()
        {
            IFactor             parentFactor     = parentFactory.CreateInstance();
            IncrementingProcess necessaryProcess = new IncrementingProcess(parentFactor);
            IResult             necessaryResult  = resultFactory.CreateInstance_WhoseUpdateCalls(necessaryProcess);
            IncrementingProcess reflexiveProcess = new IncrementingProcess(necessaryResult);
            IResult             reflexiveResult  = resultFactory.CreateInstance_WhoseUpdateCalls(reflexiveProcess);

            necessaryResult.ForceReaction();
            reflexiveResult.ForceReaction();
            
            Assert.That(necessaryResult.IsValid,       Is.True);
            Assert.That(reflexiveResult.IsValid,       Is.True);
            Assert.That(parentFactor.HasSubscribers,    Is.True);
            Assert.That(necessaryResult.HasSubscribers, Is.True);
            Assert.That(necessaryProcess.NumberOfTimesExecuted, Is.EqualTo(1));
            Assert.That(reflexiveProcess.NumberOfTimesExecuted, Is.EqualTo(1));
            
            reflexiveResult.IsReflexive = true;
            
            Assert.That(necessaryResult.IsNecessary, Is.True);
            
            parentFactor.TriggerSubscribers();
            
            Assert.That(necessaryResult.IsValid,                Is.True);
            Assert.That(reflexiveResult.IsValid,                Is.True);
            Assert.That(necessaryProcess.NumberOfTimesExecuted, Is.EqualTo(2));
            Assert.That(reflexiveProcess.NumberOfTimesExecuted, Is.EqualTo(2));
        }
        
        //[Test]
        public void WhenNecessaryDependentIsAddedToParent_ParentIsNecessary()
        {
           
        }
        //[Test]
        public void WhenAResultBecomesNecessary_ItOnlyNotifiesTheParentItIsNecessaryIfTheResultNeedsToBeUpdated()
        {
            
        }
        
        
        // [Test]
        // public void Reaction_AfterProactorChanges_IsAbleToReact()
        // {
        //
        // }

        #endregion
        
        
        
        public void CreateNewFactor_AndAResultThatIsDependentOnCreatedFactor(out TResult dependentResult)
        {
            var parentFactor = parentFactory.CreateInstance();
            
            dependentResult = resultFactory.CreateInstance_WhoseUpdateInvolves(parentFactor);

            Assert.That(dependentResult.HasTriggers, Is.False);
            Assert.That(dependentResult.IsValid,           Is.False);
            Assert.That(parentFactor.HasSubscribers,        Is.False);
            
            dependentResult.ForceReaction();
            
            Assert.That(dependentResult.NumberOfTriggers, Is.True);
            Assert.That(parentFactor.HasSubscribers,         Is.True);
            Assert.That(dependentResult.IsValid,            Is.True);

        }
    }
}