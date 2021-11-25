using System;
using System.Threading;

namespace Core.Threading
{
    public static class Locks
    {
        private static int _monitorEnterTimeout = 10_000;
        private static int _monitorWaitTimeout  = 10_000;
        
        internal static int MonitorEnterTimeout
        {
            get => _monitorEnterTimeout;
            set => Interlocked.Exchange(ref _monitorEnterTimeout, value);
        }
    }
}