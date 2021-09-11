using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Core;
using Core.Causality;
using JetBrains.Annotations;

namespace Causality.States
{
    public abstract class OutcomeBase : State, IOutcome
    {
        #region Constants

        protected const int Unstable  = 0b0000_0001;
        protected const int Necessary = 0b0000_0010;
        protected const int Reflexive = 0b0000_0100;
        protected const int Updating  = 0b0000_1000;
        
        public enum ChildState { NoChange, Clean, Destabilized, Necessary, AlreadyUpdating, }
        
        #endregion
        
        #region Static Fields

        protected static readonly WeakReference<INotifiable> invalidReference      = new WeakReference<INotifiable>(null);
        protected static readonly IState[]                   invalidatedInfluences = new IState[0];

        #endregion


        #region Instance Fields

        [NotNull] 
        protected StateVersion[]                   influences = Array.Empty<StateVersion>();
        protected WeakReference<INotifiable> callbackReference;
        

        #endregion


        #region Instance Properties

        protected virtual IProcess Process { get; }
        public            bool     HasCallback => callbackReference != null;
        public            int      Depth         { get; protected set; }
        public            long     LastUpdatedAt { get; set; }
        public            bool     IsUnstable    { get; set; }
        public            bool     IsDirty       { get; set; }
        public            bool     IsStable      { get; set; }
        public            bool     IsReflexive   { get; set; }
        public            bool     IsNecessary   { get; set; }

        public            bool     IsBeingAffected => influences.Length > 0;


        //- bool AllowDependencies :  This may be good for Action based Outcomes that want to track their dependencies,
        //                            but aren't intended to have their own dependents, like a Reaction that just prints
        //                            a value to the Console.

        #endregion


        #region Instance Methods

        public override bool Invalidate() => Invalidate(null);

