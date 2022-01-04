using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Causality.States;
using Core;
using Core.Causality;
using Core.Factors;
using Core.States;
using Core.Threading;
using JetBrains.Annotations;
using static Core.Tools.Collections;

namespace Causality.States
{ 
    // public abstract class Result : CausalFactor, IResult, IInteraction
    // {
    //     #region Constants
    //     
    //     protected const int Initialized              = 0b0000_0010;
    //     protected const int ForwardExceptions        = 0b0000_0000;
    //     protected const int StoreExceptions          = 0b0000_0000;
    //     protected const int Unstable                 = 0b0000_0001;
    //     protected const int Updating                 = 0b0001_0000;
    //     protected const int Executing                = 0b0011_1000; //- If we are executing, we should also be updating.
    //     protected const int InValid                  = 0b0000_0010;
    //     protected const int Stabilizing              = 0b0000_0100;
    //     protected const int Destabilizing            = 0b0000_0100; 
    //     protected const int Necessary                = 0b0000_0100;
    //     protected const int RecalculateAutomatically = 0b0000_0101;
    //
    //     #endregion
    //     
    //     #region Instance Fields
    //     
    //     protected WeakReference<IInteraction> weakReferenceToSelf;
    //     protected IRiposte                    riposte;
    //     private   bool                        isReflexive;
    //
    //     #endregion
    //
    //     
    //     #region Instance Properties
    //
    //     public WeakReference<IInteraction> WeakReference => weakReferenceToSelf ?? 
    //                                                         (weakReferenceToSelf = new WeakReference<IInteraction>(this));
    //     public          bool IsUpdating         { get; protected set; }
    //     public          bool IsStable           { get; protected set; }
    //     public          bool IsValid            { get; protected set; }
    //     public          bool IsStablizing       { get; protected set; }
    //     public          bool IsBeingInfluenced  => riposte.IsBeingInfluenced;
    //     public          int  NumberOfInfluences => riposte.NumberOfInfluences;
    //     public          bool IsUnstable         => IsStable is false;
    //     public          bool IsInvalid          => IsValid  is false;
    //     public override int  Priority           => riposte.Priority;
    // //  public          bool AllowRecursion     { get; set; }
    //
    //     public bool IsReflexive
    //     {
    //         get => isReflexive;
    //         set
    //         {
    //             if (value is true)
    //             {
    //                 if (isReflexive is false)
    //                 {
    //                     isReflexive = true;
    //                     
    //                     if (IsNecessary is false) //- If it's true then we've already told our influences that they are Necessary.
    //                     {
    //                         riposte.NotifyInfluences_OutcomeNecessary();
    //                     }
    //                     
    //                     Stabilize();
    //                 }
    //             }
    //             else if (isReflexive is true)
    //             {
    //                 isReflexive = false;
    //
    //                 if (IsNecessary is false)  //<- If it's true then we still need our influences to be Necessary.
    //                 {
    //                     riposte.NotifyInfluences_OutcomeNotNecessary();
    //                 }
    //             }
    //         }
    //     }
    //     
    //     //- What happens if someone sets RecalculatesAutomatically to true inside an update method, on an Outcome that 
    //     //  has been invalidated during that same update process?
    //
    //     //- bool AllowDependencies :  This may be good for Action based Outcomes that want to track their dependencies,
    //     //                            but aren't intended to have their own dependents, like a Reaction that just prints
    //     //                            a value to the Console.  Alternatively, add a mechanic that allows something to
    //     //                            subscribe to a Reactive it accesses, even if the Reactive is hidden below the surface
    //     //                            i.e. being able to subscribe to a property that only exposes the value of the Reactive. 
    //     //
    //
    //     #endregion
    //
    //
    //     #region Instance Methods
    //
    //     //- We may have to keep track of how many Necessary children an Outcome has, so that if one notifies us
    //     // it's no longer Necessary we know if we are still Necessary or not.  We could iterate through our
    //     // dependents, but that seems cumbersome.
    //     public override void NotifyNecessary()
    //     {
    //         bool wasAlreadyNecessary = IsNecessary;
    //
    //         base.NotifyNecessary();
    //
    //         if (wasAlreadyNecessary  ||  IsReflexive  ||  (IsValid && IsStable))  
    //         {
    //             return;
    //             //- We don't propagate the Necessary status if we don't need to update, to avoid walking the tree.
    //         }
    //         else //- We know it's invalid or unstable
    //         {
    //             riposte.NotifyInfluences_OutcomeNecessary(); //- Let our influences stabilize before we do.
    //             Stabilize();
    //         }
    //     }
    //     
    //     public override void NotifyNotNecessary()
    //     {
    //         base.NotifyNotNecessary();
    //
    //         if (numberOfNecessaryDependents < 1 && 
    //             IsReflexive is false)
    //         {
    //             riposte.NotifyInfluences_OutcomeNotNecessary();
    //         }
    //     }
    //
    //     public virtual bool Invalidate() => Invalidate(null);
    //
    //     public bool Invalidate(IInfluence changedParentState)
    //     {
    //         if (IsUpdating)
    //         {
    //             throw new InvalidOperationException(
    //                 "An Outcome was invalidated while it was updating, meaning either the Outcome's update process " +
    //                 "caused it to invalidated itself creating an update loop, " +
    //                 "or the Outcome was accessed by two different threads at the same time. \n  " +
    //                $"The invalidated outcome was '{this}' and it was invalidated by '{changedParentState}'. ");
    //             //- If this Outcome is in the update list we should know it's a loop, if it's not then it should be
    //             //  another thread accessing it.  Well actually, the parent won't add us to the list until this returns...
    //         }
    //
    //         return InvalidateOutcome(changedParentState);
    //     }
    //     
    //     protected bool InvalidateOutcome(IInfluence changedParentState)
    //     {
    //         if (IsValid)
    //         {
    //             IsValid = false;
    //             riposte.RemoveInfluences(changedParentState);
    //
    //             if (IsNecessary || IsReflexive)
    //             {
    //                 UpdateList.Update(this);
    //             }
    //             else if (DestabilizeDependents())
    //             {
    //                 numberOfNecessaryDependents++;
    //                 UpdateList.Update(this);
    //             }
    //
    //             return true;
    //         }
    //
    //         return false;
    //     }
    //
    //     public override bool Stabilize()
    //     {
    //         if (IsInvalid)  //- Invalid states no longer have Influences.
    //         {
    //             return React() is false;
    //         }
    //         else if (IsUnstable)
    //         {
    //             return StabilizeOutcome();
    //         }
    //         else return true;
    //     }
    //     
    //     protected bool StabilizeOutcome()
    //     {
    //         IsStablizing = true;
    //             
    //         if (riposte.TryStabilizeInfluences())
    //         {
    //             IsStablizing = false;
    //             IsStable     = true;
    //             Debug.Assert(IsValid);
    //             
    //             return true;
    //         }
    //         else
    //         {
    //             return React() is false;
    //         }
    //     }
    //
    //     //- Does not imply the caller will queue this Outcome to be updated.
    //     //  Only that the caller should be notified if this Outcome is Necessary
    //     //  and if not that it should mark itself and its dependents as Unstable
    //     public bool Destabilize()
    //     {
    //         if (IsStablizing)
    //         {
    //             return false;
    //         }
    //         else if (IsNecessary || IsReflexive)
    //         {
    //             return true;
    //             
    //             //- We shouldn't need to mark ourselves as Unstable. If we're Necessary then either
    //             //  we're going to be  invalidated when our parent recalculates, or the parent won't
    //             //  change, in which case we aren't Unstable.
    //         }
    //         else if (IsInvalid)
    //         {
    //             return false;
    //         }
    //         else if (IsStable)  //- Can we be Stable and Invalid?
    //         {
    //             bool hasNecessaryDependents = DestabilizeDependents();
    //             
    //             if (hasNecessaryDependents)
    //             {
    //                 numberOfNecessaryDependents++;
    //                 return true;
    //             }
    //             else
    //             {
    //                 IsStable = false;
    //             }
    //         }
    //
    //         return false;
    //     }
    //
    //     protected bool DestabilizeDependents()
    //     {
    //         var formerDependents = affectedResults;
    //
    //         if (formerDependents.Count > 0) 
    //         {
    //             foreach (var dependentReference in formerDependents)
    //             {
    //                 if (dependentReference.TryGetTarget(out var dependent))
    //                 {
    //                     if (dependent.Destabilize())
    //                     {
    //                         return true;
    //                     }
    //                 }
    //             }
    //         }
    //
    //         return false;
    //     }
    //     
    //     public bool React()
    //     {
    //         bool outcomeChanged;
    //
    //         if (IsUpdating) { Debug.Fail($"Update loop in {nameof(Response)} => {this}."); }
    //
    //         IsUpdating   = true;
    //         IsStable     = true;
    //         IsValid      = true;
    //         IsStablizing = false;
    //
    //         try
    //         {
    //             outcomeChanged = ExecuteProcess();
    //         }
    //         catch (Exception e)
    //         {
    //             //InvalidateOutcome(null);
    //             
    //             //- TODO : Consider having Outcomes store exceptions as an accessible field,
    //             //         similar to some of the reactives available in other libraries.
    //             throw;
    //         }
    //         finally
    //         {
    //             IsUpdating = false;
    //         }
    //         
    //         if (outcomeChanged)
    //         {
    //             InvalidateDependents();
    //
    //             return true;
    //         }
    //         else
    //         {
    //             return false;
    //         }
    //     }
    //
    //     protected abstract bool ExecuteProcess();
    //
    //     public override bool AddDependent(IInteraction dependentToAdd)
    //     {
    //         if(dependentToAdd == null) { throw new ArgumentNullException(nameof(dependentToAdd)); }
    //
    //         if (IsUpdating)
    //         {
    //             throw new InvalidOperationException(""); //- Recursive dependency
    //         }
    //         
    //         return base.AddDependent(dependentToAdd);
    //     }
    //
    //     protected void OnInvolved()
    //     {
    //         Stabilize();
    //         NotifyInvolved();
    //         
    //         //- If this Outcome is updating, then either it's accessing itself in its update method, or
    //         //  something it affected during this update is.  We could ask the Observer what Outcome is
    //         //  actively updating, and if it's not us then it has to be one we accessed during this update,
    //         //  which means it's one we depend on, which means something we depend on depends on us, which
    //         //  means there's a loop.
    //     }
    //     
    //     #endregion
    //     
    //
    //     #region Constructors
    //
    //     protected Result(object ownerToReference) : base(ownerToReference)
    //     {
    //         IsValid = false;
    //     }
    //
    //     #endregion
    //     
    //
    //     #region Explicit Implementations
    //
    //     bool IUpdateable.Update() => Stabilize();
    //
    //     #endregion
    // }
    //
    //
    
    
    
    
    // public abstract class Result : CausalFactor, IResult, IDependency, IInfluenceable, IUpdateable
    // {
    //     #region Constants
    //     
    //     protected const int Initialized              = 0b0000_0010;
    //     protected const int ForwardExceptions        = 0b0000_0000;
    //     protected const int StoreExceptions          = 0b0000_0000;
    //     protected const int Unstable                 = 0b0000_0001;
    //     protected const int Updating                 = 0b0001_0000;
    //     protected const int Executing                = 0b0011_1000; //- If we are executing, we should also be updating.
    //     protected const int InValid                  = 0b0000_0010;
    //     protected const int Stabilizing              = 0b0000_0100;
    //     protected const int Destabilizing            = 0b0000_0100; 
    //     protected const int Necessary                = 0b0000_0100;
    //     protected const int RecalculateAutomatically = 0b0000_0101;
    //
    //     #endregion
    //     
    //     #region Static Fields
    //
    //     protected static readonly IInfluence[] defaultInfluences = new IInfluence[0];
    //
    //     #endregion
    //
    //
    //     #region Instance Fields
    //
    //     [NotNull] 
    //     protected IInfluence[]               influences = defaultInfluences;
    //     protected WeakReference<IDependency> weakReferenceToSelf;
    //     private   int                        nextOpenInfluenceIndex;
    //     protected int                        priority;
    //     private   bool                       isReflexive;
    //
    //     #endregion
    //
    //     
    //     #region Instance Properties
    //
    //     public WeakReference<IDependency> WeakReference => weakReferenceToSelf ?? 
    //                                                        (weakReferenceToSelf = new WeakReference<IDependency>(this));
    //     public          bool IsUpdating         { get; protected set; }
    //     public          bool IsStable           { get; protected set; }
    //     public          bool IsValid            { get; protected set; }
    //     public          bool IsStablizing       { get; protected set; }
    //     public          bool IsBeingInfluenced  => influences.Length > 0;
    //     public          int  NumberOfInfluences => nextOpenInfluenceIndex;
    //     public          bool IsUnstable         => IsStable is false;
    //     public          bool IsInvalid          => IsValid  is false;
    //     public override int  Priority           => priority;
    // //  public          bool AllowRecursion     { get; set; }
    //
    //     public bool IsReflexive
    //     {
    //         get => isReflexive;
    //         set
    //         {
    //             if (value is true)
    //             {
    //                 if (isReflexive is false)
    //                 {
    //                     isReflexive = true;
    //                     
    //                     if (IsNecessary is false) //- If it's true then we've already told our influences that they are Necessary.
    //                     {
    //                         NotifyInfluences_OutcomeNecessary();
    //                     }
    //                     
    //                     Reconcile();
    //                 }
    //             }
    //             else if (isReflexive is true)
    //             {
    //                 isReflexive = false;
    //
    //                 if (IsNecessary is false)  //<- If it's true then we still need our influences to be Necessary.
    //                 {
    //                     NotifyInfluences_OutcomeNotNecessary();
    //                 }
    //             }
    //         }
    //     }
    //     
    //     //- What happens if someone sets RecalculatesAutomatically to true inside an update method, on an Outcome that 
    //     //  has been invalidated during that same update process?
    //
    //     //- bool AllowDependencies :  This may be good for Action based Outcomes that want to track their dependencies,
    //     //                            but aren't intended to have their own dependents, like a Reaction that just prints
    //     //                            a value to the Console.  Alternatively, add a mechanic that allows something to
    //     //                            subscribe to a Reactive it accesses, even if the Reactive is hidden below the surface
    //     //                            i.e. being able to subscribe to a property that only exposes the value of the Reactive. 
    //     //
    //
    //     #endregion
    //
    //
    //     #region Instance Methods
    //
    //     //- We may have to keep track of how many Necessary children an Outcome has, so that if one notifies us
    //     // it's no longer Necessary we know if we are still Necessary or not.  We could iterate through our
    //     // dependents, but that seems cumbersome.
    //     public override void NotifyNecessary()
    //     {
    //         bool wasAlreadyNecessary = IsNecessary;
    //
    //         base.NotifyNecessary();
    //
    //         if (wasAlreadyNecessary  ||  IsReflexive  ||  (IsValid && IsStable))  
    //         {
    //             return;
    //             //- We don't propagate the Necessary status if we don't need to update, to avoid walking the tree.
    //         }
    //         else //- We know it's invalid or unstable
    //         {
    //             NotifyInfluences_OutcomeNecessary(); //- Let our influences stabilize before we do.
    //             Reconcile();
    //         }
    //     }
    //
    //     protected void NotifyInfluences_OutcomeNecessary()
    //     {
    //         #if DEBUG
    //             Debug.Assert(IsNecessary || IsReflexive);
    //         #endif
    //
    //         var currentInfluences = influences;
    //         
    //         for (int i = 0; i < currentInfluences.Length; i++)
    //         {
    //             currentInfluences[i].NotifyNecessary();
    //         }
    //     }
    //
    //     public override void NotifyNotNecessary()
    //     {
    //         base.NotifyNotNecessary();
    //
    //         if (numberOfNecessaryDependents < 1 && 
    //             IsReflexive is false)
    //         {
    //             NotifyInfluences_OutcomeNotNecessary();
    //         }
    //     }
    //     
    //     protected void NotifyInfluences_OutcomeNotNecessary()
    //     {
    //         #if DEBUG
    //             Debug.Assert(IsNecessary is false  &&  IsReflexive is false);
    //         #endif
    //         
    //         var currentInfluences = influences;
    //         
    //         for (int i = 0; i < currentInfluences.Length; i++)
    //         {
    //             currentInfluences[i].NotifyNotNecessary();
    //         }
    //     }
    //     
    //     public virtual bool Invalidate() => Invalidate(null);
    //     
    //
    //     public bool Invalidate(IInfluence changedParentState)
    //     {
    //         if (IsUpdating)
    //         {
    //             throw new InvalidOperationException(
    //                 "An Outcome was invalidated while it was updating, meaning either the Outcome's update process " +
    //                 "caused it to invalidated itself creating an update loop, " +
    //                 "or the Outcome was accessed by two different threads at the same time. \n  " +
    //                $"The invalidated outcome was '{this}' and it was invalidated by '{changedParentState}'. ");
    //             //- If this Outcome is in the update list we should know it's a loop, if it's not then it should be
    //             //  another thread accessing it.  Well actually, the parent won't add us to the list until this returns...
    //         }
    //
    //         return InvalidateOutcome(changedParentState);
    //     }
    //     
    //     protected bool InvalidateOutcome(IInfluence changedParentState)
    //     {
    //         if (IsValid)
    //         {
    //             IsValid = false;
    //             RemoveInfluences(changedParentState);
    //
    //             if (IsNecessary || IsReflexive)
    //             {
    //                 UpdateList.Update(this);
    //             }
    //             else if (DestabilizeDependents())
    //             {
    //                 numberOfNecessaryDependents++;
    //                 UpdateList.Update(this);
    //             }
    //
    //             return true;
    //         }
    //
    //         return false;
    //     }
    //
    //     //- Note: Non-Reactive Factors always return true when Stabilize() is called,
    //     //        so any reactor that relies on one or more of them will never be able to stabilize, even if
    //     //        the parent that destabilized them was a reactor that could have been stabilized.
    //     public override bool Reconcile() => Stabilize();
    //     
    //     protected bool StabilizeOutcome()
    //     {
    //         IsStablizing = true;
    //             
    //         if (TryStabilizeInfluences())
    //         {
    //             IsStablizing = false;
    //             IsStable     = true;
    //             Debug.Assert(IsValid);
    //             
    //             return true;
    //         }
    //         else
    //         {
    //             return React() is false;
    //         }
    //     }
    //
    //     private bool TryStabilizeInfluences()
    //     {
    //         var formerInfluences = influences; 
    //         //^ Stabilizing any of these could result in it invalidating this Outcome,
    //         //  and we drop our influences when we're invalidated.
    //             
    //         for (int i = 0; i < formerInfluences.Length; i++)
    //         {
    //             var currentInfluence = formerInfluences[i];
    //             
    //             if (currentInfluence.Reconcile() is false)
    //             {
    //                 return false;
    //                 //- No point in stabilizing the rest.  We try to stabilize to avoid recalculating, and if one
    //                 //  of our influences fails to stabilize then we have no choice but to recalculate.  In addition
    //                 //  we don't even know if we'll use the same influences when we recalculate. If we do end
    //                 //  up accessing those same influences, they'll try to stabilize themselves when we access them anyways.
    //             }
    //         }
    //         
    //         return true;
    //     }
    //     
    //     //- Does not imply the caller will queue this Outcome to be updated.
    //     //  Only that the caller should be notified if this Outcome is Necessary
    //     //  and if not that it should mark itself and its dependents as Unstable
    //     public bool Destabilize()
    //     {
    //         if (IsStablizing)
    //         {
    //             return false;
    //         }
    //         else if (IsNecessary || IsReflexive)
    //         {
    //             return true;
    //             
    //             //- We shouldn't need to mark ourselves as Unstable. If we're Necessary then either
    //             //  we're going to be  invalidated when our parent recalculates, or the parent won't
    //             //  change, in which case we aren't Unstable.
    //         }
    //         else if (IsInvalid)
    //         {
    //             return false;
    //         }
    //         else if (IsStable)  //- Can we be Stable and Invalid?
    //         {
    //             bool hasNecessaryDependents = DestabilizeDependents();
    //             
    //             if (hasNecessaryDependents)
    //             {
    //                 numberOfNecessaryDependents++;
    //                 return true;
    //             }
    //             else
    //             {
    //                 IsStable = false;
    //             }
    //         }
    //
    //         return false;
    //     }
    //
    //     protected bool DestabilizeDependents()
    //     {
    //         var formerDependents = affectedResults;
    //
    //         if (formerDependents.Count > 0) 
    //         {
    //             foreach (var dependentReference in formerDependents)
    //             {
    //                 if (dependentReference.TryGetTarget(out var dependent))
    //                 {
    //                     if (dependent.Destabilize())
    //                     {
    //                         return true;
    //                     }
    //                 }
    //             }
    //         }
    //
    //         return false;
    //     }
    //
    //
    //     public bool Stabilize()
    //     {
    //         if (IsInvalid) //- Invalid states no longer have Influences.
    //         {
    //             return React() is false;
    //         }
    //         else if (IsUnstable)
    //         {
    //             return StabilizeOutcome();
    //         }
    //         else return true;
    //     }
    //     
    //     protected bool React()
    //     {
    //         bool outcomeChanged;
    //
    //         if (IsUpdating) { Debug.Fail($"Update loop in {nameof(Response)} => {this}."); }
    //
    //         IsUpdating   = true;
    //         IsStable     = true;
    //         IsValid      = true;
    //         IsStablizing = false;
    //
    //         try
    //         {
    //             outcomeChanged = ExecuteProcess();
    //         }
    //         catch (Exception e)
    //         {
    //             //InvalidateOutcome(null);
    //             
    //             //- TODO : Consider having Outcomes store exceptions as an accessible field,
    //             //         similar to some of the reactives available in other libraries.
    //             throw;
    //         }
    //         finally
    //         {
    //             IsUpdating = false;
    //         }
    //         
    //         if (outcomeChanged)
    //         {
    //             InvalidateDependents();
    //
    //             return true;
    //         }
    //         else
    //         {
    //             return false;
    //         }
    //     }
    //
    //     protected abstract bool ExecuteProcess();
    //
    //     public bool ForceReaction() => React();
    //
    //     protected void RemoveInfluences(IInfluence stateToSkip)
    //     {
    //         var formerInfluences   = influences;
    //         var lastInfluenceIndex = nextOpenInfluenceIndex - 1;
    //         
    //         for (int i = 0; i <= lastInfluenceIndex; i++)
    //         {
    //             ref IInfluence currentInfluence = ref formerInfluences[i];
    //
    //             if (currentInfluence != stateToSkip)
    //             {
    //                 currentInfluence.ReleaseDependent(this);
    //             }
    //             
    //             currentInfluence = null;
    //         }
    //
    //         nextOpenInfluenceIndex = 0;
    //         //- Note : We could choose to keep the influences until we recalculate, and then compare them with 
    //         //         the influences that are added during the update process.  
    //     }
    //     
    //     public void Notify_InfluencedBy(IInfluence influence)
    //     {
    //         if (influence is null) { throw new ArgumentNullException(nameof(influence)); }
    //         
    //         if (IsUpdating)
    //         {
    //             if (IsValid) 
    //             {
    //                 if (influence.AddDependent(this))
    //                 {
    //                     //- We expect a State to add us as a dependent, only if they don't already have us as a dependent. 
    //                     Add(ref influences, influence, nextOpenInfluenceIndex);
    //                     nextOpenInfluenceIndex++;
    //
    //                     if (influence.Priority >= this.Priority)
    //                     {
    //                         this.priority = influence.Priority + 1;
    //                     }
    //                     
    //                     #if DEBUG
    //                     
    //                     if (influence is IDependency influentialOutcome)
    //                     {
    //                         if (affectedResults.Contains(influentialOutcome.WeakReference))
    //                         {
    //                             throw new InvalidOperationException(""); //- Recursive dependency
    //                         }
    //                     }
    //                     
    //                     #endif
    //                 }
    //             }
    //         }
    //         else
    //         {
    //             #if AllowAddingInfluencesOutsideOfUpdate == false
    //             
    //             throw new InvalidOperationException("Influences can only be added while an Outcome is updating. ");
    //
    //             #endif
    //         }
    //     }
    //     
    //     public override bool AddDependent(IDependency dependentToAdd)
    //     {
    //         if(dependentToAdd == null) { throw new ArgumentNullException(nameof(dependentToAdd)); }
    //
    //         if (IsUpdating)
    //         {
    //             throw new InvalidOperationException(""); //- Recursive dependency
    //         }
    //         
    //         return base.AddDependent(dependentToAdd);
    //     }
    //
    //     protected void OnInvolved()
    //     {
    //         Stabilizer();
    //         NotifyInvolved();
    //         
    //         //- If this Outcome is updating, then either it's accessing itself in its update method, or
    //         //  something it affected during this update is.  We could ask the Observer what Outcome is
    //         //  actively updating, and if it's not us then it has to be one we accessed during this update,
    //         //  which means it's one we depend on, which means something we depend on depends on us, which
    //         //  means there's a loop.
    //     }
    //     
    //     #endregion
    //     
    //
    //     #region Constructors
    //
    //     protected Result(object ownerToReference) : base(ownerToReference)
    //     {
    //         IsValid = false;
    //     }
    //
    //     #endregion
    //     
    //
    //     #region Explicit Implementations
    //
    //     bool IUpdateable.UpdateOutcome() => Reconcile();
    //
    //     #endregion
    // }
}
