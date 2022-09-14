using System.Collections.Generic;
using Core.Factors;
using Core.States;

namespace Factors
{
    public class ProactiveCondition : Proactive<bool>, ICondition
    {
        private Proactor onTrue;
        private Proactor onFalse;
        
        
        public bool IsTrue  => Value is true;
        public bool IsFalse => Value is false;

        public IFactor OnTrue  => onTrue ??= new Proactor();
        public IFactor OnFalse => onFalse ??= new Proactor();

        
        protected override void OnUpdated(long triggerFlags)
        {
            base.OnUpdated(triggerFlags);

            if (IsTrue)
            {
                onTrue?.TriggerSubscribers();
            }
            else
            {
                onFalse?.TriggerSubscribers();
            }
        }
        
        public ProactiveCondition(IProactiveCore<bool> core, string name = null) : base(core, name)
        {
        }
        
        public ProactiveCondition(bool initialValue, string name = null) : base(initialValue, name)
        {
        }
    }
}