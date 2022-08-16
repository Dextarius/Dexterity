using System;
using System.Collections.Generic;
using System.Diagnostics;
using Core.Factors;
using Core.States;

namespace Factors
{
    public class ReactorState : IFactorSubscriber
    {
        #region Instance Fields
        
        protected readonly HashSet<IFactorSubscriber> allSubscribers       = new HashSet<IFactorSubscriber>();
        protected readonly HashSet<IFactorSubscriber> necessarySubscribers = new HashSet<IFactorSubscriber>();

        #endregion

    //  public virtual IInfluenceOwner Owner                   { get; }
        public         bool            IsTriggered             { get; protected set; }
        public         bool            IsUnstable              { get; protected set; }
        public         bool            IsNecessary             { get; protected set; }
        public         bool            CallBackOnTriggered     { get; protected set; }
        public virtual bool            HasNecessarySubscribers => necessarySubscribers.Count > 0;
        public         int             NumberOfSubscribers     => allSubscribers.Count;


        public bool Trigger() => Trigger(null, out _);

        public bool Trigger(IFactor triggeringFactor, out bool removeSubscription)
        {
            removeSubscription = false; //- Remove this
            
            if (IsReacting)
            {
                Logging.Notify_ReactorTriggeredWhileUpdating(this, triggeringFactor);

                //- If this Outcome is in the update list we should know it's a loop, if it's not then it should be
                //  another thread accessing it.
                //  Well actually, the parent won't add us to the list until this returns...
                //  Don't we add ourselves now?
            }
            
            if (IsTriggered is false)
            {
                IsTriggered = true;
                InvalidateOutcome(triggeringFactor);
                Debug.Assert(IsQueued is false);
                
                if (CallBackOnTriggered)
                {
                    Callback.ReactorTriggered(this);
                }

                return true;
            }
    
            return false;
        }
        
        public bool Destabilize(IFactor factor)
        {
            if (IsNecessary || HasNecessarySubscribers)
            {
                return true;
                
                //- We shouldn't need to mark ourselves as Unstable. If we're Necessary then either
                //  we're going to be triggered when our parent updates, or the parent won't
                //  change, in which case we aren't Unstable.
            }
            else if (IsUnstable is false)
            {
                bool hasNecessaryDependents = DestabilizeSubscribers(x);

                return hasNecessaryDependents;
            }
    
            return false;
            
        //- Note : This method used to check IsStabilizing and IsTriggered, but I removed that because it
        //         it no longer seemed relevant.  When we have time, go through all the possible ways this might
        //         called and make sure I'm not missing something.
        }
        
        public virtual bool Subscribe(IFactorSubscriber subscriberToAdd, bool isNecessary)
        {
            if (subscriberToAdd == null) { throw new ArgumentNullException(nameof(subscriberToAdd)); }

            if (allSubscribers.Add(subscriberToAdd))
            {
                if (isNecessary)
                {
                    AddSubscriberAsNecessary(subscriberToAdd);

                    if (NumberOfSubscribers is 1)
                    {
                        Owner.OnFirstSubscriberGained();
                    }
                }

                return true;
            }
            else return false;
        }

        public virtual bool Unsubscribe(IFactorSubscriber subscriberToRemove)
        {
            if (subscriberToRemove != null)
            {
                bool wasRemoved = allSubscribers.Remove(subscriberToRemove);
                
                if (wasRemoved)
                {
                    RemoveSubscriberFromNecessary(subscriberToRemove);

                    if (NumberOfSubscribers is 0)
                    {
                        Owner.OnLastSubscriberLost();
                    }
                }

                return wasRemoved;
            }
            else return false; 
        }
        
        public bool DestabilizeSubscribers(IFactor unstableFactor)
        {
            var subscribersToDestabilize = allSubscribers;

            if (subscribersToDestabilize.Count > 0)
            {
                foreach (var subscriber in subscribersToDestabilize)
                {
                    if (subscriber.Destabilize(unstableFactor))
                    {
                        AddSubscriberAsNecessary(subscriber);  
                        //^ What happens if this makes us Necessary?
                        //  If we're owned by a Reflexive Reactor will it start updating in
                        //  the middle of us destabilizing these subscribers?
                        return true;
                    }
                }

                //- TODO : We specifically tried to avoid using foreach before when triggering subscribers because they might
                //         choose to remove themselves, so consider if we really want to use it here.  
                //-        Well we don't really want to use the RemoveWhere() method like we did in TriggerSubscribers()
                //         because we want to end this method as soon as we find a necessary subscriber.
            }
            
            return false;
        }
        
        protected virtual void AddSubscriberAsNecessary(IFactorSubscriber necessarySubscriber)
        {
            bool wasAlreadyNecessary = HasNecessarySubscribers;
            
            necessarySubscribers.Add(necessarySubscriber);

            if (wasAlreadyNecessary is false)
            {
               // Owner.OnNecessary();
                
                //- TODO : OnNecessary is going to update a Reactor, so do we want that to happen before or after
                //         we add the subscriber?
            }
        }
        
        protected virtual void RemoveSubscriberFromNecessary(IFactorSubscriber unnecessarySubscriber)
        {
            if (HasNecessarySubscribers)
            {
                necessarySubscribers.Remove(unnecessarySubscriber);

                if (HasNecessarySubscribers is false)
                {
                    Owner.OnNotNecessary();
                }
            }
        }
    }
}