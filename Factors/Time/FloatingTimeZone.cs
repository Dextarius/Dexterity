using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Dextarius.Collections;
using Factors.Cores;
using static Factors.Time.Time;

namespace Factors.Time
{
    public abstract class FloatingTimeZone
    {
        #region Static Fields

        protected static          Action<Action> queueActionToBeExecuted = QueueActionUsingSyncContext;
        protected static readonly DateTime       Cancelled               = DateTime.MinValue;
        protected static readonly DateTime       NoActiveTriggers        = DateTime.MaxValue;

        #endregion


        #region Instance Fields

        private Dictionary<DateTime, WeakReference> triggersByExpirationTime = new Dictionary<DateTime, WeakReference>();
        private HashSet<DateTimeTrigger>            expiredTriggers          = new HashSet<DateTimeTrigger>();
        private HashSet<DateTimeTrigger>            activeTriggers           = new HashSet<DateTimeTrigger>();
        private int                                 pressure;
        private DateTime                            zoneTimerExpiration;
        private DateTime?                           stableTime;
        
        #endregion


        #region Properties

        public static FloatingTimeZone Default { get; } = new UtcTimeZone();

        #endregion
        
        
        #region Properties

        public FloatingDateTime Now { get; }

        #endregion
        

        #region Static Methods

        private static void QueueActionUsingSyncContext(Action actionToQueue) => 
            SynchronizationContext.Current.Post(_ => actionToQueue(), null);

        #endregion


        #region Instance Methods

        public DateTime GetStableTime()
        {
            if (stableTime is null)
            {
                queueActionToBeExecuted(ResetStableTimeToNull);
                stableTime = GetRawTime();
            }

            return stableTime.Value;
        }
        
        private void ResetStableTimeToNull() => stableTime = null;

        protected void OnZoneTimerExpired() => queueActionToBeExecuted(HandleTriggerExpired);

        private void HandleTriggerExpired()
        {
            zoneTimerExpiration = DateTime.MinValue;
            ProcessTriggers();
        }

        internal DateTimeTrigger GetATriggerThatExpiresAt(DateTime triggerExpiration)
        {
            if (triggersByExpirationTime.TryGetValue(triggerExpiration, out var triggerReference))
            {
                if (triggerReference.Target is DateTimeTrigger trigger)
                {
                    return trigger;
                }
            }

            if (pressure * 2 >= triggersByExpirationTime.Count)
            {
                foreach (var time in triggersByExpirationTime.Keys.ToList())
                {
                    triggerReference = triggersByExpirationTime[time];
                        
                    if (triggerReference.IsAlive is false)
                    {
                        triggersByExpirationTime.Remove(time);
                    }
                }

                pressure = 0;
            }

            DateTimeTrigger createdTrigger = null; // new DateTimeTrigger(triggerExpiration); 
            //^ TODO : I haven't finished making constructors for the triggers yet, so they're commented out for now.
            throw new NotImplementedException();

            triggersByExpirationTime[triggerExpiration] = new WeakReference(createdTrigger);
            pressure++;

            return createdTrigger;
        }

        internal DateTimeTrigger GetATriggerThatExpiresIn(TimeSpan timeUntilExpiration) => 
            GetATriggerThatExpiresAt(Now.Snapshot + timeUntilExpiration);

        //- TrackTrigger?
        internal void AddTriggerToQueue(DateTimeTrigger trigger)
        {
            if (trigger.IsExpired) { expiredTriggers.Add(trigger); }
            else                   {  activeTriggers.Add(trigger); }
            
            ProcessTriggers();
        }
        
        internal void RemoveTriggerFromQueue(DateTimeTrigger trigger)
        {
            expiredTriggers.Remove(trigger);
            activeTriggers.Remove(trigger);
        }
        
        protected void ProcessTriggers()
        {
            DateTime now                           = GetStableTime();
            var      expiredTriggersToReactivate   = expiredTriggers.Where(trigger => trigger.ExpirationTime >  now).ToList(); 
            var      activeTriggersThatHaveExpired =  activeTriggers.Where(trigger => trigger.ExpirationTime <= now).ToList();

            foreach (var triggerToReactivate in expiredTriggersToReactivate)
            {
                expiredTriggers.Remove(triggerToReactivate);
                activeTriggers.Add(triggerToReactivate);
                triggerToReactivate.SetIsExpiredTo(false);
            }

            foreach (var triggerThatHasExpired in activeTriggersThatHaveExpired)
            {
                activeTriggers.Remove(triggerThatHasExpired);
                expiredTriggers.Add(triggerThatHasExpired);
                triggerThatHasExpired.SetIsExpiredTo(true);
            }

            SetZoneTimerToMatchNextActiveTriggerToExpire(now);
        }

        private void SetZoneTimerToMatchNextActiveTriggerToExpire(DateTime now)
        {
            var  nextTriggerToExpire    = activeTriggers.OrderBy(trigger => trigger.ExpirationTime).FirstOrDefault();
            bool noActiveTriggersExist  = nextTriggerToExpire is null;
            bool noExpiredTriggersExist = expiredTriggers.Count == 0;

            if (noActiveTriggersExist && noExpiredTriggersExist)
            {
                if (zoneTimerExpiration != Cancelled)
                {
                    zoneTimerExpiration = Cancelled;
                    CancelZoneTimer();
                }
            }
            else if (noActiveTriggersExist)
            {
                if (zoneTimerExpiration != NoActiveTriggers)
                {
                    zoneTimerExpiration = NoActiveTriggers;
                    SetZoneTimer(OneSecond);
                }
            }
            else if (zoneTimerExpiration != nextTriggerToExpire.ExpirationTime)
            {
                TimeSpan timeUntilNextTriggerExpiration = zoneTimerExpiration - now;
                TimeSpan newZoneTimerLength;

                if (timeUntilNextTriggerExpiration > OneSecond) { newZoneTimerLength = OneSecond; }
                else                                            { newZoneTimerLength = timeUntilNextTriggerExpiration; }
                
                zoneTimerExpiration = nextTriggerToExpire.ExpirationTime;
                SetZoneTimer(newZoneTimerLength);
            }
        }
        
        public void SetQueuingAction(Action<Action> queueingAction) => queueActionToBeExecuted = queueingAction;

        protected abstract DateTime GetRawTime();
        protected abstract void     CancelZoneTimer();
        protected abstract void     SetZoneTimer(TimeSpan delay);

        #endregion


        #region Constructors

        protected FloatingTimeZone()
        {
            Now = new FloatingDateTime(this);
        }

        #endregion
    }
}