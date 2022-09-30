using System;
using Core.Factors;
using static Factors.Time.Time;

namespace Factors.Time
{
    public abstract class TimeDistance
    {
        #region Instance Fields

        protected readonly FloatingTimeZone timeZone;
        protected          DateTime         dateToCompare;

        #endregion

        
        #region Properties

        protected FloatingTimeZone TimeZone      => timeZone;
        protected DateTime         DateToCompare => dateToCompare;
        
        protected int Days    => InvolveTimersForNextAndPreviousIncrement(Snapshot.Days,    Snapshot.TrimToDays(),    OneDay);
        protected int Hours   => InvolveTimersForNextAndPreviousIncrement(Snapshot.Hours,   Snapshot.TrimToHours(),   OneHour);
        protected int Minutes => InvolveTimersForNextAndPreviousIncrement(Snapshot.Minutes, Snapshot.TrimToMinutes(), OneMinute);
        protected int Seconds => InvolveTimersForNextAndPreviousIncrement(Snapshot.Seconds, Snapshot.TrimToSeconds(), OneSecond);
        
        protected abstract TimeSpan Snapshot { get; }

        #endregion

        #region Instance Methods

        protected abstract int InvolveTimersForNextAndPreviousIncrement(int returnValue, TimeSpan cut, 
                                                                        TimeSpan incrementLength);

        protected void ThrowIfComparedInstanceBelongsToADifferentTimeZone(TimeDistance instanceToCompare)
        {
            if (instanceToCompare.TimeZone != this.TimeZone)
            {
                throw new InvalidOperationException(
                    $"Cannot compare {nameof(TimeDistance)} instances from different time zones.");
            }
        }

        #endregion

        #region Constructors

        protected TimeDistance(FloatingTimeZone zone, DateTime comparedDate)
        {
            timeZone = zone;
            dateToCompare = comparedDate;
        }

        #endregion
    }
}