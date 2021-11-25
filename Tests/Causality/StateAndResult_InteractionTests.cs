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
    [TestFixture(typeof(Response     ), typeof(Response_Factory    ), typeof(CausalFactor), typeof(CausalFactor_Factory))]
    [TestFixture(typeof(Reaction     ), typeof(ReactionFactory     ), typeof(CausalFactor), typeof(CausalFactor_Factory))]
    [TestFixture(typeof(Result<int>  ), typeof(Result_Int_Factory  ), typeof(CausalFactor), typeof(CausalFactor_Factory))]
    [TestFixture(typeof(Reactive<int>), typeof(Reactive_Int_Factory), typeof(CausalFactor), typeof(CausalFactor_Factory))]
    
    public class StateAndResult_InteractionTests<TResult, TResultFactory, TParent, TParentFactory> 
        where TResult        : IResult
        where TResultFactory : IResultFactory<TResult>, new()
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

            Assert.False(resultToTest.IsBeingInfluenced);
            Assert.False(involvedFactor.HasDependents);

            resultToTest.React();
            
            Assert.True(resultToTest.IsBeingInfluenced);
            Assert.True(involvedFactor.HasDependents);
        }
        
        [Test]
        public void WhenParentFactorInvalidatesDependents_DependentResultsAreInvalidated()
        {
            IFactor involvedFactor = parentFactory.CreateInstance();
            IResult resultToTest   = resultFactory.CreateInstance_WhoseUpdateInvolves(involvedFactor);

            resultToTest.React();
            Assert.That(resultToTest.IsValid, Is.True);
            
            involvedFactor.InvalidateDependents();
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

            necessaryResult.React();
            reflexiveResult.React();
            
            Assert.That(necessaryResult.IsValid,       Is.True);
            Assert.That(reflexiveResult.IsValid,       Is.True);
            Assert.That(parentFactor.HasDependents,    Is.True);
            Assert.That(necessaryResult.HasDependents, Is.True);
            Assert.That(necessaryProcess.NumberOfTimesExecuted, Is.EqualTo(1));
            Assert.That(reflexiveProcess.NumberOfTimesExecuted, Is.EqualTo(1));
            
            reflexiveResult.IsReflexive = true;
            
            Assert.That(necessaryResult.IsNecessary, Is.True);
            
            parentFactor.InvalidateDependents();
            
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

            Assert.That(dependentResult.IsBeingInfluenced, Is.False);
            Assert.That(dependentResult.IsValid,           Is.False);
            Assert.That(parentFactor.HasDependents,        Is.False);
            
            dependentResult.React();
            
            Assert.That(dependentResult.NumberOfInfluences, Is.True);
            Assert.That(parentFactor.HasDependents,         Is.True);
            Assert.That(dependentResult.IsValid,            Is.True);

        }
    }
}