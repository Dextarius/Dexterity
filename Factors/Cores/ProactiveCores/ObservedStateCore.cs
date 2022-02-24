using System.Collections.Generic;
using Core.Factors;
using Core.States;
using Factors.Observer;
using JetBrains.Annotations;

namespace Factors.Cores.ProactiveCores
{
    public class ObservedStateCore<T> : StateCore<T>, IInvolved
    {
        #region Instance Properties

        public override T Value 
        {
            get
            {
                NotifyInvolved();
                return currentValue;
            }
            set
            {
                bool valueIsDifferent = valueComparer.Equals(value, currentValue) is false;
                
                if (valueIsDifferent)
                {
                    currentValue = value;
                    NotifyChanged();
                    TriggerSubscribers();
                }
            }
        }

        #endregion
        
        
        #region Instance Methods

        public void NotifyInvolved() => CausalObserver.ForThread.NotifyInvolved(this);
        public void NotifyChanged()  => CausalObserver.ForThread.NotifyChanged(this);
        
        #endregion
        

        public ObservedStateCore(T initialValue, string name = null, IEqualityComparer<T> comparer = null) : 
            base(initialValue, name, comparer)
        {
        }

        public ObservedStateCore(T initialValue, IEqualityComparer<T> comparer) : this(initialValue, null, comparer)
        {
        }
    }
}