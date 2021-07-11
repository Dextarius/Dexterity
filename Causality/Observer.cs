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
        private const string NullObjectInvolved  = "A process attempted to submit a null object as being involved in determining an outcome.";
        private const string ProvidedNullOutcome = "A thread requested a process be observed, but the accompanying Outcome was null.";
        private const string ProvidedNullProcess = "A thread requested a process be observed, but the process provided was null.";
        
        //^ TODO : Decide on whether to use constants or not, but whatever we decide it should be consistent.

        [ThreadStatic] private static CausalEvent        currentEvent;
        [ThreadStatic] private static Stack<CausalEvent> eventPool;
        [ThreadStatic] private static bool               isObservationPaused; 
        //^ We changed this from isObservationActive to isObservationPaused because there's no easy way
        //  to set the value of a ThreadStatic field, now when it will behave appropriately because the value will default to false.

        [NotNull, ItemNotNull]
        private static Stack<CausalEvent> EventPool            => eventPool?? (eventPool = new Stack<CausalEvent>());
        public static bool                IsCurrentlyObserving => currentEvent != null  &&  isObservationPaused is false;

        public static void NotifyInvolved(IState involvedState)
        {
            if (involvedState == null) { throw new ArgumentNullException(nameof(involvedState), NullObjectInvolved); }

            if (IsCurrentlyObserving)
            {
                currentEvent.AddInfluence(involvedState);
            }
        }

        //- TODO : Make sure all of the Proactors trigger this.
        public static void NotifyChanged(IState involvedState)
        {
            if (involvedState == null) { throw new ArgumentNullException(nameof(involvedState), 
                "A process attempted to submit a null object as having been changed during an outcome."); }

            if (IsCurrentlyObserving)
            {
                Debug.WriteLine("A factor was changed during an observation.");
            }
        }
        
        public static void ObserveInteractions(IProcess processToObserve, IOutcome outcomeToModify)
        {
            if (processToObserve == null) { throw new ArgumentNullException(nameof(processToObserve), ProvidedNullProcess); }
            if (outcomeToModify  == null) { throw new ArgumentNullException(nameof(outcomeToModify),  ProvidedNullOutcome); }
            
            StartNewEvent(outcomeToModify);
            
            try     { processToObserve.Execute(); }
            finally { ConcludeCurrentEvent();     }
        }

        public static T ObserveInteractions<T>(IProcess<T> processToObserve, IOutcome outcomeToModify)
        {
            if (processToObserve == null) { throw new ArgumentNullException(nameof(processToObserve), ProvidedNullProcess); }
            if (outcomeToModify  == null) { throw new ArgumentNullException(nameof(outcomeToModify),  ProvidedNullOutcome); }
            
            StartNewEvent(outcomeToModify);
            
            try     { return processToObserve.Execute(); }
            finally { ConcludeCurrentEvent();            }
        }

        private static void StartNewEvent(IOutcome eventOutcome)
        {
            CausalEvent newEvent = (EventPool.Count > 0)? EventPool.Pop() : 
                                                          new CausalEvent();
            newEvent.Outcome    = eventOutcome;
            newEvent.PriorEvent = currentEvent;
            currentEvent        = newEvent;
        }

        //- TODO: Make sure these work correctly
        public static PauseToken PauseObservation()
        {
            if (isObservationPaused) {
                throw new InvalidOperationException("A process attempted to pause the Observer, but it was already paused."); }

            isObservationPaused = true;
            
            return new PauseToken();
        }
        
        public static void ResumeObservation()
        {
            if (isObservationPaused is false) {
                throw new InvalidOperationException("A process attempted to resume the Observer, but it was not paused."); }

            isObservationPaused = false;
        }

        private static void ConcludeCurrentEvent()
        {
            CausalEvent formerEvent = currentEvent;
            
            currentEvent = formerEvent.Conclude();
            eventPool.Push(formerEvent);
        }
    }
}