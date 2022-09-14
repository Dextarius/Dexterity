using System;
using System.Threading;
using Core.Factors;
using Core.States;
using Factors.Cores;
using Factors.Cores.ProactiveCores;

namespace Factors.Time
{
    public abstract class TimerCoreBase : ProactorCore, ITimerCore, IFactorSubscriber
    {
        #region Instance Fields

        protected readonly WeakSubscriber   weakSubscriber;
        protected readonly FloatingTimeZone timeZone;
        protected          DateTimeTrigger  currentTimer;

        #endregion
        
        
        #region Properties

        public DateTime ExpirationTime => currentTimer?.ExpirationTime ?? DateTime.MinValue;
        public TimeSpan TimeRemaining  => currentTimer?.TimeRemaining  ?? TimeSpan.Zero;
        public bool     IsExpired      => currentTimer?.IsExpired      ?? true;
        public bool     IsRunning      { get; protected set; }
        
        #endregion


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

        public void SetToExpireIn(TimeSpan duration)
        {
            if (duration == Timeout.InfiniteTimeSpan)
            {
                Cancel();
            }
            else
            {
                SetToExpireAt(timeZone.GetStableTime() + duration);
            }
        }

        protected void GetTimerThatExpiresAt(DateTime expirationDate)
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

        protected TimerCoreBase(FloatingTimeZone timeZoneToUse)
        {
            timeZone = timeZoneToUse;
            weakSubscriber = new WeakSubscriber(this);
        }
        
        #endregion
        

        #region Explicit Implementations

        bool ITriggerable.Trigger() => OnTriggered(null);

        bool ITriggerable.Trigger(IFactor triggeringFactor, long triggerFlags, out bool removeSubscription)
        {
            removeSubscription = false;
            return OnTriggered(triggeringFactor);
        }
        
        bool IDestabilizable.Destabilize()
        {
            //- I can't think of any reason why this would be called. Right now the plan is to have
            //  this core subscribe to a DateTimeTrigger, and they shouldn't use Destabilize().
            throw new NotImplementedException();
        }

        #endregion
    }
}