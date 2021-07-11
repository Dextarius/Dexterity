using Factors;
using NUnit.Framework;

namespace Tests
{
    public class Reactions
    {
        [Test]
        public void AfterReacting_DoesNotReactWithoutBeingInvalidated()
        {
            int numberOfTimesActionIsRun = 0;
            
            Reaction testReaction = new Reaction(() => numberOfTimesActionIsRun += 1);
            
            testReaction.React();
            
            Assert.That(numberOfTimesActionIsRun, Is.EqualTo(1));

            testReaction.React();
            testReaction.React();
            testReaction.React();
            testReaction.React();
            
            Assert.That(numberOfTimesActionIsRun, Is.EqualTo(1));
        }

        // [Test]
        // public void Reaction_AfterProactorChanges_IsAbleToReact()
        // {
        //
        // }

        //[Test]
        //public void Reaction_AfterProactorChanges_IsAbleToReact()
        //{
        //    
        //}


        // [Test] 
        // public void Reaction_IfAlreadyReacting_DoesNotExecuteAction()
        // {
        //
        // }
    }
}