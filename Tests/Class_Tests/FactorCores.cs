using Core.Factors;
using Factors;
using NUnit.Framework;
using Tests.Tools.Interfaces;
using static Tests.Tools.Tools;

namespace Tests.Class_Tests
{
    public abstract class FactorCores<TFactor, TCore, TFactory> 
        where TFactor  : Factor<TCore>
        where TCore    : IFactor
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
        public void Constructor_WhenGivenNullName_UsesADefault()
        {
            Reactive<int> testReactive = new Reactive<int>(Return42, null);

            Assert.NotNull(testReactive.Name);
        }
        
        [Test]
        public void WhenCreated_HasDependentsIsFalse()
        {
            TFactor factorBeingTested = factory.CreateInstance();

            Assert.That(factorBeingTested.HasSubscribers, Is.False,
                $"The property {nameof(factorBeingTested.HasSubscribers)} was marked as true during construction.");
        }
        
        [Test]
        public void WhenCreated_NumberOfDependentsIsZero()
        {
            TFactor factorBeingTested = factory.CreateInstance();

            Assert.That(factorBeingTested.NumberOfSubscribers, Is.Zero);
        }
        
        [Test]
        public void WhenCreated_IsNotNecessary()
        {
            TFactor factorBeingTested = factory.CreateInstance();

            Assert.That(factorBeingTested.IsNecessary, Is.False);
        }
        
        #endregion
        
        
        //protected abstract TFactor[] CallAllConstructors
    }
}