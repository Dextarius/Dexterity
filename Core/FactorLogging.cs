using System.Diagnostics;
using Core.Factors;
using Core.States;
using static Core.Tools.Types;

namespace Core
{
    //- TODO : Come back and actually flesh this out.  It should handle the logging more
    //         flexibly than just throwing out Trace messages.
    public class FactorLogging : IFactorLogging
    {
        public bool IsEnabled { get; set; }

        public void Notify_ReactorTriggeredWhileUpdating<TReactor>(TReactor triggeredReactor, IFactor triggeringFactor)
        {
            if (IsEnabled)
            {
                Trace.WriteLine(
                    $"A {NameOf<TReactor>()} was invalidated while it was updating meaning either, the object's update process " +
                    "caused it to invalidate itself creating an update loop, " +
                    "or the object was accessed by two different threads at the same time. \n  " +
                    $"The triggered Reactor was '{triggeredReactor}' and it was invalidated by '{triggeringFactor}'. "); 
            }
        }

        public void Notify_ReactorHasRecursiveDependency<TReactor>(TReactor reactor, IFactorSubscriber subscriber)
        {
            if (IsEnabled)
            {
                Trace.WriteLine($"An object subscribed to a {NameOf<TReactor>()} while it was in the process of reacting, " +
                                 "which means it may have recursive dependency. \n" +
                                $"The Reactor was '{reactor}' and the subscriber was {subscriber}. ");
                
                //- TODO : See if this still makes sense.  I'm not sure the recursion part still applies after
                //         all of the changes we've made to the subscription process. 
            }
        }

    }
}