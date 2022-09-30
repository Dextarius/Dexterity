using System;
using System.Collections.Generic;
using Core.States;
using Factors.Cores.ProactiveCores;

namespace Factors.Time
{
    public class DateTimeTrigger : Proactor<ProactorCore>
    {
        #region Instance Fields

        protected readonly FloatingTimeZone timeZone;
        private            bool             isExpired;

        #endregion
        
        
        #region Properties

        public DateTime ExpirationTime { get; }
        public TimeSpan TimeRemaining  => ExpirationTime - timeZone.GetStableTime();

        public bool IsExpired
        {
            get
            {
                //- trigger.NotifyInvolved();
                return isExpired;
            }
            protected set => isExpired = value;
        }
        
        #endregion


        #region Static Methods

        public static DateTimeTrigger GetATriggerThatExpiresIn(TimeSpan timeUntilExpiration, FloatingTimeZone timeZone) =>
          timeZone.GetATriggerThatExpiresIn(timeUntilExpiration);
        
        public static DateTimeTrigger GetATriggerThatExpiresAt(DateTime expirationDate, FloatingTimeZone timeZone) =>
            timeZone.GetATriggerThatExpiresAt(expirationDate);
        
        public static DateTimeTrigger GetATriggerThatExpiresIn(TimeSpan timeUntilExpiration) =>
            GetATriggerThatExpiresIn(timeUntilExpiration, FloatingTimeZone.Default);
        
        public static DateTimeTrigger GetATriggerThatExpiresAt(DateTime expirationDate) =>
            GetATriggerThatExpiresAt(expirationDate, FloatingTimeZone.Default);

        #endregion


        #region Instance Methods
        
        internal void SetIsExpiredTo(bool newValue)
        {
            if (IsExpired != newValue)
            {
                IsExpired = newValue;
                TriggerSubscribers();
            }
        }

        public override bool CoresAreNotEqual(ProactorCore oldCore, ProactorCore newCore) => false;
        
        protected override void OnFirstSubscriberGained()
        {
            base.OnFirstSubscriberGained();
            timeZone.AddTriggerToQueue(this);
        }

        protected override void OnLastSubscriberLost()
        {
            base.OnLastSubscriberLost();
            timeZone.RemoveTriggerFromQueue(this);
        }

        #endregion


        #region Constructors

        public DateTimeTrigger(ProactorCore factorCore, FloatingTimeZone zone, DateTime expirationTime, 
                               string factorsName = nameof(DateTimeTrigger)) : 
            base(factorCore, factorsName)
        {
            timeZone = zone;
            ExpirationTime = expirationTime;
        }

        #endregion
    }
}