using System;
using Core.Factors;

namespace Factors.Time
{
    public abstract class ReactiveTimerBase<TCore> : Proactor<TCore>  where TCore : TimerCoreBase
    {
        #region Properties

        public DateTime ExpirationTime => core.ExpirationTime;
        public TimeSpan TimeRemaining  => core.TimeRemaining;
        public bool     IsExpired      => core.IsExpired;
        public bool     IsRunning      => core.IsRunning;

        #endregion

        
        #region Instance Methods

        public void Cancel() => core.Cancel();

        #endregion


        #region Constructors


        protected ReactiveTimerBase(TCore reactorCore, string nameToGive = nameof(ReactiveTimerBase<TCore>)) : 
            base(reactorCore, nameToGive)
        {
        }

        #endregion
    }
}