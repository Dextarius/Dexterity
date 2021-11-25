// using System;
// using System.Diagnostics;
// using Core.Causality;
// using JetBrains.Annotations;
// using Core;
// using static Core.Config;
//
// namespace Causality
// {
//     public static class UpdateHandler
//     {
//         //- TODO : Decide if this really belongs in the Causality project.  I'm not sure that how we execute the updates needs
//         //         to be coupled to the Causality system.
//         //- TODO : We need to make sure something actually executes the updates returned by StopQueuingUpdates().
//         //         The ConsequenceTracker might actually work well for that since it knows when updates start and end.
//         //- TODO : We should provide some way to forward the updates to a derived handler so that UI updates can be handled separately
//         //- TODO : Consider reworking this to accept instances of the IProcess interface, instead of just Actions 
//
//         #region Static Fields
//         
//         /// <summary>
//         ///     The Action which the UpdateHandler uses to request that a given update be performed asynchronously by the system.
//         /// </summary>
//         /// <remarks>
//         ///     In WPF, the Action stored in this field should call the Dispatcher assigned to the UI thread.
//         ///     This is because most UI elements are DispatcherObjects which can only be modified by the Dispatcher for the
//         ///     thread they are created on.
//         /// </remarks>
//         private static Action<Action> updateExecutionProcess;
//
//         /// <summary>
//         ///     A <see cref="ThreadStaticAttribute"/> field that holds the <see cref="UpdateHandler"/> for this thread if any.
//         /// </summary>
//         [ThreadStatic] private static UpdateQueue queueForThisThread;
//
//         #endregion
//
//
//         #region Static Methods
//
//         /// <summary>  Requests the <see cref="UpdateHandler"/> queue updates in the returned queue until the queue signals to
//         ///            to stop the queuing. If no <see cref="UpdateScheduler"/> is assigned to this thread, one is created and returned,
//         ///            otherwise returns null.
//         /// </summary>
//         /// <returns>  The <see cref="UpdateHandler"/> created, or null if one is already assigned to this thread.  </returns>
//         /// <remarks>  If there is already a queue assigned to this thread it means something else is already queuing the
//         ///            updates and plans to execute them later, so there's no need for us to do it as well.
//         /// </remarks>
//         [CanBeNull]
//         public static IUpdateQueue RequestQueuing() => (queueForThisThread == null)? 
//                                                             queueForThisThread = new UpdateQueue()  :  
//                                                             null;  //- The variable is ThreadStatic, so we don't need locks.
//
//         /// <summary> Sends an update to the <see cref="UpdateHandler"/> so that it can be executed either asynchronously by the UI thread, 
//         ///           or manually from the <see cref="IEnumerable{Action}"/> returned by <see cref="StopQueuingUpdates"/>. </summary>
//         /// <param name="requestedUpdate"> An <see cref="Action"/> that you want executed.                                 </param>
//         public static void RequestUpdate(Action requestedUpdate)
//         {
//             Debug.Assert(updateExecutionProcess != null);
//             
//             bool updatesAreBeingQueued = queueForThisThread != null;
//
//             if (queueForThisThread != null) //- If there is a Handler for this thread, something is queuing updates, 
//             {
//                 queueForThisThread.AddUpdate(requestedUpdate); //-  so add our update to that queue.
//             }
//             else if (updateExecutionProcess != null) //- Else if updateExecutionProcess has been set,
//             {
//                 updateExecutionProcess(requestedUpdate); //- use that to have the update executed asynchronously
//             }
//             else /* Else the updateExecutionProcess hasn't been initialized somehow and we need to find out why. */
//             {
//
//                 throw new InvalidOperationException($"A process attempted to request an update be executed by the "       +
//                                                     $"{nameof(UpdateHandler)} class, but  the update execution delegate " +
//                                                     $"was never set. This delegate is obtained from"                      +
//                                                     $"{nameof(Config)}.{nameof(ActiveExecutionProvider)}");
//             }
//         }
//
//         internal static void NotifyQueueEnded(UpdateQueue queueThatEnded)
//         {
//             if (queueThatEnded == queueForThisThread)
//             {
//                 queueForThisThread = null;
//             }
//             else
//             {
//                 throw new InvalidOperationException($"An update queue attempted to notify the {nameof(UpdateHandler)} " +
//                                                     $"that it had ended, but it was not the queue assigned to that thread. ");
//             }
//         }
//
//         #endregion
//
//
//         #region Constructors
//
//         //- TODO : Ensure nothing needs to update before this can be run.
//         static UpdateHandler()
//         {
//             if (ActiveExecutionProvider == null)
//             {
//                 throw new InvalidOperationException($"A process attempted to initialize the static constructor for " +
//                                                     $"the {nameof(UpdateHandler)} class, but "                               +
//                                                     $"{nameof(Config)}.{nameof(ActiveExecutionProvider)} was null, "         +
//                                                     "and most likely never set.");
//             }
//
//             Action<Action> updateProcess = ActiveExecutionProvider.GetUpdateExecutionProcess();
//
//             if (updateProcess == null)
//             {
//                 throw new InvalidOperationException($"A process attempted to initialize the {nameof(UpdateHandler)}, " +
//                                                     $"but the delegate it got from the {nameof(ActiveExecutionProvider)}"      +
//                                                     $".{nameof(ActiveExecutionProvider.GetUpdateExecutionProcess)}"            +
//                                                     $" was null.");
//             }
//
//             updateExecutionProcess = updateProcess;
//         }
//
//         #endregion
//     }
// }