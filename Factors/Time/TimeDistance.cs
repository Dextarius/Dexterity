using System;
using Core.Factors;
using static Factors.Time.Time;

namespace Factors.Time
{
    public abstract class TimeDistance
    {
        protected readonly FloatingTimeZone timeZone;
        protected          DateTime         dateToCompare;

        protected FloatingTimeZone TimeZone      => timeZone;
        protected DateTime         DateToCompare => dateToCompare;
        protected int              Days          => InvolveTimersForNextAndPreviousIncrement(Snapshot.Days,    new TimeSpan(Snapshot.Days, 0,                             0,                0), OneDay);
        protected int              Hours         => InvolveTimersForNextAndPreviousIncrement(Snapshot.Hours,   new TimeSpan(Snapshot.Days, Snapshot.Hours,                0,                0), OneHour);
        protected int              Minutes       => InvolveTimersForNextAndPreviousIncrement(Snapshot.Minutes, new TimeSpan(Snapshot.Days, Snapshot.Hours, Snapshot.Minutes,                0), OneMinute);
        protected int              Seconds       => InvolveTimersForNextAndPreviousIncrement(Snapshot.Seconds, new TimeSpan(Snapshot.Days, Snapshot.Hours, Snapshot.Minutes, Snapshot.Seconds), OneSecond);
        
        protected abstract TimeSpan Snapshot { get; }

        protected abstract int InvolveTimersForNextAndPreviousIncrement(
            int returnValue, TimeSpan cut, TimeSpan incrementLength);

        protected void ThrowIfComparedInstanceBelongsToADifferentTimeZone(TimeDistance instanceToCompare)
        {
            if (instanceToCompare.TimeZone != this.TimeZone)
            {
                throw new InvalidOperationException(
                    $"Cannot compare {nameof(TimeDistance)} instances from different time zones.");
            }
        }

        protected TimeDistance(FloatingTimeZone zone, DateTime startPoint)
        {
            
        }
    }
}