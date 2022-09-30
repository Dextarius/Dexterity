using System;
using System.Collections.Generic;
using System.Timers;
using Core.Factors;
using Core.States;
using Timer = System.Timers.Timer;

namespace Factors.Time
{
    public class TimerCore : TimerCoreBase, IFactorSubscriber
    {
        #region Constructors

        public TimerCore(FloatingTimeZone timeZoneToUse) : base(timeZoneToUse)
        {
            
        }
        
        public TimerCore() : this(FloatingTimeZone.Default)
        {
            
        }

        #endregion

        bool ITriggerable.Trigger()
        {
            throw new NotImplementedException();
        }

        bool ITriggerable.Trigger(IFactor triggeringFactor, long triggerFlags, out bool removeSubscription)
        {
            removeSubscription = false; //- Remove this
            
            // if (IsReacting)
            // {
            //     Logging.Notify_ReactorTriggeredWhileUpdating(this, triggeringFactor);
            //
            //     //- If this Outcome is in the update list we should know it's a loop, if it's not then it should be
            //     //  another thread accessing it.
            //     //  Well actually, the parent won't add us to the list until this returns...
            //     //  Don't we add ourselves now?
            // }
            //
            // if (IsTriggered is false)
            // {
            //     IsTriggered = true;
            //     InvalidateOutcome(triggeringFactor);
            //     Debug.Assert(IsQueued is false);
            //
            //     if (IsReflexive || 
            //         AutomaticallyReacts ||
            //         Callback != null && Callback.ReactorTriggered(this))
            //     {
            //         UpdateOutcome();
            //     }
            //
            //     return true;
            // }
    
            return false;
        }
    }
}