using System;
using System.Threading;

namespace Factors.Time
{
    public class RepeatingTimerCore : TimerCoreBase
    {
        public TimeSpan InitialDelay { get; set; } = Timeout.InfiniteTimeSpan;
        public TimeSpan RepeatDelay  { get; set; } = Timeout.InfiniteTimeSpan;

        //- Consider making it so that if you change the repeat delay while the timer is repeating, it
        //  modifies the current timer.
        
        public void Start()
        {
            if (InitialDelay != Timeout.InfiniteTimeSpan)
            {
                SetToExpireIn(InitialDelay);
            }
        }
        public RepeatingTimerCore(FloatingTimeZone timeZoneToUse) : base(timeZoneToUse)
        {
            
        }
        
        public RepeatingTimerCore() : this(FloatingTimeZone.Default)
        {
            
        }
    }
}