using Factors;
using NUnit.Framework;
using static Tests.Tools;

namespace Tests.Factors
{
    
    public class All_Factors<TFactor>  where TFactor : Factor
    {
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
    }
}