using System;
using System.Timers;

namespace Factors.Time
{
    public class UsableTimer : Proactor<TimerCore>
    {
        #region Properties

        public DateTime ExpirationTime => core.ExpirationTime;
        public TimeSpan TimeRemaining  => core.TimeRemaining;
        public bool     IsExpired      => core.IsExpired;
        public bool     IsRunning      => core.IsRunning;

        #endregion

        
        #region Instance Methods

        public void SetToExpireAt(DateTime expirationDate) => core.SetToExpireAt(expirationDate);
        public void SetToExpireIn(TimeSpan duration)       => core.SetToExpireIn(duration);
        public void Cancel()                               => core.Cancel();

        #endregion


        #region Constructors

        public UsableTimer(string nameToGive = nameof(UsableTimer)) : base(new TimerCore(), nameToGive)
        {
        }
        
        public UsableTimer(TimerCore reactorCore, string nameToGive = nameof(UsableTimer)) : 
            base(reactorCore, nameToGive)
        {
        }

        #endregion
    }
}