// using System;
// using Causality.Processes;
// using Core.Causality;
// using Core.States;
// using static Tests.Tools;
//
// namespace Tests
// {
//     public abstract class IOutcomeTester
//     {
//         private IOutcome       outcome;
//         private IState         parentState;
//
//         public IState ParentState
//         {
//             get => parentState;
//         }
//
//         public IOutcome Outcome
//         {
//             get => outcome;
//         }
//
//         public IOutcome CreateNewOutcome()
//         {
//             outcome = CreateOutcomeThatDoes(DoNothing);
//             return outcome;
//         }
//         
//         public IState CreateNewParentState()
//         {
//             parentState = createParentState();
//             return parentState;
//         }
//
//         public IOutcome CreateNewOutcomeWithParentState()
//         {
//             var createdParent  = CreateNewParentState();
//             var process        = CreateProcessThatInvolves(createdParent);
//             var createdOutcome = CreateOutcomeThatDoes(process);
//             
//             parentState = createdParent;
//             outcome     = createdOutcome;
//             
//             return createdOutcome;
//         }
//         
//         
//
//         protected abstract IOutcome CreateOutcomeThatDoes(IProcess thingToDo);
//         protected abstract IProcess CreateProcessThatInvolves(IState state);
//         protected abstract IState   CreateDefaultParentState();
//     }
//     
// }