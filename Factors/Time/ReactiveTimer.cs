using System;
using System.Threading;
using System.Timers;

namespace Factors.Time
{
    public class ReactiveTimer : ReactiveTimerBase<TimerCore>
    {
       
        #region Instance Methods

        public void SetToExpireAt(DateTime expirationDate) => core.SetToExpireAt(expirationDate);
        public void SetToExpireIn(TimeSpan duration)       => core.SetToExpireIn(duration);

        #endregion


        #region Constructors

        public ReactiveTimer(string nameToGive = nameof(ReactiveTimer)) : base(new TimerCore(), nameToGive)
        {
        }
        
        public ReactiveTimer(TimerCore reactorCore, string nameToGive = nameof(ReactiveTimer)) : 
            base(reactorCore, nameToGive)
        {
        }

        #endregion

        public override bool CoresAreNotEqual(TimerCore oldCore, TimerCore newCore) =>
            newCore.ExpirationTime != oldCore.ExpirationTime;
        //- TODO : Come back to this later.  We need to decide how we're going to handle
        //         situations where a timer swaps cores to a core with a different
        //         expiration time, since if we trigger our subscribers they may
        //         think the timer went off because the time was up.
    }

    public class RepeatingTimer : ReactiveTimerBase<RepeatingTimerCore>
    {
        public TimeSpan InitialDelay { get => core.InitialDelay; set => core.InitialDelay = value; } 
        public TimeSpan RepeatDelay  { get => core.RepeatDelay;  set => core.RepeatDelay = value; } 

        public void Start() => core.Start();
        
        public override bool CoresAreNotEqual(RepeatingTimerCore oldCore, RepeatingTimerCore newCore) =>
            newCore.ExpirationTime != oldCore.ExpirationTime;
        
        public RepeatingTimer(string nameToGive = nameof(ReactiveTimer)) : base(new RepeatingTimerCore(), nameToGive)
        {
        }
        
        public RepeatingTimer(RepeatingTimerCore reactorCore, string nameToGive = nameof(ReactiveTimer)) : 
            base(reactorCore, nameToGive)
        {
        }
    }

}