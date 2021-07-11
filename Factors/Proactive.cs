using System.Collections.Generic;
using Causality.States;
using Core.Causality;
using Core.Factors;
using JetBrains.Annotations;
using static Core.InterlockedUtils;
using static Core.Tools.Types;

namespace Factors
{
    public class Proactive<T> : ProactiveValue<T>
    {
        #region Instance Fields

        [NotNull]
        protected readonly IEqualityComparer<T> valueComparer;

        #endregion
        
        #region Static Properties

        public static IEqualityComparer<T> DefaultValueComparer { get; set; } = EqualityComparer<T>.Default;
        
        #endregion
        
        
        #region Instance Properties
        
        public T Value
        {
            get => state.Value;
            set
            {
                IState<T> oldState = state;

                if (TrySetValue(value, oldState))
                {
                    oldState.Invalidate();
                }
            }
        }

        #endregion


        #region Instance Methods
        
        private bool TrySetValue(T valueToSet, [NotNull] IState<T> oldState)
        {
            bool valuesAreDifferent = valueComparer.Equals(oldState.Value, valueToSet) == false;
            bool valueWasSet        = false;

            if (valuesAreDifferent)
            {
                State<T> newState = new State<T>(valueToSet);
                    
                while ((valueWasSet == false)  &&  valuesAreDifferent)
                {
                    if (TryCompareExchangeOrSet(ref state, newState, ref oldState))
                    {
                        valueWasSet = true;
                    }
                    else
                    {
                        valuesAreDifferent = (valueComparer.Equals(oldState.Value, valueToSet) == false);
                    }
                }
            }

            return valueWasSet;
        }

        protected override bool ValuesAreDifferent(T firstValue, T secondValue) => valueComparer.Equals(firstValue, secondValue) == false;

        public override string ToString() => $"Value => {Value}";

        #endregion

        #region Operators

        public static implicit operator T(Proactive<T> proactive) => proactive.Value;

        #endregion


        #region Constructors

        public Proactive(T initialValue, string name) : this(initialValue, DefaultValueComparer, name)
        {

        }
        
        public Proactive(T initialValue, IEqualityComparer<T> comparer = null, string name = null) :
            base(initialValue, name ?? NameOf<Proactive<T>>())
        {
            valueComparer = comparer?? DefaultValueComparer;
        }

        #endregion
    }
}
