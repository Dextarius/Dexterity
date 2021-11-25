using System.Collections.Generic;
using Causality;
using Causality.States;
using Core.Causality;
using Core.Factors;
using Core.States;
using JetBrains.Annotations;
using static Core.InterlockedUtils;
using static Core.Tools.Types;

namespace Factors
{
    public class Proactive<T> : Proactor, IMutableState<T>, IProcess<T>
    {
        #region Instance Fields

        protected IMutableState<T> state;

        #endregion


        #region Properties


        public T Value
        {
            get => state.Value;
            set
            {
                state.Value = value;
            }
        }

        public T Peek() => state.Peek();

        #endregion


        #region Instance Methods
        
        // private bool TrySetValue(T valueToSet, [NotNull] IMutableState<T> oldState)
        // {
        //     bool valuesAreDifferent = valueComparer.Equals(oldState.Value, valueToSet) == false;
        //     bool valueWasSet        = false;
        //
        //     if (valuesAreDifferent)
        //     {
        //         IMutableState<T> newState = new MutableState<T>(this, valueToSet);
        //             
        //         while ((valueWasSet == false)  &&  valuesAreDifferent)
        //         {
        //             if (TryCompareExchangeOrSet(ref state, newState, ref oldState))
        //             {
        //                 valueWasSet = true;
        //             }
        //             else
        //             {
        //                 valuesAreDifferent = (valueComparer.Equals(oldState.Value, valueToSet) == false);
        //             }
        //         }
        //     }
        //
        //     return valueWasSet;
        // }

        //protected override bool ValuesAreDifferent(T firstValue, T secondValue) => valueComparer.Equals(firstValue, secondValue) == false;

        protected override IFactor GetFactorImplementation() => state;

        public override string ToString() => $"{Name} => {Value}";

        #endregion

        #region Operators

        public static implicit operator T(Proactive<T> proactive) => proactive.Value;

        #endregion


        #region Constructors

        public Proactive(T initialValue, IEqualityComparer<T> comparer = null, string name = null) :
            //base(initialValue, name?? NameOf<Proactive<T>>())
            base(name?? NameOf<Proactive<T>>())
        {
            state = new State<T>(this, initialValue, comparer);
        }
        
        public Proactive(T initialValue, string name) : this(initialValue, null, name)
        {
        }

        #endregion


        #region Explicit Implementations

        T IProcess<T>.Execute() => Value;

        #endregion


    }
}
