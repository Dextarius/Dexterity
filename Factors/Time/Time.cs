using System;
using Core.Factors;

namespace Factors.Time
{
    public static class Time
    {
        #region Constants

        public enum Month { January = 1, February, March, April, May, June, July, August, September, October, November, December }

        #endregion
        
        
        #region Fields

        public static readonly TimeSpan OneSecond = TimeSpan.FromSeconds(1);
        public static readonly TimeSpan OneMinute = TimeSpan.FromMinutes(1);
        public static readonly TimeSpan OneHour   = TimeSpan.FromHours(1);
        public static readonly TimeSpan OneDay    = TimeSpan.FromDays(1);
        public static readonly TimeSpan OneWeek   = TimeSpan.FromDays(7);

        #endregion


        #region Methods

        //- Any year that is a multiple of 4 is a leap year: for example, 2004, 2008,
        //  and 2012 are leap years. There is an exception though. If a year is a multiple
        //  of 100 (i.e. 1700) it is not a leap year, unless it is also also a multiple of 400.
        //  As an example and 1600 would be a leap year, but 1900 would not.
        public static bool IsLeapYear(int year) => year %   4 == 0 && 
                                                  (year % 100 != 0 || 
                                                   year % 400 == 0);

        public static bool IsNotLeapYear(int year) => IsLeapYear(year) is false;


        public static TimeSpan TrimToDays(this TimeSpan timeSpan) => new TimeSpan(timeSpan.Days, 0, 0, 0);

        public static TimeSpan TrimToHours(this TimeSpan timeSpan) => new TimeSpan(timeSpan.Days, timeSpan.Hours, 0, 0);

        public static TimeSpan TrimToMinutes(this TimeSpan timeSpan) => 
            new TimeSpan(timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, 0);

        public static TimeSpan TrimToSeconds(this TimeSpan timeSpan) => 
            new TimeSpan(timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
        
        public static DateTime TrimToYear(this DateTime date)  => new DateTime(date.Year, (int)Month.January, 1);

        public static DateTime TrimToMonth(this DateTime date) => new DateTime(date.Year, date.Month, 1);
        
        public static DateTime TrimToDay(this DateTime date) => new DateTime(date.Year, date.Month, date.Day);

        public static DateTime TrimToHour(this DateTime date) => 
            new DateTime(date.Year, date.Month, date.Day, date.Hour, 0, 0);

        public static DateTime TrimToMinute(this DateTime date) => 
            new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, 0);

        public static DateTime TrimToSecond(this DateTime date) => 
            new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
        
        public static DateTime NextYear(this DateTime date) => new DateTime(date.Year + 1, (int)Month.January, 1);
        
        public static DateTime NextMonth(this DateTime date)
        {
            if (date.Month is (int) Month.December) { return new DateTime(date.Year + 1, (int) Month.January, 1); }
            else                                    { return new DateTime(date.Year,     date.Month + 1,      1); }
        }

        public static DateTime NextDay(this DateTime date)
        {
            if (IsLastDayInMonth(date)) { return date.NextMonth();                                  }
            else                        { return new DateTime(date.Year, date.Month, date.Day + 1); }
        }

        public static DateTime NextHour(this DateTime date) 
        {
            if (date.Hour is 24) { return date.NextDay(); }
            else                 { return new DateTime(date.Year, date.Month, date.Day, date.Hour + 1, 0, 0); }
        }

        //^ We totally could have made those NextX() methods by just calling date.AddX().TrimToX().
        
        public static bool IsLastDayInMonth(int day, Month month, int year)
        {
            if (day < 28)
            {
                return false;
            }
            else if (day > 31)
            {
                throw new ArgumentOutOfRangeException(nameof(day), "Day cannot be higher than 31. ");
            }
            else
            {
                int lastDayInMonth;
                
                switch (month)
                {
                    case Month.January:   lastDayInMonth = 31;                         break;
                    case Month.February:  lastDayInMonth = IsLeapYear(year) ? 29 : 28; break;
                    case Month.March:     lastDayInMonth = 31;                         break;
                    case Month.April:     lastDayInMonth = 30;                         break;
                    case Month.June:      lastDayInMonth = 30;                         break;
                    case Month.August:    lastDayInMonth = 31;                         break;
                    case Month.September: lastDayInMonth = 30;                         break;
                    case Month.November:  lastDayInMonth = 30;                         break;
                    case Month.December:  lastDayInMonth = 31;                         break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(month), month, $"Unhandled case for month {month}. ");
                }

                if (day > lastDayInMonth)
                {
                    throw new ArgumentOutOfRangeException($"Day number {day} is too high for month {month}. ");
                }
                else
                {
                    return day == lastDayInMonth;
                }
            }
        }

        public static bool IsLastDayInMonth(DateTime date) => IsLastDayInMonth(date.Day, (Month)date.Month, date.Year);

        public static bool IsLastDayOfTheYear(DateTime date) => date.Day is 31 && date.Month is (int)Month.December;

        

        

        #endregion


        // public static readonly FloatingDateTime OneSecondFromNow = TimeSpan.FromSeconds(1);
        // public static readonly FloatingDateTime OneMinuteFromNow = TimeSpan.FromMinutes(1);
        // public static readonly FloatingDateTime OneHourFromNow   = TimeSpan.FromHours(1);
        // public static readonly FloatingDateTime OneDayFromNow    = TimeSpan.FromDays(1);
        // public static readonly FloatingDateTime OneWeekFromNow   = TimeSpan.FromDays(7);
        //
        // public static readonly IFactor EverySecond = CreateTimerThatGoesOffEvery(OneSecond);
        // public static readonly IFactor EveryMinute = CreateTimerThatGoesOffEvery(OneMinute);
        // public static readonly IFactor EveryHour   = CreateTimerThatGoesOffEvery(OneHour);
        // public static readonly IFactor EveryDay    = CreateTimerThatGoesOffEvery(OneDay);
        //
        // private static IFactor CreateTimerThatGoesOffEvery(TimeSpan interval)
        // {
        //     
        //     return new ReactiveTimer();
        // }
    }
}