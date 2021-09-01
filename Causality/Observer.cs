using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Causality.States;
using Core.Causality;
using Core.Factors;
using JetBrains.Annotations;

namespace Causality
{
    public static partial class Observer
    {
        #region Static Fields

        [ThreadStatic] private static ObserverInstance observerForThread;
                       private static long             revisionNumber = long.MinValue; 
        //- Can a revision number ever be lower than what an up to date Outcome has?  
        //- If not then we could just use Observer.revisionNumber != Outcome.revisionNumber to see if it's changed.

        #endregion
        
        
        //^ We changed this from isObservationActive to isObservationPaused because there's no easy way
        //  to set the value of a ThreadStatic field, now when it will behave appropriately because the value will default to false.

        //- Could we use a SortedDictionary to allow us to keep outcomes in a queue that would also allow us to
        //  check if they're already in progress without having to iterate over all of the active Events?
        
        //- If we can figure out the issue of making a ThreadStatic flow across async bounds,
        //  we could just implement this as recursive stack calls.

        


        #region Static Properties

        private static ObserverInstance Instance => observerForThread??  (observerForThread = new ObserverInstance());
        public  static bool IsCurrentlyObserving => Instance.IsCurrentlyObserving;

        #endregion


        public static void NotifyInvolved(IState involvedState) => Instance.NotifyInvolved(involvedState);
        public static void  NotifyChanged(IState involvedState) => Instance.NotifyChanged(involvedState);
        public static PauseToken  PauseObservation() => Instance.PauseObservation();
        public static void       ResumeObservation() => Instance.ResumeObservation();
        public static void         ClearInfluences() => Instance.ClearInfluences();


        public static void ObserveInteractions(IProcess processToObserve, IOutcome outcomeToModify) => 
            Instance.ObserveInteractions(processToObserve, outcomeToModify);

        public static T ObserveInteractions<T>(IProcess<T> processToObserve, IOutcome outcomeToModify) =>
            Instance.ObserveInteractions(processToObserve, outcomeToModify);
    }
}