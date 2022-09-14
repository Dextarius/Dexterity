﻿using System;
using System.Collections.Generic;
using Core.Factors;
using Core.States;
using JetBrains.Annotations;

namespace Factors
{
    public class ReactiveCondition : Reactive<bool>, ICondition
    {
        private Proactor onTrue;
        private Proactor onFalse;
        
        
        public bool IsTrue  => Value is true;
        public bool IsFalse => Value is false;

        public IFactor OnTrue  => onTrue  ??= new Proactor();
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

        public ReactiveCondition([NotNull] IResult<bool> valueSource, string name = null) : base(valueSource, name)
        {
        }
        
        public ReactiveCondition(Func<bool> functionToDetermineValue, IEqualityComparer<bool> comparer, string name = null) : 
            base(functionToDetermineValue, comparer, name)
        {
        }
        
        public ReactiveCondition(Func<bool> functionToDetermineValue, string name = null) : base(functionToDetermineValue, name)
        {
        }
        
        public ReactiveCondition(IFactor<bool> factorToGetValueOf, string name = null) : base(factorToGetValueOf, name)
        {
        }
    }
}