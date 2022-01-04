using NUnit.Framework;

namespace Tests
{
    public class Reactor_Tests
    {
        [Test]
        public void AfterConstruction_WeakReferenceReturnsAReferenceToTheOwner()
        {
            TResult interaction = resultFactory.CreateInstance();

            interaction.WeakReference.TryGetTarget(out var referenceTarget);
            Assert.That(referenceTarget, Is.SameAs(interaction));
        }
        
        [Test]
        public void WhenAProactivesValueIsRetrieved_DependencyIsCreated()
        {
            TState stateBeingTested = factory.CreateInstance();
            var    interaction      = new MockInteraction();
            var    process          = new RetrieveValueResult<TValue>(stateBeingTested);

            AssumeHasNoDependents(stateBeingTested);
            Assert.That(interaction.WasInfluenced, Is.False);

            factory.ObserveProcess(process, interaction);

            Assert.That(stateBeingTested.HasDependents,      Is.True);
            Assert.That(stateBeingTested.NumberOfDependents, Is.EqualTo(1));
            Assert.That(interaction.WasInfluenced,           Is.True);
        }
    }
}