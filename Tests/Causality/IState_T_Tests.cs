using Causality.States;
using Core.Causality;
using Core.States;
using Factors;
using NUnit.Framework;
using Tests.Causality.Factories;
using Tests.Causality.Interfaces;
using Tests.Causality.Mocks;
using static Tests.Tools;

namespace Tests.Causality
{
    [TestFixture(typeof(    State<int>), typeof(    State_Int_Factory), typeof(int))]
    [TestFixture(typeof(   Result<int>), typeof(   Result_Int_Factory), typeof(int))]
    [TestFixture(typeof( Reactive<int>), typeof( Reactive_Int_Factory), typeof(int))]
    [TestFixture(typeof(Proactive<int>), typeof(Proactive_Int_Factory), typeof(int))]
    public class IState_T_Tests<TState, TFactory, TValue>  where TState : IState<TValue>
        where TFactory : IState_T_Factory<TState, TValue>, new()
    {
        private TFactory factory = new TFactory();
        
        [Test]
        public void WhenValueRetrieved_DependencyIsCreated()
        {
            TState stateBeingTested = factory.CreateInstance();
            var    interaction      = new MockInteraction();
            var    process          = new RetrieveValueProcess<TValue>(stateBeingTested);

            AssumeHasNoDependents(stateBeingTested);
            Assert.That(interaction.WasInfluenced, Is.False);

            factory.ObserveProcess(process, interaction);

            Assert.That(stateBeingTested.HasDependents,      Is.True);
            Assert.That(stateBeingTested.NumberOfDependents, Is.EqualTo(1));
            Assert.That(interaction.WasInfluenced,           Is.True);
        }
        
        [Test]
        public void WhenPeekIsUsed_NoDependencyIsCreated()
        {
            TState stateBeingTested = factory.CreateInstance();
            var    interaction      = new MockInteraction();
            var    process          = CreateProcessThatPeeksAtTheValueOf(stateBeingTested);

            AssumeHasNoDependents(stateBeingTested);
            Assert.That(interaction.WasInfluenced, Is.False);

            factory.ObserveProcess(process, interaction);

            Assert.That(stateBeingTested.HasDependents,      Is.False);
            Assert.That(stateBeingTested.NumberOfDependents, Is.Zero);
            Assert.That(interaction.WasInfluenced,           Is.False);
        }
    }
}