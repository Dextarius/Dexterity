using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Factors;
using NUnit.Framework;
using static Core.Tools.Types;
using static Tests.Tools;

namespace Tests
{
    public class Proactives
    {
        [Test]
        public void WhenValueRetrievedDuringAReaction_BecomesConsequential()
        {
            Proactive<int> proactiveBeingTested = new Proactive<int>(42);
            Reactive<int>  dependentReactive    = new Reactive<int>(() => proactiveBeingTested);
            int            triggerValueUpdate   = dependentReactive.Value;

            Assert.That(proactiveBeingTested.HasDependents);
        }

        [Test]
        public void WhenValueSet_IsNoLongerConsequential()
        {
            Proactive<int> proactiveBeingTested       = new Proactive<int>(5);
            Reactive<int>  reactiveThatRetrievesValue = CreateReactiveThatGetsValueOf(proactiveBeingTested);

            Assert.That(proactiveBeingTested.HasDependents, Is.False, ErrorMessages.HasDependents<Proactive<int>>("before being used. "));
            int triggerAReaction = reactiveThatRetrievesValue.Value;

            Assert.That(proactiveBeingTested.HasDependents, Is.True, ErrorMessages.FactorDidNotHaveDependents<Proactive<int>>("despite being used to calculate a value. "));
            proactiveBeingTested.Value = 10;

            Assert.That(proactiveBeingTested.HasDependents, Is.False, ErrorMessages.HasDependents<Proactive<int>>("despite its value changing. "));
        }


        [Test]
        public void WhenGivenAValueEqualToCurrentValue_DependentsAreNotInvalidated()
        {
            int            initialValue         = 5;
            Proactive<int> proactiveBeingTested = new Proactive<int>(initialValue);
            Reactive<int>  dependentReactive    = CreateReactiveThatGetsValueOf(proactiveBeingTested);

            Assert.False(proactiveBeingTested.HasDependents,
                $"The {NameOf<Proactive<int>>()} was marked as consequential before being used. ");

            int triggerAReaction = dependentReactive.Value;

            Assert.That(dependentReactive.IsValid, ErrorMessages.ReactorWasNotValid<Reactive<int>>());
            Assert.That(proactiveBeingTested.HasDependents,
                ErrorMessages.FactorDidNotHaveDependents<Proactive<int>>("despite being used to calculate a value. "));

            proactiveBeingTested.Value = 5;

            Assert.That(dependentReactive.IsValid,
                $"Setting the value of a {NameOf<Proactive<int>>()} invalidated its dependents even though " +
                "the value set was equal to the old value. ");
            Assert.That(proactiveBeingTested.HasDependents,
                ErrorMessages.FactorDidNotHaveDependents<Proactive<int>>("despite being used to calculate a value. "));
        }

        //- This may seem like a strange test, but the nature of Proactives is to store references to the
        //  things they affect, and the nature of Reactives to store reference to things that affect them
        //  can easily lead to a bunch of objects never being collected because they all reference each other.
        //  Both classes are structured in a way that prevents them from creating circular references
        //  to each other, and this is here to make sure that is/remains true. 
        [Test]
        public void WhenAffectedFactorsNoLongerInUse_CanBeGarbageCollected()
        {
            HashSet<WeakReference<Reactive<int>>> reactiveReferences = new HashSet<WeakReference<Reactive<int>>>();
            WeakReference<Proactive<int>>         proactiveReference = GenerateChainOfFactors(reactiveReferences);

            GC.Collect();
            Thread.Sleep(1000);

            foreach (var reference in reactiveReferences)
            {
                reference.TryGetTarget(out var reactive);
                Assert.That(reactive, Is.Null);
            }

            proactiveReference.TryGetTarget(out var proactive);
            Assert.That(proactive, Is.Null);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static WeakReference<Proactive<int>> GenerateChainOfFactors(HashSet<WeakReference<Reactive<int>>> references)
        {
            Proactive<int>                proactiveValue       = new Proactive<int>(5);
            WeakReference<Proactive<int>> referenceToProactive = new WeakReference<Proactive<int>>(proactiveValue);
            Reactive<int>                 createdReactive      = CreateReactiveThatGetsValueOf(proactiveValue);
            int                           triggerAReaction     = createdReactive.Value;

            for (int i = 0; i < 3; i++)
            {
                createdReactive = CreateReactiveThatDependsOn(createdReactive);
                references.Add(new WeakReference<Reactive<int>>(createdReactive));
            }

            foreach (var reference in references)
            {
                reference.TryGetTarget(out var reactive);
                Assert.That(reactive, Is.Not.Null);
            }

            Assert.That(proactiveValue.HasDependents);

            return referenceToProactive;
        }
    }
}