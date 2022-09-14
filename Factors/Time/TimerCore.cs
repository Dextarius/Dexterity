using System;
using System.Collections.Generic;
using System.Timers;
using Core.Factors;
using Core.States;
using Timer = System.Timers.Timer;

namespace Factors.Time
{
    public class TimerCore : TimerCoreBase, IFactorSubscriber
    {
        #region Instance Methods

        public void SetToExpireAt(DateTime expirationDate)
        {
            if (expirationDate < timeZone.GetStableTime())
            {
                Cancel();
                Callback?.CoreUpdated(this, TriggerFlags.Default);
            }
            else
            {
                var oldTimer = currentTimer;
                var newTimer = timeZone.GetATriggerThatExpiresAt(expirationDate);

                if (newTimer != oldTimer)
                {
                    oldTimer?.Unsubscribe(weakSubscriber);
                    currentTimer = newTimer;
                    newTimer.Subscribe(this, false);

                    if (newTimer.IsExpired)
                    {
                        // Trigger(currentTimer, out _);
                    }
                    else
                    {
                        IsRunning = true;
                    }
                }
            }
        }

        public void SetToExpireIn(TimeSpan duration) => SetToExpireAt(timeZone.GetStableTime() + duration);

        private bool OnTriggered(IFactor triggeringFactor)
        {
            bool timerIsExpired = currentTimer != null && 
                                  currentTimer.IsExpired;

            if (timerIsExpired && IsRunning)
            {
                StopTimer();
                Callback?.CoreUpdated(this, TriggerFlags.Default);

                return true;
            }
            else return false;
        }
        
        private void StopTimer()
        {
            var oldTimer = currentTimer;

            IsRunning = false;
            currentTimer = null;
            oldTimer?.Unsubscribe(weakSubscriber);
        }

        public void Cancel()
        {
            if (IsRunning)
            {
                StopTimer();
            }
        }

        #endregion


        #region Constructors

        public TimerCore(FloatingTimeZone timeZoneToUse) : base(timeZoneToUse)
        {
            
        }
        
        public TimerCore() : this(FloatingTimeZone.Default)
        {
            
        }

        #endregion

        bool ITriggerable.Trigger()
        {
            throw new NotImplementedException();
        }

        bool ITriggerable.Trigger(IFactor triggeringFactor, long triggerFlags, out bool removeSubscription)
        {
            removeSubscription = false; //- Remove this
            

            
            // if (IsReacting)
            // {
            //     Logging.Notify_ReactorTriggeredWhileUpdating(this, triggeringFactor);
            //
            //     //- If this Outcome is in the update list we should know it's a loop, if it's not then it should be
            //     //  another thread accessing it.
            //     //  Well actually, the parent won't add us to the list until this returns...
            //     //  Don't we add ourselves now?
            // }
            //
            // if (IsTriggered is false)
            // {
            //     IsTriggered = true;
            //     InvalidateOutcome(triggeringFactor);
            //     Debug.Assert(IsQueued is false);
            //
            //     if (IsReflexive || 
            //         AutomaticallyReacts ||
            //         Callback != null && Callback.ReactorTriggered(this))
            //     {
            //         UpdateOutcome();
            //     }
            //
            //     return true;
            // }
    
            return false;
        }
        
        bool IDestabilizable.Destabilize()
        {
            //- I can't think of any reason why this would be called. Right now the plan is to have
            //  this core subscribe to a DateTimeTrigger, and they shouldn't use Destabilize().
            throw new NotImplementedException();
        }
    }
}