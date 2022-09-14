using System;
using Core.Factors;
using Core.States;
using Factors;
using NUnit.Framework;
using Tests.Tools;
using Tests.Tools.Factories;
using Tests.Tools.Factories.Controllers;
using Tests.Tools.Interfaces;
using Tests.Tools.Mocks;
using static Tests.Tools.Tools;
using static Tests.Tools.ErrorMessages;
using static Core.Tools.Types;

namespace Tests.Interface_Tests
{
    
    [TestFixture(typeof(ObservedFunctionResult_Controller))]
    [TestFixture(typeof(DirectFunctionResult_Controller))]
    [TestFixture(typeof(DirectFunctionResult2_Controller))]
    [TestFixture(typeof(DirectFunctionResult3_Controller))]
    [TestFixture(typeof(DirectActionResponse_Controller))]
    [TestFixture(typeof(ObservedActionResponse_Controller)) ]
    public class ITriggeredStates<TController>  where TController : ITriggeredState_Controller, new()
    {
        #region Tests

        [Test]
        public void AfterReacting_HasBeenTriggeredIsFalse()
        {
            TController     controller = new TController();
            ITriggeredState reactorBeingTested = controller.ControlledInstance;

            if (reactorBeingTested.IsTriggered is false)
            {
                reactorBeingTested.Trigger();

                if (reactorBeingTested.IsTriggered is false)
                {
                    Assert.Inconclusive($"The {nameof(IReactor)} could not be put in a triggered state.");
                }
            }
            
            reactorBeingTested.ForceReaction();
            Assert.That(reactorBeingTested.IsTriggered, Is.False, 
                $"{nameof(reactorBeingTested.IsTriggered)} was still true after reacting.");
        }

        [Test]
        public void AfterTriggerIsCalled_HasBeenTriggeredIsTrue()
        {
            TController     controller         = new TController();
            ITriggeredState reactorBeingTested = controller.ControlledInstance;

            if (reactorBeingTested.IsTriggered)
            {
                reactorBeingTested.ForceReaction();
            }
            
            Assert.That(reactorBeingTested.IsTriggered, Is.False);
            reactorBeingTested.Trigger();
            Assert.That(reactorBeingTested.IsTriggered, Is.True, 
                $"The reactor {nameof(reactorBeingTested.IsTriggered)} was still false after calling " +
                $"{nameof(reactorBeingTested.Trigger)}() ");
        }
        
        [Test]
        public void IsReflexive_AfterBeingSet_HasAValueMatchingWhatItWasSetTo()
        {
            TController     controller         = new TController();
            ITriggeredState reactorBeingTested = controller.ControlledInstance;

            reactorBeingTested.IsReflexive = true;
            Assert.That(reactorBeingTested.IsReflexive, Is.True);
            
            reactorBeingTested.IsReflexive = true;
            Assert.That(reactorBeingTested.IsReflexive, Is.True);
                
            reactorBeingTested.IsReflexive = false;
            Assert.That(reactorBeingTested.IsReflexive,  Is.False);
            
            reactorBeingTested.IsReflexive = false;
            Assert.That(reactorBeingTested.IsReflexive,  Is.False);
        }
        
        [Test]
        public void AutomaticallyReacts_AfterBeingSet_HasAValueMatchingWhatItWasSetTo()
        {
            TController     controller         = new TController();
            ITriggeredState reactorBeingTested = controller.ControlledInstance;

            reactorBeingTested.AutomaticallyReacts = true;
            Assert.That(reactorBeingTested.AutomaticallyReacts, Is.True);
            
            reactorBeingTested.AutomaticallyReacts = true;
            Assert.That(reactorBeingTested.AutomaticallyReacts, Is.True);
                
            reactorBeingTested.AutomaticallyReacts = false;
            Assert.That(reactorBeingTested.AutomaticallyReacts,  Is.False);
            
            reactorBeingTested.AutomaticallyReacts = false;
            Assert.That(reactorBeingTested.AutomaticallyReacts,  Is.False);
        }
        
        [Test]
        public void WhileIsReflexiveIsTrue_ReactsWhenTriggered()
        {
            TController     controller         = new TController();
            ITriggeredState reactorBeingTested = controller.ControlledInstance;

            if (reactorBeingTested.AutomaticallyReacts)
            {
                reactorBeingTested.AutomaticallyReacts = false;
            }
            
            if (reactorBeingTested.IsTriggered)
            {
                reactorBeingTested.AttemptReaction();
            }

            Assert.That(reactorBeingTested.IsTriggered, Is.False);

            reactorBeingTested.IsReflexive = true;
            reactorBeingTested.Trigger();
            
            Assert.That(reactorBeingTested.IsTriggered, Is.False);
        }
        
        [Test]
        public void WhileAutomaticallyReactsIsTrue_ReactsWhenTriggered()
        {
            TController     controller         = new TController();
            ITriggeredState reactorBeingTested = controller.ControlledInstance;

            if (reactorBeingTested.IsReflexive)
            {
                reactorBeingTested.IsReflexive = false;
            }
            
            if (reactorBeingTested.IsTriggered)
            {
                reactorBeingTested.AttemptReaction();
            }
            
            Assert.That(reactorBeingTested.IsTriggered, Is.False);

            reactorBeingTested.AutomaticallyReacts = true;
            reactorBeingTested.Trigger();
            
            Assert.That(reactorBeingTested.IsTriggered, Is.False);
        }
        
