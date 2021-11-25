// namespace Tests.Causality
// {
//     public class Factor_Tests
//     {
//         [Test]
//         public void WhenGivenANameDuringConstruction_HasThatName()
//         {
//             string      givenName         = "Some Factor";
//             Factor factorBeingTested = new Factor<int>(42, givenName);
//             string      actualName        = factorBeingTested.Name;
//
//             Assert.That(actualName, Is.EqualTo(givenName));
//             TestContext.WriteLine($"Expected Value => {givenName},\nActual Value => {actualName}");
//
//             //- TODO : Test the other constructors.
//         }
//         
//         
//     }
//     
//     
// }