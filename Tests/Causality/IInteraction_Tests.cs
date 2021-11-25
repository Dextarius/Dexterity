using Causality.Processes;
using Causality.States;
using Core.Causality;
using Core.States;
using Factors;
using Tests.Causality.Factories;
using NUnit.Framework;
using Tests.Causality.Interfaces;
using static Tests.Tools;

namespace Tests.Causality
{
    [TestFixture(typeof(Response   ), typeof(Response_Factory  ))]
    [TestFixture(typeof(Result<int>), typeof(Result_Int_Factory))]
    
    public class IInteraction_Tests<TResult, TFactory> 
        where TResult  : IInteraction
        where TFactory : IFactory<TResult>, new()
    {
        private TFactory resultFactory = new TFactory();

        [Test]
        public void AfterConstruction_WeakReferenceReturnsAReferenceToTheOwner()
        {
            TResult interaction = resultFactory.CreateInstance();

            interaction.WeakReference.TryGetTarget(out var referenceTarget);
            Assert.That(referenceTarget, Is.SameAs(interaction));
        }
    }
}