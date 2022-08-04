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

        protected IFactor Owner { get; set; }
        
        #endregion
        
        
        #region Instance Methods

        public void NotifyInvolved()
        {
            if (Owner is null) 
            { 
                throw new InvalidOperationException(
                    $"An {nameof(ObservedProactiveCore<T>)} attempted to notify the Observer that it was involved, " +
                    "but its Owner field is null"); 
            }
            
            CausalObserver.ForThread.NotifyInvolved(Owner);
        }
        
        public void NotifyChanged()
        {
            if (Owner is null) 
            { 
                throw new InvalidOperationException(
                $"An {nameof(ObservedProactiveCore<T>)} attempted to notify the Observer that it had changed, " +
                 "but its Owner field is null"); 
            }
            
            CausalObserver.ForThread.NotifyChanged(Owner);
        }
        
        //^ TODO : Consolidate the error strings for the above methods, they're different by two words.

        public override bool SetValueIfNotEqual(T newValue)
        {
            if (base.SetValueIfNotEqual(newValue))
            {
                NotifyChanged();
                return true;
            }
            else return false;
        }

        //- TODO : Come up with something better than this, it's awkward as heck having to create the core,
        //         then create the Proactive using the core, then use the core to set the Proactive as it's owner.
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