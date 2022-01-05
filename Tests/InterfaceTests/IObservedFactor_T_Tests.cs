using Core.Factors;
using Factors;
using Factors.Outcomes.ObservedOutcomes;
using NUnit.Framework;
using Tests.Tools.Factories;
using Tests.Tools.Interfaces;
using Tests.Tools.Mocks;
using static Tests.Tools.Tools;

namespace Tests.InterfaceTests
{
    [TestFixture(typeof(    State<int>),    typeof(    Factor_Int_Factory), typeof(int))]
    [TestFixture(typeof( ObservedResult<int>), typeof(  ), typeof(int))]
    [TestFixture(typeof( Reactive<int>),    typeof( Reactive_Int_Factory),  typeof(int))]
    [TestFixture(typeof(Proactive<int>),    typeof(Proactive_Int_Factory),  typeof(int))]
    public class IObservedFactor_T_Tests<TFactor, TFactory, TValue>  
        where TFactor  : IFactor<TValue>, IInvolved
        where TFactory : IFactor_T_Factory<TFactor, TValue>, new()
    {
        private TFactory factory = new TFactory();
        
        [Test]
        public void WhenValueIsRetrieved_NoDependencyIsCreated()
        {
            TFactor factorBeingTested          = factory.CreateInstance();
            var     dependent                  = new MockObserved();
            var     process                    = CreateProcessThatPeeksAtTheValueOf(factorBeingTested);
            int     previousNumberOfDependents = factorBeingTested.NumberOfSubscribers;
            
            Assert.That(dependent.WasInfluenced, Is.False);


            Assert.That(factorBeingTested.HasSubscribers,      Is.False);
            Assert.That(factorBeingTested.NumberOfSubscribers, Is.Zero);
            Assert.That(dependent.WasInfluenced,              Is.False);
        }
        
        [Test]
        public void WhenPeekIsUsed_NoDependencyIsCreated()
        {
            TFactor factorBeingTested          = factory.CreateInstance();
            var     dependent                  = new MockObserved();
            var     process                    = CreateProcessThatPeeksAtTheValueOf(factorBeingTested);
            int     previousNumberOfDependents = factorBeingTested.NumberOfSubscribers;
            
            Assert.That(dependent.WasInfluenced, Is.False);


            Assert.That(factorBeingTested.HasSubscribers,      Is.False);
            Assert.That(factorBeingTested.NumberOfSubscribers, Is.Zero);
            Assert.That(dependent.WasInfluenced,           Is.False);
        }
    }
}