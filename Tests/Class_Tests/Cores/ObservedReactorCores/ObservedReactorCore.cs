using System;
using Factors;
using NUnit.Framework;

namespace Tests.Class_Tests.Cores.ObservedReactorCores
{
    public abstract class ObservedReactorCores
    {
        [Test]
        public void Constructor_WhenGivenNullDelegate_ThrowsException() => 
            Assert.Throws<ArgumentNullException>(() => new Reactive<int>((Func<int>) null));
        
        public void IfReactingWithoutBeingTriggered_PreviousInfluencesAreRemoved()
        {
            
        }
    }
}