using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Causality;
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
    [TestFixture(typeof(Proactive<int>), typeof(Proactive_Int_Factory))]
    [TestFixture(typeof( Reactive<int>), typeof( Reactive_Int_Factory))]
    [TestFixture(typeof(  Result<int>), typeof(  Result_Int_Factory))]
    [TestFixture(typeof(    State<int>), typeof(    State_Int_Factory))]
    [TestFixture(typeof(Reaction),       typeof(ReactionFactory))]
    [TestFixture(typeof(Response),        typeof(OutcomeFactory))]
    public class IFactor_Tests<TFactor, TFactory> 
        where TFactor   : IFactor
        where TFactory  : IFactory<TFactor>, new()
    {
        private TFactory factory = new TFactory();
        
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void WhenCreated_HasDependentsIsFalse()
        {
            IFactor factorBeingTested = factory.CreateInstance();

            Assert.That(factorBeingTested.HasDependents, Is.False);
        }
        
        [Test]
        public void WhenCreated_NumberOfDependentsIsZero()
        {
            IFactor factorBeingTested = factory.CreateInstance();

            Assert.That(factorBeingTested.NumberOfDependents, Is.Zero);
        }
        
        [Test]
        public void WhenCreated_IsNotNecessary()
        {
            IFactor factorBeingTested = factory.CreateInstance();

            Assert.That(factorBeingTested.IsNecessary, Is.False);
        }

        [Test]
        public void WhenDependentAddsItself_HasDependent()
        {
            IFactor factorBeingTested = factory.CreateInstance();
            var     interaction       = new MockInteraction();
            
            AssumeHasNoDependents(factorBeingTested);
            factorBeingTested.AddDependent(interaction);
            
            Assert.That(factorBeingTested.HasDependents,      Is.True);
            Assert.That(factorBeingTested.NumberOfDependents, Is.EqualTo(1));
        }

        [Test]
        public void WhenDependentTriesToAddItselfMultipleTimes_OnlyAddsDependentOnce()
        {
            IFactor factorBeingTested = factory.CreateInstance();
            var     interaction       = new MockInteraction();

            AssumeHasNoDependents(factorBeingTested);

            for (int i = 0; i < 100; i++)
            {
                factorBeingTested.AddDependent(interaction);
            
                Assert.That(factorBeingTested.HasDependents,      Is.True);
                Assert.That(factorBeingTested.NumberOfDependents, Is.EqualTo(1));
            }
        }
        
        [Test]
        public void WhenInvolved_NotifiesObserver()
        {
            IFactor factorBeingTested = factory.CreateInstance();
            var     process           = CreateProcessThatCallsNotifyInvolvedOn(factorBeingTested);
            var     observedObject    = new MockInteraction(process);

            CausalFactor.Observe(process, observedObject);
            
            Assert.That(observedObject.WasInfluenced, Is.True);
        }

        [Test]
        public void WhenInvalidateDependentsIsCalled_DependentsAreInvalidated()
        {
            IFactor factorBeingTested = factory.CreateInstance();
            var     interaction       = new MockInteraction();

            factorBeingTested.AddDependent(interaction);
            AssumeHasOneDependent(factorBeingTested);
            Assert.That(interaction.IsValid, Is.True);
            
            factorBeingTested.InvalidateDependents();
            Assert.That(interaction.IsValid, Is.False);
        }

        [Test]
        public void WhenChanged_InvalidatesDependents()
        {
            IFactor factorBeingTested = factory.CreateInstance();
            var     interaction       = new MockInteraction();

            factorBeingTested.AddDependent(interaction);
            AssumeHasOneDependent(factorBeingTested);
            Assert.That(interaction.IsValid, Is.True);

            factorBeingTested.OnChanged();
            Assert.That(interaction.IsValid, Is.False);
        }

        [Test]
        public void IfFactorQueuesUpdatesWhileInvalidatingDependents_QueuedUpdatedAreExecuted()
        {
            IFactor factorBeingTested = factory.CreateInstance();
            var     interaction       = new MockInteraction();

            interaction.MakeValid();
            interaction.MakeNecessary();
            AssumeHasNoDependents(factorBeingTested);
            factorBeingTested.AddDependent(interaction);
            AssumeHasOneDependent(factorBeingTested);
            Assert.That(interaction.IsValid, Is.True);

            factorBeingTested.InvalidateDependents();

            Assert.That(interaction.IsValid, Is.True);
            Assert.That(interaction.WasUpdated, Is.True);
        }

        [Test]
        public void AfterNecessaryDependentsAreInvalidated_DependentsUpdatesArePerformedAccordingToPriority()
        {
            int     numberOfDependents = 10;
            IFactor factorBeingTested  = factory.CreateInstance();
            var     interactions       = new MockInteraction[numberOfDependents];

            for (int i = 0; i < numberOfDependents; i++)
            {
                int    copyOfIndex        = i;
                Action updateProcess      = () => EnsureUpdatedInOrder(copyOfIndex);
                var    createdInteraction = new MockInteraction(updateProcess);
                
                interactions[i] = createdInteraction;
                createdInteraction.SetPriority(i);
                createdInteraction.MakeNecessary();
                createdInteraction.MakeValid();
                factorBeingTested.AddDependent(createdInteraction);
                Assert.That(createdInteraction.IsValid, Is.True);
            }

            Assert.That(factorBeingTested.NumberOfDependents, Is.EqualTo(numberOfDependents));

            factorBeingTested.InvalidateDependents();

            for (int i = 0; i < numberOfDependents; i++)
            {
                Assert.That(interactions[i].WasUpdated, Is.True);
            }
            
            TestContext.WriteLine($"Completed for type {typeof(TFactor).Name}");


            void EnsureUpdatedInOrder(int index)
            {
                if (index > 0)
                {
                    var previousInteraction = interactions[index - 1];
                    
                    Assert.That(previousInteraction.WasUpdated, Is.True, 
                        $"The dependent with Priority {previousInteraction.Priority} was not updated before the current " +
                        $"dependent, which has Priority {interactions[index].Priority}. ");
                }

                if (index < numberOfDependents - 1)
                {
                    var nextInteraction = interactions[index + 1];
                    
                    Assert.That(nextInteraction.WasUpdated, Is.False,
                        $"The dependent with Priority {nextInteraction.Priority} was updated before the current " +
                        $"dependent, which has Priority {interactions[index].Priority}. ");
                }
            }
        }



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