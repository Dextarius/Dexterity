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
    }

    public class RepeatingTimer : ReactiveTimerBase<RepeatingTimerCore>
    {
        public TimeSpan InitialDelay { get => core.InitialDelay; set => core.InitialDelay = value; } 
        public TimeSpan RepeatDelay  { get => core.RepeatDelay;  set => core.RepeatDelay = value; } 

        public void Start() => core.Start();
        
        
        public RepeatingTimer(string nameToGive = nameof(ReactiveTimer)) : base(new RepeatingTimerCore(), nameToGive)
        {
        }
        
        public RepeatingTimer(RepeatingTimerCore reactorCore, string nameToGive = nameof(ReactiveTimer)) : 
            base(reactorCore, nameToGive)
        {
        }
    }

}