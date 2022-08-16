using System;
using System.Threading;

namespace Factors.Time
{
    public class UtcTimeZone : FloatingTimeZone
    {
        private static readonly TimeSpan MinimumTimerLength = TimeSpan.FromMilliseconds(20);

        private readonly Timer zoneTimer;

        protected override DateTime GetRawTime() => DateTime.UtcNow;

        protected override void SetZoneTimer(TimeSpan lengthOfTime)
        {
            if (lengthOfTime < MinimumTimerLength)
            {
                lengthOfTime = MinimumTimerLength;
            }

            zoneTimer.Change(lengthOfTime.Milliseconds, Timeout.Infinite);
        }

        protected override void CancelZoneTimer() => zoneTimer.Change(Timeout.Infinite, Timeout.Infinite);

        private void ZoneTimerExpired(object unused)
        {
            zoneTimer.Change(Timeout.Infinite, Timeout.Infinite);
            OnZoneTimerExpired();
        }

        public UtcTimeZone()
        {
            zoneTimer = new Timer(ZoneTimerExpired, null, Timeout.Infinite, Timeout.Infinite);
        }
    }
}