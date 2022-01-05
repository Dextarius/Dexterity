using System;
using Core.Factors;
using Factors;
using NUnit.Framework;
using Tests.Tools;
using Tests.Tools.Mocks;
using static Tests.Tools.Tools;

namespace Tests.Integration
{
    public class ProactiveReactiveInteractions
    {
        [Test]
        public void Proactive_WhenValueRetrievedDuringAReaction_GainsReactorAsADependent()
        {
            Proactive<int> proactiveBeingTested = new Proactive<int>(42);
            Reactive<int>  dependentReactive    = new Reactive<int>(() => proactiveBeingTested);
            int            triggerValueUpdate   = dependentReactive.Value;

            Assert.That(proactiveBeingTested.HasSubscribers);
        }
        
        [Test]
        public void WhenValueSet_NoLongerHasDependents()
        {
            Proactive<int> proactiveBeingTested       = new Proactive<int>(5);
            Reactive<int>  reactiveThatRetrievesValue = CreateReactiveThatGetsValueOf(proactiveBeingTested);

            Assert.That(proactiveBeingTested.HasSubscribers, Is.False, 
                ErrorMessages.HasDependents<Proactive<int>>("before being used. "));
            
            int triggerAReaction = reactiveThatRetrievesValue.Value;

            Assert.That(proactiveBeingTested.HasSubscribers, Is.True, 
                ErrorMessages.FactorDidNotHaveDependents<Proactive<int>>("despite being used to calculate a value. "));
            
            proactiveBeingTested.Value = 10;

            Assert.That(proactiveBeingTested.HasSubscribers, Is.False, 
                ErrorMessages.HasDependents<Proactive<int>>("despite its value changing. "));
        }
        
        [Test]
        public void AfterNecessaryDependentsAreInvalidated_DependentsUpdatesArePerformedAccordingToPriority()
        {
            int     numberOfDependents = 10;
            IFactor factorBeingTested  = factory.CreateInstance();
            var     interactions       = new MockFactorSubscriber[numberOfDependents];

            for (int i = 0; i < numberOfDependents; i++)
            {
                int    copyOfIndex        = i;
                Action updateProcess      = () => EnsureUpdatedInOrder(copyOfIndex);
                var    createdInteraction = new MockFactorSubscriber(updateProcess);
                
                interactions[i] = createdInteraction;
                createdInteraction.SetPriority(i);
                createdInteraction.MakeNecessary();
                createdInteraction.MakeValid();
                factorBeingTested.Subscribe(createdInteraction);
                Assert.That(createdInteraction.IsValid, Is.True);
            }

            Assert.That(factorBeingTested.NumberOfSubscribers, Is.EqualTo(numberOfDependents));

            factorBeingTested.TriggerSubscribers();

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
    }
    }
}