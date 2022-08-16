using System.Diagnostics;
using Core.Factors;
using Core.States;
using static Core.Tools.Types;

namespace DirectFactors
{
    public abstract class ReactiveCore<T> : DirectFactorCore, IReactiveCore<T>
    {
        #region Instance Fields

        protected readonly IEqualityComparer<T> valueComparer;
        protected readonly WeakSubscriber<T>    weakSubscriber;
        protected          T                    currentValue;
        protected          bool                 unsubscribeWhenTriggered = false;

        #endregion
        
        
        #region Instance Properties

        protected          IReactiveCore<T>           Owner            { get; set; }
        public             bool                       IsReacting       { get; protected set; }
        public             bool                       IsStabilizing    { get; protected set; }
        public             bool                       HasReacted       { get; protected set; }
        protected abstract IEnumerable<IDirectFactor> Triggers         { get; }
        public    abstract bool                       HasTriggers      { get; }
        public    abstract int                        NumberOfTriggers { get; }

        public bool IsUnstable {            get => weakSubscriber.IsUnstable;
                                  protected set => weakSubscriber.IsUnstable = value; }
        public bool IsNecessary {           get => weakSubscriber.IsNecessary;
                                  protected set => weakSubscriber.IsNecessary = value; }
        public bool HasBeenTriggered {      get => weakSubscriber.HasBeenTriggered;
                                  protected set => weakSubscriber.HasBeenTriggered = value; }

        #endregion


        #region Instance Methods

        public bool GenerateOutcome(out T oldValue, out T newValue)
        {
            if (IsReacting)                { Debug.Fail($"Update loop in {NameOf<ReactiveCore<T>>()} => {this}."); }
            if (HasBeenTriggered is false) { InvalidateOutcome(null); }
            
            bool outcomeChanged;

            IsReacting       = true;
            IsUnstable       = false;
            HasBeenTriggered = false;
            
            try
            {
                outcomeChanged = CreateOutcome(out oldValue, out newValue);
            }
            catch (Exception e)
            {
                //- TODO : Consider storing exceptions as an accessible field,
                //         similar to some of the reactives available in other libraries.
                
                // InvalidateOutcome(null);
                throw;
            }
            finally
            {
                IsReacting = false;
            }
            
            if (HasReacted is false)
            {
                HasReacted = true;
                foreach (var trigger in Triggers)
                {
                    AddTrigger(trigger, IsNecessary);
                }
            }
            
            if (outcomeChanged)
            {
                VersionNumber++;
                return true;
            }
            else return false;
        }
        
        protected abstract bool CreateOutcome(out T oldValue, out T newValue);
        protected abstract void InvalidateOutcome(IDirectFactor<T> changedFactor);

        public bool TryStabilizeOutcome()
        {
            bool successfullyReconciled = true;
            
            IsStabilizing = true;

            foreach (var determinant in Triggers)
            {
                if (determinant.Reconcile() is false)
                {
                    successfullyReconciled = false;
                    Debug.Assert(HasBeenTriggered);
                    //^ If the reconciliation failed then the trigger should have reacted and then
                    //  triggered us since we're one of its subscribers.

                    break;
                    //- No point in stabilizing the rest.  We try to stabilize to avoid updating, and if one
                    //  of our triggers fails to stabilize then we have no choice but to update.  In addition
                    //  we don't even know if we'll use the same triggers when we update. If we do end
                    //  up accessing those same triggers, they'll try to stabilize themselves when we access
                    //  them anyways.
                }
            }
            
            IsUnstable    = false;
            //- Should we be changing this?  Should it be set by GenerateOutcome?
            //  Should it only happen if we successfully reconcile?
            //  What if we're destabilized while generating our outcome?  This may mark us as stable even if we aren't
            IsStabilizing = false;

            return successfullyReconciled;
        }
        
        public bool Trigger() => Trigger(null, out _);

        public bool Trigger(IFactor triggeringFactor, out bool removeSubscription)
        {
            removeSubscription = unsubscribeWhenTriggered;
            
            if (IsReacting)
            {
                Logging.Notify_ReactorTriggeredWhileUpdating(this, triggeringFactor);

                //- If this Outcome is in the update list we should know it's a loop, if it's not then it should be
                //  another thread accessing it.
                //  Well actually, the parent won't add us to the list until this returns...
                //  Don't we add ourselves now?
            }
            
            if (HasBeenTriggered is false)
            {
                HasBeenTriggered = true;
                InvalidateOutcome(triggeringFactor);
                Owner?.CoreTriggered(this);
                
                return true;
            }
    
            return false;
        }

        //- Does not imply the caller will queue this subscriber to be updated.  Only that the this subscriber
        //  should mark itself and its dependents as Unstable, return whether it is Necessary or not.
        public bool Destabilize(IFactor factor)
        {
            if (IsNecessary)
            {
                return true;
                
                //- We shouldn't need to mark ourselves as Unstable. If we're Necessary then either
                //  we're going to be triggered when our parent updates, or the parent won't
                //  change, in which case we aren't Unstable.
            }
            else if (IsUnstable is false)
            {
                if (NotifyOwner_CoreDestabilized())
                {
                    return true;
                }
                else
                {
                    IsUnstable = true;
                }
            }
    
            return false;
            
            //- Note : This method used to check IsStabilizing and HasBeenTriggered, but I removed that because it
            //         it no longer seemed relevant.  When we have time, go through all the possible ways this might
            //         called and make sure I'm not missing something.
        }
        
        protected bool NotifyOwner_CoreDestabilized() => Owner?.CoreDestabilized(this) ?? false;
        
        public virtual void OnNecessary()
        {
            IsNecessary = true;

            //- We don't propagate the Necessary status to our triggers, to avoid walking up the tree.
            //  All Reactors walk down the tree to destabilize their dependents, so we can let them know then
            //  if need be.
        }

        public virtual void OnNotNecessary()
        {
            IsNecessary = false;

            if (HasReacted)
            {
                foreach (var trigger in Triggers)
                {
                    trigger.NotifyNotNecessary(this);
                }
            }
            
            //- Note : This may not work as intended if Observed cores continue to pass themselves as triggers,
            //         because the core won't be able to tell its owner when its dependents notify it that it's
            //         not necessary.
        }
        
        protected bool    AddTrigger(IDirectFactor<T> trigger, bool necessary) => trigger.Subscribe(weakSubscriber, necessary);
        protected void RemoveTrigger(IDirectFactor<T> trigger)                 => trigger.Unsubscribe(weakSubscriber);
        
        public void SetOwner(IReactorCoreOwner reactor)
        {
            if (Owner != null)
            {
                throw new InvalidOperationException(
                    $"A process attempted to set the Owner for a ReactorCore to {reactor}, " +
                    $"but its Owner property is already assigned to {Owner}. ");
            }
            
            Owner = reactor ?? throw new ArgumentNullException(nameof(reactor));
        }

        public override void Dispose()
        {
            Owner = null;

            foreach (var trigger in Triggers)
            {
                RemoveTrigger(trigger);
            }
        }
        #endregion
    }
    
    
    public class DirectFactorCore : IDirectFactorCore
    {
        #region Instance Properties

        public virtual int  UpdatePriority => 0;
        public         uint VersionNumber  { get; protected set; }

        #endregion

        
        #region Instance Methods
        
        public virtual void Dispose() { }

        #endregion
    }
    

    public interface IDirectFactorCore : IDisposable, IVersioned, IPrioritizedUpdate
    {
        
    }
}