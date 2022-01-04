using Core.Factors;
using Factors;
using NUnit.Framework;
using Tests.Tools;
using Tests.Tools.Factories;
using Tests.Tools.Interfaces;
using Tests.Tools.Mocks;

namespace Tests.InterfaceTests
{
    [TestFixture(typeof(Proactive<int>), typeof(Proactive_Int_Factory))]
    [TestFixture(typeof(Reactive<int> ), typeof( Reactive_Int_Factory))]
    [TestFixture(typeof(Reaction),       typeof(ReactionFactory))]
 // [TestFixture(typeof(ObservedResponse),   typeof(OutcomeFactory))]
 // [TestFixture(typeof(  Result<int>),      typeof(  Result_Int_Factory))
 // [TestFixture(typeof(ObservedState<int>), typeof(State_Int_Factory))]
    public class IFactor_Tests<TFactor, TFactory> 
        where TFactor   : IFactor
        where TFactory  : IFactory<TFactor>, new()
    {
        private TFactory factory = new TFactory();
        
        [Test]
        public void WhenDependentAddsItself_HasDependentIsTrue()
        {
            IFactor factorBeingTested = factory.CreateInstance();

            //- If for whatever reason the factor already has dependents when it's created, we don't need this test.
            if (factorBeingTested.HasDependents is false)
            {
                var dependent = new MockDependent();
                
                Assert.That(factorBeingTested.NumberOfDependents, Is.Zero,  
                    ErrorMessages.DependentsGreaterThanZero<TFactor>(
                        $"when {nameof(factorBeingTested.HasDependents)} is false. ", factorBeingTested.NumberOfDependents));
            
                factorBeingTested.AddDependent(dependent);
                Assert.That(factorBeingTested.HasDependents, Is.True);
            }
        }
        
        [Test]
        public void WhenDependentAddsItself_NumberOfDependentsGoesUpByOne()
        {
            IFactor factorBeingTested          = factory.CreateInstance();
            int     previousNumberOfDependents = factorBeingTested.NumberOfDependents;

            for (int i = 0; i < 100000; i++)
            {
                var dependent = new MockDependent();

                factorBeingTested.AddDependent(dependent);
                
                Assert.That(factorBeingTested.HasDependents,      Is.True);
                Assert.That(factorBeingTested.NumberOfDependents, Is.EqualTo(previousNumberOfDependents + 1));

                previousNumberOfDependents++;
            }
        }

        [Test]
        public void WhenDependentTriesToAddItselfMultipleTimes_OnlyAddsDependentOnce()
        {
            IFactor factorBeingTested          = factory.CreateInstance();
            var     dependent                  = new MockDependent();
            int     originalNumberOfDependents = factorBeingTested.NumberOfDependents;
            int     expectedNumberOfDependents = originalNumberOfDependents + 1;

            for (int i = 0; i < 100000; i++)
            {
                factorBeingTested.AddDependent(dependent);
            
                Assert.That(factorBeingTested.HasDependents,      Is.True);
                Assert.That(factorBeingTested.NumberOfDependents, Is.EqualTo(expectedNumberOfDependents));
            }
        }
        
        [Test]
        public void WhenInvalidateDependentsIsCalled_DependentsAreInvalidated()
        {
            IFactor factorBeingTested = factory.CreateInstance();
            var     dependent         = new MockDependent();

            factorBeingTested.AddDependent(dependent);
            Assert.That(dependent.IsValid, Is.True);
            
            factorBeingTested.InvalidateDependents();
            Assert.That(dependent.IsValid, Is.False);
        }
        
        
        
        // [Test]
        // public void WhenChanged_InvalidatesDependents()
        // {
        //     IFactor factorBeingTested = factory.CreateInstance();
        //     var     interaction       = new MockDependent();
        //
        //     factorBeingTested.AddDependent(interaction);
        //     Assert.That(interaction.IsValid, Is.True);
        //
        //     factorBeingTested..NotifyChanged();
        //     Assert.That(interaction.IsValid, Is.False);
        // }
        
        //[Test]
        // public void WhenAffectedFactorsNoLongerInUse_CanBeGarbageCollected()
        // {
        //     HashSet<WeakReference<IOutcome>> reactiveReferences = new HashSet<WeakReference<IOutcome>>();
        //     WeakReference<IState>            stateReference     = GenerateChainOfFactors(reactiveReferences);
        //
        //     GC.Collect();
        //     Thread.Sleep(1000);
        //
        //     foreach (var reference in reactiveReferences)
        //     {
        //         reference.TryGetTarget(out var reactive);
        //         Assert.That(reactive, Is.Null);
        //     }
        //
        //     stateReference.TryGetTarget(out var state);
        //     Assert.That(state, Is.Null);
        //
        //
        //     //- This may seem like a strange test, but the nature of States is to store references to the
        //     //  things they affect, and the nature of Outcomes to store reference to things that affect them
        //     //  can easily lead to a bunch of objects never being collected because they all reference each other.
        //     //  Both classes are structured in a way that prevents them from creating circular references
        //     //  to each other, and this is here to make sure that is/remains true. 
        // }
        


        #region Static Methods

        //[MethodImpl(MethodImplOptions.NoInlining)]
        // private static WeakReference<IState> GenerateChainOfFactors(HashSet< WeakReference<IOutcome>> references)
        // {
        //     IState                stateValue       = CreateDefaultState();
        //     WeakReference<IState> referenceToState = new WeakReference<IState>(stateValue);
        //     IOutcome              createdOutcome   = CreateOutcomeThatGetsValueOf(stateValue);
        //
        //     createdOutcome.Stabilize();
        //
        //     for (int i = 0; i < 3; i++)
        //     {
        //         createdOutcome = cre(createdOutcome);
        //         references.Add(new WeakReference<IOutcome>(createdOutcome));
        //     }
        //
        //     foreach (var reference in references)
        //     {
        //         reference.TryGetTarget(out var reactive);
        //         Assert.That(reactive, Is.Not.Null);
        //     }
        //
        //     Assert.That(stateValue.HasDependents);
        //
        //     return referenceToState;
        // }



        #endregion

    }
}