        public bool Invalidate(IState invalidState)
        {
            if (base.Invalidate())
            {
                // RemoveInfluences(invalidState);

                var referenceToNotify = Interlocked.Exchange(ref callbackReference, invalidReference);

                if ((referenceToNotify != null) && referenceToNotify.TryGetTarget(out var objectToNotify))
                {
                    Debug.Assert(referenceToNotify != invalidReference, "");
                    UpdateHandler.RequestUpdate(objectToNotify.Notify);
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        
        public void NotifyParentChanged(IState changedState, long newVersionNumber, int currentDepth)
        {
            if (VersionIsNewer(changedState, newVersionNumber))
            {
                if (IsReflexive)
                {
                    bool valueChanged = ExecuteProcess(out newVersionNumber);

                    if (valueChanged)
                    {
                        foreach (var outcome in affectedOutcomes)
                        {
                            outcome.NotifyParentChanged(this, newVersionNumber, currentDepth + 1);
                        }
                    }

                }
                else if (IsStable)
                {
                    IsUnstable = true;

                    foreach (var outcome in affectedOutcomes)
                    {
                        NotifyParentUnstable(this, newVersionNumber);
                    }
                }
            }
        }
        
        protected bool VersionIsNewer(IState changedState, long newVersionNumber)
        {
            StateVersion[] previousVersions = influences;

            for (int i = 0; i < previousVersions.Length; i++)
            {
                var versionWeHave = previousVersions[i];
                
                if (versionWeHave.State == changedState)
                {
                    if (versionWeHave.VersionNumber < newVersionNumber)
                    {
                        influences = Array.Empty<StateVersion>();
                        
                        return true;
                    }
                    
                    break;
                }
            }

            return false;
        }

        public ChildState NotifyParentUnstable(IState changedState, long changedVersionNumber)
        {
            if (VersionIsNewer(changedState, changedVersionNumber))  
            {
                //- We should dump our influences, to indicate that we are queued for recalc
                //  We need a way to for a thread that stumbles on this object to access the 
                //  list of nodes to be recalc'd so it can help instead of just waiting.
                //  We should also add an option that will allow the thread to just take the
                //  old value instead of taking the time to help.


                if (IsNecessary || IsReflexive)
                {
                    return ChildState.Necessary;
                }
                else if (IsStable)
                {
                    var unstableVersionNumber = CurrentVersion;  //- Interlocked here?
                    
                    IsUnstable = true;

                    foreach (var outcome in affectedOutcomes)
                    {
                        bool dependentIsNecessary = outcome.NotifyParentUnstable(this, unstableVersionNumber);
                        
                        if (dependentIsNecessary)
                        {
                            return ChildState.Necessary;
                        }
                    }

                    return ChildState.Destabilized;
                }
            }
            else
            {
                return ChildState.NoChange;
            }

        }


        public bool NotifyParentUnstable(long changeNumber)
        {
            if (changeNumber > LastUpdatedAt)  
            {
                //- We should dump our influences, to indicate that we are queued for recalc
                //  We need a way to for a thread that stumbles on this object to access the 
                //  list of nodes to be recalc'd so it can help instead of just waiting.
                //  We should also add an option that tells the thread to just take the
                //  old value instead of taking the time to help

                LastUpdatedAt = changeNumber;

                if (IsReflexive)
                {
                    return true;
                }
                else if (IsStable)
                {
                    IsUnstable = true;

                    foreach (var outcome in affectedOutcomes)
                    {
                        bool dependentIsReflexive = outcome.NotifyParentUnstable(this, changeNumber);
                        
                        if (dependentIsReflexive)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        protected virtual bool ExecuteProcess(out long newVersionNumber)
        {
            throw new NotImplementedException();
        }

        public bool MarkUnstable(int currentDepth)
        {
            if ((status & Unstable)  ==  Unstable)
            {
                return false;
            }
            else if (status > Necessary)
            {
                return true;
            }
            else
            {
                bool isNecessary = false;
                    
                foreach (var outcome in affectedOutcomes)
                {
                    bool dependentIsNecessary = outcome.MarkUnstable(currentDepth + 1);

                    if (dependentIsNecessary)
                    {
                        isNecessary = true;
                    }
                }

                if (isNecessary)
                {
                    status |= Necessary;
                }

                return isNecessary;
            }
        }

        public bool Recalculate(int currentDepth)
        {
            if (currentDepth > Depth)
            {
                Depth = currentDepth;
            }
            
            if ((status & Unstable)  ==  Unstable)
            {
                bool valueChanged = ExecuteProcess();

                if (valueChanged)
                {
                    foreach (var outcome in affectedOutcomes)
                    {
                        if (outcome.IsNecessary)
                        {
                            UnstableList.Default.AddOutcome(outcome, outcome.Depth);
                        }
                    }
                }
            }
            
            
        }

        public bool ManuallyRecalculate()
        {
            if (IsDirty)
            {
                bool shouldRecalculate = false;
                
                for (int i = 0; i < influences.Length; i++)
                {
                    var stateVersion = influences[i];
                    var state        = stateVersion.State;

                    if (stateVersion.VersionNumber  !=  state.CurrentVersion)
                    {
                        var influenceChanged = state.ManuallyRecalculate();

                        if (influenceChanged)
                        {
                            shouldRecalculate = true;
                            break;
                        }
                    }
                }

                bool outcomeChanged = shouldRecalculate && ExecuteProcess();

                IsDirty = false;

                return outcomeChanged;
            }
            
            return false;
        }
        
        protected void Revalidate()
        {
            if (IsInvalid)
            {
                status &= ~Invalid;
                
                bool outcomeChanged = ExecuteProcess();

                if (outcomeChanged)
                {
                    InvalidateDependents();
                }

            }
            else if (IsPotentiallyInvalid)  // Unstable?
            {
                CheckValidityOfInfluences();
            }
        }

        protected abstract bool ExecuteProcess();

        private void CheckValidityOfInfluences()
        {
            
        }

        protected void RemoveInfluences(IState stateToSkip) => ReplaceInfluences(stateToSkip, invalidatedInfluences);


        protected void ReplaceInfluences(IState stateToSkip, IState[] newInfluences)
        {
            if (newInfluences == null)
            {
                throw new ArgumentNullException(nameof(newInfluences));
            }

            IState[] formerInfluences = influences;
            IState[] influencesToRemove = null;
            bool handled = false;

            while (handled == false)
            {
                if (formerInfluences != invalidatedInfluences)
                {
                    if (InterlockedUtils.TryCompareExchangeOrSet(ref influences, newInfluences, ref formerInfluences))
                    {
                        influencesToRemove = formerInfluences;
                        handled = true;
                    }
                }
                else
                {
                    influencesToRemove = newInfluences;
                    handled = true;
                }
            }

            if (influencesToRemove != null)
            {
                for (int i = 0; i < formerInfluences.Length; i++)
                {
                    IState currentFactor = formerInfluences[i];

                    if (currentFactor != stateToSkip)
                    {
                        formerInfluences[i]?.ReleaseDependent(this);
                    }
                }
            }
        }

        public void SetInfluences(IState[] newInfluences)
        {
            if (newInfluences == null)
            {
                throw new ArgumentNullException(nameof(newInfluences));
            }

            ReplaceInfluences(null, newInfluences);
        }

        public void SetCallback(INotifiable objectToNotify)
        {
            if (objectToNotify == null)
            {
                throw new ArgumentNullException(nameof(objectToNotify));
            }

            var formerReference = callbackReference;
            var newReference = objectToNotify != null ? new WeakReference<INotifiable>(objectToNotify) : null;
            while (true)
            {
                if (formerReference != invalidReference)
                {
                    if (InterlockedUtils.TryCompareExchangeOrSet(ref callbackReference, newReference, ref formerReference))
                    {
                        return;
                    }
                }
                else
                {
                    UpdateHandler.RequestUpdate(objectToNotify.Notify);
                    return;
                }
            }
        }

        public void DisableCallback()
        {
            var formerReference = callbackReference;

            while (IsValid &&
                   formerReference != null &&
                   formerReference != invalidReference)
            {
                if (InterlockedUtils.TryCompareExchangeOrSet(ref callbackReference, null, ref formerReference))
                {
                    return;
                }
            }
        }

        //public bool CheckIfInvalid()
        //{
        //    for (int i = 0; i < influences.Length; i++)
        //    {
        //        var currentInfluence = influences[i];
        //
        //        if (currentInfluence.IsInvalid)
        //        {
        //            Invalidate(currentInfluence);
        //        }
        //    }
        //}

        public bool CheckIfUnstable()
        {
            
            
        }
        
        public bool MarkDirty(int currentDepth)
        {
            if (IsDirty)
            {
                if (Depth < currentDepth)
                {
                    Depth = currentDepth;
                }

                return IsNecessary;
            }
            else
            {
                bool shouldRecalculate = IsNecessary;
                
                if (affectedOutcomes.Count > 0)
                {
                    if (shouldRecalculate)
                    {
                        foreach (var outcome in affectedOutcomes)
                        {
                            outcome.MarkDirty(currentDepth + 1);
                        }
                    }
                    else
                    {
                        foreach (var outcome in affectedOutcomes)
                        {
                            if (outcome.MarkDirty(currentDepth + 1))
                            {
                                shouldRecalculate = true;
                            }
                        }
                    }
                }
                else
                {
                    shouldRecalculate = HasCallback;
                }

                if (shouldRecalculate == false)
                {
                    this.Invalidate();
                }

                return shouldRecalculate;
            }
        }

        private void RecalculateDependents()
        {
            foreach (var outcome in affectedOutcomes)
            {
                if (outcome.Depth == (this.Depth + 1) &&
                    outcome.IsNecessary)
                {
                    outcome.Recalculate();
                }
            }
        }
        


        
        #endregion


        public bool Recalculate1()
        {
            if (IsInvalid)
            {
                bool valueChanged = ExecuteProcess();

                if (valueChanged)
                {
                    foreach (var outcome in affectedOutcomes)
                    {
                        if (outcome.IsPendingUpdate is false && 
                            outcome.IsNecessary)
                        {
                            UnstableList.Default.AddOutcome(outcome, outcome.Depth);
                        }
                        else if(outcome.IsReflexive)
                        {
                            
                        }
                    }
                    
                    affectedOutcomes.Clear();
                }
            }
            
            
        }

        
        public void MarkReflexive()
        {
            if (IsReflexive is false)
            {
                if (IsStable is false)
                {
                    RecalculateUp();
                }

                IsReflexive = true;
            }
        }

        public bool RecalculateUp()
        {
            bool shouldRecalculate = false;
            
            UnstableList.Default.AddOutcome(this, Depth);

            for (int i = 0; i < influences.Length; i++)
            {
                if (influences[i].IsClean is false)
                {
                    if (RecalculateUp())
                    {
                        shouldRecalculate = true;
                    }
                }
            }

            if (shouldRecalculate)
            {
                Array.Clear(influences, 0, influences.Length);

                var outcomeChanged = ExecuteProcess();
                
                if (outcomeChanged)
                {
                    foreach (var outcome in affectedOutcomes)
                    {
                        if (outcome.IsStable)
                        {
                            continue;
                        }
                        else if (outcome.IsReflexive && 
                                 outcome.IsDirty)
                        {
                            outcome.NotifyParentChanged(this);
                        }
                    }
                }
            }

        }

        public void NotifyParentChanged(IOutcome outcomeBase)
        {
            
        }
    }
    
    
    
    
    
    
    

    //- Record the access order of all of the Responsive elements?
    //  Then iterate back down the tree using the recorded numbers to avoid actually searching for what nodes to recalculate.
    //  A node could change its access order by simply removing the old value and adding the new value to the end.
}



public class Continuum
{
    private IOutcome[] highestInvalidOutcome;
    private int        numberOfOutcomes;
    private int        highestValidOutcome;
    private int[]      potentiallyInvalidOutcomes;  //- TODO : Come back to this.  Maybe we can have outcomes reference each other by index, instead of by reference.

    public bool CheckIfValid(int outcomeHeight) => outcomeHeight >= highestValidOutcome;

    public void InformInvalid(int outcomeHeight)
    {
        if (outcomeHeight <= highestValidOutcome)
        {
            highestValidOutcome = outcomeHeight - 1;
        }
    }
    
}



public class MultiContinuum : Continuum
{
    private Continuum parent1;
    private Continuum parent2;
    private int       initialHeight;
    
    
}


public class RecalcNode
{
    private RecalcNode previous;
    private RecalcNode next;
    private IOutcome   outcome;

    
}

public class ResponsivenessManager
{
    private ResponsivenessTracker[] trackers;
    
    
}

public class ResponsivenessTracker
{
    private ResponsivenessManager parent;
    private long bits;

    public void Invalidate()
    {
        
    }
}





//- intended to be a recursive set of sorts.  The Hashsets contain the index in *states for each dependent state of
//  whoever has that Id index.  Then you could use the indexes for those dependents to get their dependent ad infinitum. 
public class TreeSet
{
    public WeakReference<IState>[] states;
    public HashSet<int>[] dependencies;

    public void GetAllDependents(int id)
    {
        var allDependents = new HashSet<int>();
        bool finished = false;

        while (finished is false)
        {
            HashSet<int> dependents = dependencies[id];
            
            allDependents.UnionWith(dependents);

            foreach (var dependentIndex in dependents)
            {
                
            }
        }
    }

    public void GetAllDependents(int id)
    {
        
    }
}


















