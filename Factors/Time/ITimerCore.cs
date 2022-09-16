using System;
using Core.Factors;

namespace Factors.Time
{
    public interface ITimerCore : IProactorCore
    {
        DateTime ExpirationTime { get; }
        TimeSpan TimeRemaining  { get; }
        bool     IsExpired      { get; }
        bool     IsRunning      { get; }
    }
}