        [Test]
        public void Destabilize_WhenCalled_WhileIsReflexiveIsFalse_ReturnsFalse()
        {
            TController     controller         = new TController();
            ITriggeredState reactorBeingTested = controller.ControlledInstance;

            if (reactorBeingTested.IsReflexive)
            {
                reactorBeingTested.IsReflexive = false;
            }
            
            Assert.That(reactorBeingTested.IsUnstable,    Is.False);
            Assert.That(reactorBeingTested.Destabilize(), Is.False);
        }
        
        [Test]
        public void Destabilize_WhenCalled_WhileIsReflexiveIsTrue_ReturnsTrue()
        {
            TController     controller         = new TController();
            ITriggeredState reactorBeingTested = controller.ControlledInstance;

            Assert.That(reactorBeingTested.IsUnstable,        Is.False);
            reactorBeingTested.IsReflexive = true;
            Assert.That(reactorBeingTested.Destabilize(), Is.True);
        }

        [Test]
        public void WhenIsReflexive_IsSetToTrue_WhileTriggered_WillReact()
        {
            TController     controller         = new TController();
            ITriggeredState reactorBeingTested = controller.ControlledInstance;

            if (reactorBeingTested.AutomaticallyReacts)
            {
                reactorBeingTested.AutomaticallyReacts = false;
            }
            
            if (reactorBeingTested.IsReflexive)
            {
                reactorBeingTested.IsReflexive = false;
            }
            
            Assert.That(reactorBeingTested.IsReflexive,         Is.False);
            Assert.That(reactorBeingTested.AutomaticallyReacts, Is.False);
            
            reactorBeingTested.Trigger();
            Assert.That(reactorBeingTested.IsTriggered, Is.True);
            Assert.That(reactorBeingTested.IsUnstable,  Is.False);
            
            reactorBeingTested.IsReflexive = true;
            
            Assert.That(reactorBeingTested.IsTriggered, Is.False);
            Assert.That(reactorBeingTested.IsUnstable,  Is.False);
            //- TODO : Is there a better way of testing if the reaction goes off?
        }
        
        
        
        [Test]
        public void WhenAutomaticallyReacts_IsSetToTrue_WhileTriggered_WillReact()
        {
            TController     controller         = new TController();
            ITriggeredState reactorBeingTested = controller.ControlledInstance;

            if (reactorBeingTested.IsReflexive)
            {
                reactorBeingTested.IsReflexive = false;
            }

            if (reactorBeingTested.AutomaticallyReacts)
            {
                reactorBeingTested.AutomaticallyReacts = false;
            }
            
            Assert.That(reactorBeingTested.IsReflexive,         Is.False);
            Assert.That(reactorBeingTested.AutomaticallyReacts, Is.False);
            
            reactorBeingTested.Trigger();
            Assert.That(reactorBeingTested.IsTriggered, Is.True);
            Assert.That(reactorBeingTested.IsUnstable,  Is.False);
            
            reactorBeingTested.AutomaticallyReacts = true;
            
            Assert.That(reactorBeingTested.IsTriggered, Is.False);
            Assert.That(reactorBeingTested.IsUnstable,  Is.False);
            //- TODO : Is there a better way of testing if the reaction goes off?
        }
        
        // [Test]
        // public void AttemptReaction_WhenUnstable_ReactsAndReturnsTrue()
        // {
        //     TController     controller         = new TController();
        //     ITriggeredState reactorBeingTested = controller.ControlledInstance;
        //
        //     if (reactorBeingTested.IsUnstable is false)
        //     {
        //         reactorBeingTested.Destabilize(null);
        //     }
        //     
        //     Assert.That(reactorBeingTested.IsTriggered,       Is.False);
        //     Assert.That(reactorBeingTested.IsUnstable,        Is.True);
        //     
        //     Assert.That(reactorBeingTested.AttemptReaction(), Is.True);
        //     Assert.That(reactorBeingTested.IsTriggered,       Is.False);
        //     Assert.That(reactorBeingTested.IsUnstable,        Is.False);
        // }

        [Test]
        public void AttemptReaction_WhenTriggered_ReactsAndReturnsTrue()
        {
            TController     controller         = new TController();
            ITriggeredState reactorBeingTested = controller.ControlledInstance;

            if (reactorBeingTested.IsTriggered is false)
            {
                controller.SetOffInstancesTriggers();
            }
            
            Assert.That(reactorBeingTested.IsTriggered,       Is.True);
            Assert.That(reactorBeingTested.IsUnstable,        Is.False);
            Assert.That(reactorBeingTested.AttemptReaction(), Is.True);
            Assert.That(reactorBeingTested.IsTriggered,       Is.False);
            Assert.That(reactorBeingTested.IsUnstable,        Is.False);
        }

        public void WhenCreated_IsReflexive_IsFalse()
        {
            
            
        }
        
        #endregion
    }
}