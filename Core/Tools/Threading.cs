using System;
using System.Threading;

namespace Core.Tools
{
    public static class Threading
    {
        public static void StartNewThreadThatRuns(ThreadStart delegateToRun, bool useBackgroundThread = false)
        {
            _ = delegateToRun ?? throw new ArgumentNullException(nameof(delegateToRun));
            
            Thread setValueThread = new Thread(delegateToRun) { IsBackground = useBackgroundThread };
            setValueThread.Start();
        }
    }
}