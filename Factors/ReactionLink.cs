// namespace Factors
// {
//     //- A class that notifies Reactors a given method has been called.  
//     //  I imagine you would call NotifyInvoked() at the start or end of the method,
//     //  so that dependents would be executed there.  It could also be used as a return value
//     //  for methods that would normally return void, providing something a Reaction could use
//     //  to indicate it wanted to register to be called.
//     
//     public class Invocation // : Proactor
//     {
//         public void NotifyInvoked()
//         {
//             InvalidateDependents();
//         }
//         
//         public void ShowInterest()
//         {
//             source.NotifyInvolved;
//         }
//         
//         //- If no one shows interest before we are invoked again, then there were no interested parties?
//         //  Actually, the original caller may have just called another interested Reactor after they invoked
//         //  this and before they themselves showed interest. 
//     }
// }