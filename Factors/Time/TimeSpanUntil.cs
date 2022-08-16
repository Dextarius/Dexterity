using System;

namespace Factors.Time
{
    public class TimeSpanUntil : TimeDistance
    {
        protected override TimeSpan Snapshot => dateToCompare - timeZone.GetStableTime();
        
        
        protected override int InvolveTimersForNextAndPreviousIncrement(
            int returnValue, TimeSpan trimmedTimeSpan, TimeSpan incrementLength)
        {
            TimeSpan timeSpan;

            if (Snapshot <= TimeSpan.Zero) { timeSpan = -trimmedTimeSpan - incrementLength;       }
            else                           { timeSpan =  trimmedTimeSpan - TimeSpan.FromTicks(1); }

            DateTimeTrigger.GetATriggerThatExpiresAt(dateToCompare - timeSpan,                   timeZone); //- NotifyInvolved()
            DateTimeTrigger.GetATriggerThatExpiresAt(dateToCompare - timeSpan - incrementLength, timeZone); //- NotifyInvolved()

            return returnValue;
        }
        
        
        public TimeSpanUntil(FloatingTimeZone zone, DateTime startPoint) : base(zone, startPoint)
        {
            
        }
    }
}