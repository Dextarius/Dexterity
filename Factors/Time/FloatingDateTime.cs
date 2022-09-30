using System;
using System.Globalization;
using static Factors.Time.Time;

namespace Factors.Time
{
    public class FloatingDateTime // : IComparable<FloatingDateTime>, IEquatable<FloatingDateTime>, 
                                  //   IComparable<DateTime>,         IEquatable<DateTime>,
                                  //   IComparable
    {
        public FloatingTimeZone TimeZone  { get; }
        public TimeSpan         Offset    { get; }
        public DateTime         Snapshot  => TimeZone.GetStableTime() + Offset;
        public DateTime         Date      => FloorCurrentDateAndNotifyInvolved(OneDay);
        public int              Year      => InvolveTimersSetTo(new DateTime(Snapshot.Year,     1, 1), 
                                                                new DateTime(Snapshot.Year + 1, 1, 1), 
                                                                Snapshot.Year);
        public int              Month     => InvolveTimersSetTo(new DateTime(Snapshot.Year, Snapshot.Month, 1), 
                                                                new DateTime(Snapshot.Year, Snapshot.Month, 1).AddMonths(1), 
                                                                Snapshot.Month);
        public int              Day       => Date.Day;
        public int              DayOfYear => Date.DayOfYear;
        public DayOfWeek        DayOfWeek => Date.DayOfWeek;
        public int              Hour      => FloorCurrentDateAndNotifyInvolved(OneHour).Hour;
        public int              Minute    => FloorCurrentDateAndNotifyInvolved(OneMinute).Minute;
        public int              Second    => FloorCurrentDateAndNotifyInvolved(OneSecond).Second;


        protected DateTime FloorCurrentDateAndNotifyInvolved(TimeSpan intervalToRoundTo)
        {
            long ticksInFlooredDate = (Snapshot.Ticks / intervalToRoundTo.Ticks) * intervalToRoundTo.Ticks;
            var  flooredDate        = new DateTime(ticksInFlooredDate);
            
            InvolveTimersSetTo(flooredDate - Offset, intervalToRoundTo);
            //- NotifyInvolved();

            return flooredDate; 
        }
        
        protected T InvolveTimersSetTo<T>(DateTime previousIncrement, DateTime nextIncrement, T returnValue)
        {
            DateTimeTrigger.GetATriggerThatExpiresAt(previousIncrement, TimeZone); //- NotifyInvolved();
            DateTimeTrigger.GetATriggerThatExpiresAt(nextIncrement,     TimeZone); //- NotifyInvolved();

            return returnValue;
        }

        protected void InvolveTimersSetTo(DateTime previousIncrement, TimeSpan incrementLength) =>
            InvolveTimersSetTo(previousIncrement, previousIncrement + incrementLength, default(int));

        protected DateTimeTrigger GetAndInvolveTimer(DateTime originalDateTime)
        {
            var expirationTime = originalDateTime - Offset;
            var timer          = DateTimeTrigger.GetATriggerThatExpiresAt(expirationTime, TimeZone);
            
            //- timer.NotifyInvolved();

            return timer;
        }

        protected DateTimeTrigger GetAndInvolveTimerOneTickAfter(DateTime originalDateTime) =>
            GetAndInvolveTimer(originalDateTime.AddTicks(1));

        public override string ToString() => Snapshot.ToString(CultureInfo.CurrentCulture);

        public FloatingDateTime(FloatingTimeZone timeZone, TimeSpan offset = default)
        {
            TimeZone = timeZone;
            Offset   = offset;
        }

        // public static bool operator >=(FloatingDateTime floatingDateTime, DateTime dateToCompare) =>
        //     floatingDateTime.GetAndInvolveTimer(dateToCompare).IsExpired;
        //
        // public static bool operator <=(FloatingDateTime floatingDateTime, DateTime dateToCompare) =>;
    }
}