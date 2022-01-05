using Factors;
using NUnit.Framework;
using Tests.Tools.Interfaces;
using static Tests.Tools.Tools;

namespace Tests
{
    // [TestFixture(typeof(Proactive<int>), typeof(Proactive_Int_Factory))]
    // [TestFixture(typeof(Reactive<int> ), typeof(Reactive_Int_Factory ))]
    // [TestFixture(typeof(Reaction      ), typeof(ReactionFactory      ))]
    public class Factors<TFactor, TFactory> 
        where TFactor  : Factor<TCore>
        where TFactory : IFactory<TFactor>, new()
    {
        #region Instance Fields

        private TFactory factory = new TFactory();

        #endregion
        

        #region Tests
        
        [Test]
        public void WhenGivenANameDuringConstruction_HasThatName()
        {
            string         givenName            = "Some Factor";
            Proactive<int> proactiveBeingTested = new Proactive<int>(42, givenName);
            string         actualName           = proactiveBeingTested.Name; 
            
            Assert.That(actualName, Is.EqualTo(givenName));
            WriteNameAndValueToTestContext(givenName, actualName);
            
            //- TODO : Test the other constructors.
        }
        
        [Test]
        public void WhenCreated_HasDependentsIsFalse()
        {
            Factor factorBeingTested = factory.CreateInstance();

            Assert.That(factorBeingTested.HasSubscribers, Is.False);
        }
        
        [Test]
        public void WhenCreated_NumberOfDependentsIsZero()
        {
            Factor factorBeingTested = factory.CreateInstance();

            Assert.That(factorBeingTested.NumberOfSubscribers, Is.Zero);
        }
        
        [Test]
        public void WhenCreated_IsNotNecessary()
        {
            Factor factorBeingTested = factory.CreateInstance();

            Assert.That(factorBeingTested.IsNecessary, Is.False);
        }
        
        #endregion
        
        
        //protected abstract TFactor[] CallAllConstructors
    }
}