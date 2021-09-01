using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Causality.States;
using Core.Causality;
using JetBrains.Annotations;
using static Causality.Observer;

namespace Causality
{
    public class ObserverInstance
    {
        #region Constants

        private const int    InitialArraySize      = 10;
        private const string NullObjectInvolved    = "A process attempted to submit a null object as being involved in determining an outcome.";
        private const string ProvidedNullOutcome   = "A thread requested a process be observed, but the accompanying Outcome was null.";
        private const string ProvidedNullProcess   = "A thread requested a process be observed, but the process provided was null.";
        private const string CurrentOutcomeIsNull  = "A process attempted to call " + nameof(AddInfluence) + "but the " + nameof(Outcome) + " at the current index was null.";
        private const string ChangedStateIsNull    = "A process attempted to submit a null object as having been changed during an outcome.";
        private const string ObserverAlreadyPaused = "A process attempted to pause the Observer, but it was already paused.";
        private const string ObserverNotPaused     = "A process attempted to resume the Observer, but it was not paused.";
        private const string NextOutcomeIsNotNull  = "A process attempted to move to a new event, but the next event already had an Outcome.";
        private const string NoCurrentInfluences   = "A process attempted to clear the influences for the current" + 
                                                         nameof(ObserverInstance) +" event, but no event was being observed. ";
        #endregion
        
        
        #region Instance Fields

        [NotNull] private HashSet<IState>[] influences = new HashSet<IState>[InitialArraySize];
        [NotNull] private IOutcome[]        outcomes   = new IOutcome[InitialArraySize];
                  private int               indexOfCurrentEvent = -1;
                  private bool              isObservationPaused;

        #endregion
        
        
        #region Properties

        public  bool IsCurrentlyObserving => indexOfCurrentEvent >= 0  &&  isObservationPaused is false;
      //  private CausalEvent CurrentEvent => events[indexOfCurrentEvent];

        #endregion


        #region Instance Methods

        public void NotifyInvolved(IState involvedState)
        {
            if (involvedState == null) { throw new ArgumentNullException(nameof(involvedState), NullObjectInvolved); }

            if (IsCurrentlyObserving)
            {
                AddInfluence(involvedState);
            }
        }
        
        private void AddInfluence([NotNull] IState contributingFactor)
        {
            var currentOutcome = outcomes[indexOfCurrentEvent];
            
            Debug.Assert(currentOutcome != null, CurrentOutcomeIsNull);

            if (contributingFactor.IsInvalid)
            {
                currentOutcome.Invalidate(contributingFactor);
            }
            else if (currentOutcome.IsValid  &&  
                     contributingFactor.AddDependent(currentOutcome))
            {
                influences[indexOfCurrentEvent].Add(contributingFactor);
            }
        }

        //- TODO : Make sure all of the Proactors trigger this.
        public void NotifyChanged(IState involvedState)
        {
            if (involvedState == null) { throw new ArgumentNullException(nameof(involvedState), ChangedStateIsNull); }

            if (IsCurrentlyObserving)
            {
                Debug.WriteLine("A factor was changed during an observation.");
            }
        }
        
        public void ObserveInteractions(IProcess processToObserve, IOutcome outcomeToModify)
        {
            if (processToObserve == null) { throw new ArgumentNullException(nameof(processToObserve), ProvidedNullProcess); }
            if (outcomeToModify  == null) { throw new ArgumentNullException(nameof(outcomeToModify),  ProvidedNullOutcome); }
            
            StartNewEvent(outcomeToModify);
            
            try     { processToObserve.Execute(); }
            finally { ConcludeCurrentEvent();     }
        }

        public  T ObserveInteractions<T>(IProcess<T> processToObserve, IOutcome outcomeToModify)
        {
            if (processToObserve == null) { throw new ArgumentNullException(nameof(processToObserve), ProvidedNullProcess); }
            if (outcomeToModify  == null) { throw new ArgumentNullException(nameof(outcomeToModify),  ProvidedNullOutcome); }
            
            StartNewEvent(outcomeToModify);
            
            try     { return processToObserve.Execute(); }
            finally { ConcludeCurrentEvent();            }
        }
        
        private  void StartNewEvent(IOutcome eventOutcome)
        {
            var indexOfNextEvent = MoveToNewEvent();

            outcomes[indexOfNextEvent] = eventOutcome;
            indexOfCurrentEvent        = indexOfNextEvent;
        }

        private int MoveToNewEvent()
        {
            var indexOfNextEvent = indexOfCurrentEvent + 1;

            while (indexOfNextEvent >= outcomes.Length)
            {
                ExpandArrays();
            }

            if (influences[indexOfNextEvent] is null) { influences[indexOfNextEvent] = new HashSet<IState>(); }
            if (  outcomes[indexOfNextEvent] != null) { throw new InvalidOperationException(NextOutcomeIsNotNull); }

            return indexOfNextEvent;
        }

        private void ExpandArrays()
        {
            var oldInfluencesArray = influences;
            var oldOutcomeArray    = outcomes;
            var newInfluencesArray = new HashSet<IState>[influences.Length * 2];
            var newOutcomeArray    = new IOutcome[outcomes.Length * 2];

            oldInfluencesArray.CopyTo(newInfluencesArray, 0);
            oldOutcomeArray.CopyTo(newOutcomeArray, 0);
            influences = newInfluencesArray;
            outcomes   = newOutcomeArray;
        }

        //- TODO: Make sure these work correctly
        public PauseToken PauseObservation()
        {
            if (isObservationPaused) { throw new InvalidOperationException(ObserverAlreadyPaused); }
            isObservationPaused = true;
            
            return new PauseToken();
        }

        public void ResumeObservation()
        {
            if (isObservationPaused is false) { throw new InvalidOperationException(ObserverNotPaused); }
            isObservationPaused = false;
        }

        private  void ConcludeCurrentEvent()
        {
            ref HashSet<IState> eventInfluences = ref influences[indexOfCurrentEvent];
            ref IOutcome        eventOutcome    = ref outcomes[indexOfCurrentEvent];
                IState[]        factors;

            if (eventInfluences.Count > 0  &&  eventOutcome.IsValid)
            {
                factors = eventInfluences.ToArray();
            }
            else
            {
                factors = Array.Empty<IState>();
            }
            
            eventOutcome.SetInfluences(factors);
            eventInfluences.Clear();
            eventOutcome = null;
            indexOfCurrentEvent--;
        }

        public void ClearInfluences()
        {
            if (indexOfCurrentEvent < 0) { throw new InvalidOperationException(NoCurrentInfluences); }
            influences[indexOfCurrentEvent].Clear();
        }

        //public void CreateTemporaryEvent()
        //{
        //    ConditionalToken dd = new ConditionalToken(new CausalEvent());
        //}

        #endregion
    }
}