using System.Collections.Generic;
using Core.States;
using JetBrains.Annotations;

namespace Factors.Cores.ProactiveCores
{
    public class DirectStateCore<T> : StateCore<T>
    {
        #region Instance Properties

        public override T Value 
        {
            get => currentValue;
            set
            {
                bool valueIsDifferent = valueComparer.Equals(value, currentValue) is false;
                
                if (valueIsDifferent)
                {
                    currentValue = value;
                    TriggerSubscribers();
                }
            }
        }
        
        #endregion


        #region Constructors

        public DirectStateCore(T initialValue, string name = null, IEqualityComparer<T> comparer = null) : 
            base(initialValue, name, comparer)
        {
        }

        public DirectStateCore(T initialValue, IEqualityComparer<T> comparer) : this(initialValue, null, comparer)
        {
        }

        #endregion
    }
}