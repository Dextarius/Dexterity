using System.Diagnostics;
using Core.States;

namespace DirectFactors
{
    public abstract class CoreBase<T> : IFactorSubscriber<T>, IUpdateable
    {
        #region Instance Fields

        private IDirectFactor<T> subscribedFactor;
        private bool             isReflexive;

        #endregion
        

        #region Properties

        public    bool IsReacting       { get; protected set; }
        public    bool IsStabilizing    { get; protected set; }
        public    bool HasReacted       { get; protected set; }
        public    bool IsUnstable       { get; protected set; }
        public    bool HasBeenTriggered { get; protected set; }
        protected bool IsQueued         { get; set; }

        protected abstract IEnumerable<IDirectFactor> Triggers         { get; }
        public    abstract bool                       HasTriggers      { get; }
        public    abstract int                        NumberOfTriggers { get; }
        
        public bool IsNecessary => IsReflexive;

        public bool IsReflexive
        {
            get => isReflexive;
            set
            {
                if (value is true)
                {
                    if (isReflexive is false)
                    {
                        isReflexive = true;
                        OnNecessary();
                    }
                }
                else if (isReflexive is true)
                {
                    isReflexive = false;
                    OnNotNecessary();
                }
            }
        }

        #endregion

        #region Instance Methods

        protected void CreateOutcome()
        {
            if (HasReacted is false)
            {
                HasReacted = true;
                foreach (var trigger in Triggers)
                {
                    AddTrigger(trigger, IsNecessary);
                }
            }
        }

        protected bool React()
        {
            
        }

        public void AttemptReaction()
        {
            if (HasBeenTriggered)
            {
                CreateOutcome();
            }
            else if (IsUnstable)
            {
                if (TryStabilizeOutcome())
                {
                    Debug.Assert(HasBeenTriggered is false);

                   // return false;
                }
                // else
                // {
                //     CreateOutcome();
                // }
            }
         // else return false;
        }
        
        public bool ForceReaction() => React();

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

            IsUnstable = false;
            //- Should we be changing this?  Should it be set by GenerateOutcome?
            //  Should it only happen if we successfully reconcile?
            //  What if we're destabilized while generating our outcome?  This may mark us as stable even if we aren't
            IsStabilizing = false;

            return successfullyReconciled;
        }
        
        public bool Subscribe()          => subscribedFactor.Subscribe(this, IsNecessary);
        public void Unsubscribe()        => subscribedFactor.Unsubscribe(this);
        public void NotifyNecessary()    => subscribedFactor.NotifyNecessary(this);
        public void NotifyNotNecessary() => subscribedFactor.NotifyNotNecessary(this);

        public bool Destabilize(IDirectFactor factor)
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
                bool hasNecessaryDependents = DestabilizeSubscribers();
                
                if (hasNecessaryDependents)
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
        
        

        public void ValueChanged(IDirectFactor<T> factor, T oldValue, T newValue, out bool removeSubscription)
        {
            if (IsReacting) { Debug.Fail($"Update loop in {NameOf<ReactiveCore<T>>()} => {this}."); }

            removeSubscription = false;

            if (HasBeenTriggered is false)
            {
                HasBeenTriggered = true;
                InvalidateOutcome(factor);
                
                if (IsNecessary)
                {
                    UpdateOutcome(factor, oldValue, newValue);
                }
            }
            
        }
        
        public bool Trigger(IDirectFactor triggeringFactor, out bool removeSubscription)
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
                
                if (IsNecessary || DestabilizeSubscribers())
                {
                }
                
                return true;
            }
    
            return false;
        }
        
        protected virtual void OnNecessary()
        {
            // core.OnNecessary();

            if (HasBeenTriggered || IsUnstable)
            {
                AttemptReaction();
                //- TODO : This seems like it could lead to some weird behavior when combined with the fact that Reactives
                //         already update before returning a value. Think about it for a bit when you get a chance.
            }
            
            //- TODO : Should we check if we're already reacting, or if we're queued?
        }

        protected virtual void OnNotNecessary()
        {
            if (HasReacted)
            {
                foreach (var trigger in Triggers)
                {
                    trigger.NotifyNotNecessary(this);
                }
            }
        }
        
        public bool Reconcile()
        {
            if (HasBeenTriggered)
            {
                if (IsQueued is false)
                {
                    CreateOutcome();
                }

                return false;
            }
            else if (IsUnstable)
            {
                if (TryStabilizeOutcome())
                {
                    return true;
                }
                else return false;
            }
            else return true;

            //- This method is intended to be used by factors that have been made unstable.
            //  If they were made unstable, then we (or another Reactor) were either triggered or made unstable,
            // and in turn made them unstable. If we are no longer triggered or unstable then we reacted
            // before this call, and we would have triggered our subscribers if the reaction 'changed something'.
            // Since they're trying to reconcile, then they haven't been triggered, so we accept the reconciliation.

            //- TODO : We could probably just return "AttemptReaction() is false" if we can guarantee that
            //         AttemptReaction() is always going to be based on HasBeenTriggered and IsUnstable anyways.
        }

        protected bool UpdateOutcome(IDirectFactor<T> directFactor, T oldValue, T newValue)
        {
            if (IsReacting)                { Debug.Fail($"Update loop in {NameOf<ReactiveCore<T>>()} => {this}."); }
            if (HasBeenTriggered is false) { InvalidateOutcome(null); }
            
            bool outcomeChanged;

            IsReacting         = true;
            IsUnstable         = false;
            HasBeenTriggered   = false;
            
            try
            {
                outcomeChanged = UpdateOutcome(directFactor1, oldValue, newValue);
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
            
            if (outcomeChanged)
            {
                VersionNumber++;
            }
        }
        
        
        protected abstract void InvalidateOutcome(IDirectFactor<T> changedFactor);

        #endregion
        
        
        #region Explicit Implementations

        void IUpdateable.Update()
        {
            IsQueued = false;
            AttemptReaction();
        }

        #endregion
    }
    

    public class PrintCore<T> : CoreBase<T>
    {
        protected override IEnumerable<IDirectFactor> Triggers         { get; }
        public    override bool                       HasTriggers      { get; }
        public    override int                        NumberOfTriggers { get; }


        protected override bool CreateOutcome(T oldValue, T newValue)
        {
            Console.WriteLine(newValue);
            return true;
        }

        protected override void InvalidateOutcome(IDirectFactor<T> changedFactor)
        {
            
        }
    }
}