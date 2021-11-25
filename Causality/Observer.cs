using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Causality.States;
using Core.Causality;
using Core.States;
using Core.Tools;
using JetBrains.Annotations;

namespace Causality
{
    public class Observer<TInfluence, TObserved> : IPausable  
        where TObserved  : IInfluenceable
        where TInfluence : IInfluence
    {
        #region Constants

        private const int    InitialArraySize      = 10;
        private const string NullObjectInvolved    = "A process attempted to submit a null object as being involved in determining an outcome.";
        private const string ProvidedNullOutcome   = "A thread requested a process be observed, but the accompanying Outcome was null.";
        private const string ProvidedNullProcess   = "A thread requested a process be observed, but the process provided was null.";
        private const string CurrentOutcomeIsNull  = "A process attempted to call " + nameof(NotifyInvolved) + "but the " + nameof(Response) + " at the current index was null.";
        private const string ChangedStateIsNull    = "A process attempted to submit a null object as having been changed during an outcome.";
        private const string ObserverAlreadyPaused = "A process attempted to pause the Observer, but it was already paused.";
        private const string ObserverNotPaused     = "A process attempted to resume the Observer, but it was not paused.";
        private const string NextOutcomeIsNotNull  = "A process attempted to move to a new event, but the next event already had an Outcome.";
        // private const string NoCurrentInfluences   = "A process attempted to clear the influences for the current" + 
        //                                                  nameof(ObserverInstance) +" event, but no event was being observed. ";
        #endregion

        
        #region Instance Fields

        [CanBeNull] 
        private TObserved currentSubject;
        private bool      isObservationPaused;
        
        #endregion
        
                  
        #region Properties

        public  bool IsCurrentlyObserving => currentSubject != null  &&  isObservationPaused is false;

        #endregion


        #region Instance Methods

        public void NotifyInvolved(TInfluence involvedObject)
        {
            if (involvedObject == null) { throw new ArgumentNullException(nameof(involvedObject), NullObjectInvolved); }

            if (IsCurrentlyObserving)
            {
                currentSubject.Notify_InfluencedBy(involvedObject);
            }
        }

        //- TODO : Make sure all of the States trigger this.
        public void NotifyChanged(TInfluence changedObject)
        {
            if (changedObject == null) { throw new ArgumentNullException(nameof(changedObject), ChangedStateIsNull); }

            #if DEBUG
            
            if (IsCurrentlyObserving)
            {
                Debug.WriteLine("A factor was changed during an observation.");
            }
            
            #endif
        }

        public void ObserveInteractions(IProcess processToObserve, TObserved outcomeForProcess)
        {
            if (processToObserve  == null) { throw new ArgumentNullException(nameof(processToObserve), ProvidedNullProcess); }
            if (outcomeForProcess == null) { throw new ArgumentNullException(nameof(outcomeForProcess),  ProvidedNullOutcome); }
            
            TObserved outerOutcome         = currentSubject;
            bool      previousPauseState   = isObservationPaused;
            bool      executedSuccessfully = false;
        
            currentSubject = outcomeForProcess;
            isObservationPaused = false;
            //- Pausing the Observation is intended to prevent States from notifying Outcomes that they are involved
            //  while a process is being observed.  But if the process accesses an Outcome that is Invalid, that Outcome
            //  is still going to want to recalculate.  If we kept the observation paused when that that Outcome calls
            //  this method, all of the States that Outcome interacted with would be ignored when they tried to notify
            //  us they were involved, meaning the Outcome would run its process but would have no influences and as a
            //  result never be notified if one of them changed.  That's why we reset the pause state when a process
            //  starts, and replace it once that process finishes.
        
            try
            {
                processToObserve.Execute();
                executedSuccessfully = true;
            }
            finally
            {
                currentSubject = outerOutcome;
        
                if (isObservationPaused && executedSuccessfully)
                {
                    throw new InvalidOperationException(
                        "A process paused the Observer, but did not unpause it before finishing. ");
                }

                isObservationPaused = previousPauseState;
            }
        }
        
        public T ObserveInteractions<T>(IProcess<T> processToObserve, TObserved outcomeForProcess)
        {
            if (processToObserve  == null) { throw new ArgumentNullException(nameof(processToObserve), ProvidedNullProcess); }
            if (outcomeForProcess == null) { throw new ArgumentNullException(nameof(outcomeForProcess),  ProvidedNullOutcome); }
            
            var  outerOutcome         = currentSubject;
            bool previousPauseState   = isObservationPaused;
            bool executedSuccessfully = false;
            T    newValue;

            currentSubject = outcomeForProcess;
            isObservationPaused = false;
        
            try
            {
                newValue = processToObserve.Execute();
                executedSuccessfully = true;
            }
            finally
            {
                if (isObservationPaused && executedSuccessfully)
                {
                    throw new InvalidOperationException(
                        "A process paused the Observer, but did not unpause it before finishing. ");
                }
                
                currentSubject      = outerOutcome;
                isObservationPaused = previousPauseState;
            }
        
            return newValue;
        }
        
        //- TODO: Make sure these work correctly
        public PauseToken PauseObservation()
        {
            if (isObservationPaused) { throw new InvalidOperationException(ObserverAlreadyPaused); }
            
            isObservationPaused = true;
            
            return new PauseToken(this);
        }

        public void ResumeObservation()
        {
            if (isObservationPaused is false) { throw new InvalidOperationException(ObserverNotPaused); }
            
            isObservationPaused = false;
        }

        #endregion

        
        #region Explicit Implementations

        PauseToken IPausable.Pause()   => PauseObservation();
        void       IPausable.Unpause() => ResumeObservation();

        #endregion
    }

    
}