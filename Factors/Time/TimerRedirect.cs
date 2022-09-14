using System;
using Core.Factors;
using Core.States;

namespace Factors.Time
{
    public class TimerRedirect<T> : IFactor<T>, IFactorSubscriber
    {
        public readonly TimeSpan MinimumLength = TimeSpan.FromMilliseconds(1);
        
        protected readonly FloatingTimeZone timeZone;
        protected          DateTimeTrigger  previousIncrement;
        protected          DateTimeTrigger  nextIncrement;
        protected          DateTime         dateToCompare;
        private            T                value;


        public string Name           { get; }
        public int    UpdatePriority { get; set; } //<-
        public uint   VersionNumber  { get; }
        
        public T Value
        {
            get
            {
                // if (expr)
                // {
                //     
                // }
                return value;
            }
            protected set => this.value = value;
        }

        public TimeSpan Length { get; set; }

        public bool Subscribe(IFactorSubscriber subscriberToAdd, bool isNecessary, long triggerFlags)
        {
            throw new NotImplementedException();
        }
        
        public bool Subscribe(IFactorSubscriber subscriberToAdd, bool isNecessary)
        {
            throw new System.NotImplementedException();
        }

        public void Unsubscribe(IFactorSubscriber subscriberToRemove)
        {
            throw new System.NotImplementedException();
        }

        public void NotifyNecessary(IFactorSubscriber necessarySubscriber)
        {
            throw new System.NotImplementedException();
        }

        public void NotifyNotNecessary(IFactorSubscriber unnecessarySubscriber)
        {
            throw new System.NotImplementedException();
        }

        public bool Reconcile()
        {
            throw new System.NotImplementedException();
        }
        
        public bool ValueEquals(T valueToCompare)
        {
            throw new System.NotImplementedException();
        }
        
        public T Peek()
        {
            throw new System.NotImplementedException();
        }

        bool ITriggerable.Trigger() => Trigger(null, TriggerFlags.Default, out _);
        
        public bool Trigger(IFactor triggeringFactor,  long triggerFlags, out bool removeSubscription)
        {
            removeSubscription = true;
            
            // if (IsNecessary)
            // {
            //     UpdateValue();
            //     TriggerSubscribers();
            //
            //     if (StillHasSubscribers)
            //     {
            //         GetNextTimer();
            //     }
            // }

            return true;
        }
        
        public bool Destabilize()
        {
            throw new System.NotImplementedException();
        }
    }
}