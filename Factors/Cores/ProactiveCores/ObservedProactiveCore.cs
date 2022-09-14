using System;
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
                T value = base.Value;
                
                //^ Getting the Value of a Factor often causes changes in factors, so grab it before we NotifyInvolved(). 
                NotifyInvolved();

                return value;
            }
        }

        #endregion
        
        
        #region Instance Methods

        public void NotifyInvolved(long triggerFlags)
        {
            if (Callback is not null) 
            { 
                throw new InvalidOperationException(
                    $"An {nameof(ObservedProactiveCore<T>)} attempted to notify the Observer that it was involved, " +
                    $"but its {nameof(Callback)} field is null"); 
                //- TODO : Should this really throw?  I feel this is going to happen a fair amount
                //         since this method is primarily used by callers other than ourselves.
            }
            
            Observer.NotifyInvolved(Callback, triggerFlags);
        }

        public void NotifyInvolved() => NotifyInvolved(TriggerFlags.Default);

        public void NotifyChanged()
        {
            if (Callback is null) 
            { 
                throw new InvalidOperationException(
                $"An {nameof(ObservedProactiveCore<T>)} attempted to notify the Observer that it had changed, " +
                $"but its {nameof(Callback)} field is null"); 
            }
            
            CausalObserver.ForThread.NotifyChanged(Callback);
        }
        //^ TODO : Consolidate the error strings for the above methods, they're different by two words.

        public override bool SetValueIfNotEqual(T newValue)
        {
            if (base.SetValueIfNotEqual(newValue))
            {
                NotifyChanged();  //- Should this happen before we trigger our subscribers?
                return true;
            }
            else return false;
        }

        #endregion
        

        public ObservedProactiveCore(T initialValue, IEqualityComparer<T> comparer = null) : 
            base(initialValue, comparer)
        {
        }
    }
}