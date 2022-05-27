﻿using System;
using System.Collections.Generic;
using Core.Factors;
using Core.States;
using Factors.Observer;
using JetBrains.Annotations;

namespace Factors.Cores.ProactiveCores
{
    public class ObservedProactiveCore<T> : ProactiveCore<T>, IInvolved
    {
        #region Instance Properties

        public override T Value 
        {
            get
            {
                NotifyInvolved();
                return currentValue;
            }
        }
        
        protected IFactor Owner { get; set; }


        #endregion
        
        
        #region Instance Methods

        public void NotifyInvolved() => CausalObserver.ForThread.NotifyInvolved(Owner);
        public void NotifyChanged()  => CausalObserver.ForThread.NotifyChanged(Owner);

        public override bool ChangeValueTo(T newValue)
        {
            if (base.ChangeValueTo(newValue))
            {
                NotifyChanged();
                return true;
            }
            else return false;
        }

        public void SetOwner(IFactor factor)
        {
            if (factor is null) { throw new ArgumentNullException(nameof(factor)); }
            
            if (Owner != null)
            {
                throw new InvalidOperationException(
                    $"A process attempted to set the Owner for a {nameof(ObservedProactiveCore<T>)} to {factor}, " +
                    $"but its Owner property is already assigned to {Owner}. ");
            }
            
            Owner = factor ?? throw new ArgumentNullException(nameof(factor));
        }
        
        public override void Dispose()
        {
            Owner = null;
        }
        
        #endregion
        

        public ObservedProactiveCore(T initialValue, IEqualityComparer<T> comparer = null) : 
            base(initialValue, comparer)
        {
        }
    }
}