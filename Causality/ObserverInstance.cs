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
        
        public void NotifyInvolved2(IState involvedState, bool isValid)
        {
            if (involvedState == null) { throw new ArgumentNullException(nameof(involvedState), NullObjectInvolved); }

            if (IsCurrentlyObserving)
            {
                AddInfluence2(involvedState, isValid);
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
        
        private void AddInfluence2([NotNull] IState contributingFactor, bool isValid)
        {
            var currentOutcome = outcomes[indexOfCurrentEvent];
            
            Debug.Assert(currentOutcome != null, CurrentOutcomeIsNull);

            if (isValid)
            {
                if (currentOutcome.IsValid &&
                    contributingFactor.AddDependent(currentOutcome))
                {
                    influences[indexOfCurrentEvent].Add(contributingFactor);
                }
            }
            else
            {
                currentOutcome.Invalidate(contributingFactor);
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


    public class OutcomeTracker
    {
        public static OutcomeTracker Default;
        
        private IOutcome[] outcomes;
        private int[]      states;
        private int        nextAvailableId;


        public int RequestStateId()
        {
            int idToGive = nextAvailableId;

            nextAvailableId = states[idToGive];
            
            return nextAvailableId;
        }

        public ref int   GetState(int id) => ref states[id];

        public int EraseState(int id) => states[id] = 0;
        
        
        
        public bool Add(IOutcome outcome)
        {
            
        }
        
        
        private void ExpandArray()
        {
            var oldArray = outcomes;
            var newArray = new IOutcome[oldArray.Length * 2];
        
            Array.Copy(oldArray, newArray, oldArray.Length);
            outcomes = newArray;
        }
    }

    public class TestOutcome : State
    {
        private int stateNumber;
        private int matrixNodeX;
        private int matrixNodeY;

        
        public bool Invalidate()
        {
            if (stateNumber != -1)
            {
                OutcomeTracker.Default.EraseState(stateNumber);
                stateNumber = -1;
                
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Invalidate(int currentDepth, int currentWidth)
        {
            var node = Matrix.Default.GetNode(matrixNodeX, matrixNodeY);
                
            if (node.Depth < currentDepth)
            {
                node.Depth = currentDepth;
            }
            
            if (node.Width < currentWidth)
            {
                node.Width = currentWidth;
            }

            return Invalidate();
        }

        public TestOutcome()
        {
            stateNumber = OutcomeTracker.Default.RequestStateId();
        }
    }


    public class Matrix
    {
        public static Matrix Default;
        
        private MatrixNode[,] nodes;

        public MatrixNode GetNode(int x, int y)
        {
            return nodes[x, y];
        }
    }
    
    
    public class MatrixNode
    {
        private Matrix     parent;
        private int        x;
        private int        y;
        private int        depth;
        private int        width;
        public  MatrixNode Up;
        public  MatrixNode Right;
        public  MatrixNode Down;

        public int Depth
        {
            get
            {
                if (Up == null)
                {
                    return depth;
                }
                else
                {
                    return Up.Depth + 1;
                }
            }
            set
            {
                if (value > depth)
                {
                    depth = value;
                }
            }
        }
        
        public int Width
        {
            get
            {
                if (Up == null)
                {
                    return width;
                }
                else
                {
                    return Right.width - 1;
                }
            }
            set
            {
                if (value > width)
                {
                    width = value;
                }
            }
        }
        


        public MatrixNode GetRight()
        {
            return parent.GetNode(x + 1, y);
        }
        
        public void ResetDepth()
        {
            Depth = 0;
        }
    }
    
}