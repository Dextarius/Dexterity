using System;
using Core.Factors;

namespace Factors.Time
{
    public static class Time
    {
        public static readonly TimeSpan OneSecond = TimeSpan.FromSeconds(1);
        public static readonly TimeSpan OneMinute = TimeSpan.FromMinutes(1);
        public static readonly TimeSpan OneHour   = TimeSpan.FromHours(1);
        public static readonly TimeSpan OneDay    = TimeSpan.FromDays(1);
        public static readonly TimeSpan OneWeek   = TimeSpan.FromDays(7);
        
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