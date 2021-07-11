using Causality;
using Causality.States;
using Core.Causality;
using Core.Factors;
using JetBrains.Annotations;
using static Core.InterlockedUtils;

namespace Factors
{
    public abstract class ProactiveValue<TValue> : Proactor
    {
        #region Instance Fields

        protected IState<TValue> state;

        #endregion

        
        #region Properties
        protected override IState State => state;

        #endregion

        
        #region Instance Methods
        
        //- Note : These methods that swap out the state after a change should probably store the state they are using
        //         in a variable, instead of repeatedly grabbing it from the field, in case the field gets changed 
        //         somehow in the middle of an operation.
        protected void SetValue(TValue newValue)
        {
            IState<TValue> oldState = state;

            if (TrySetValue(newValue, oldState))
            {
                oldState.Invalidate();
                Observer.NotifyChanged(state);
            }
        }
        
        //- TODO: Consider if we'll need to create a way to tell which update came before another, so that if
        //        a Proactive's value is set twice and there's a race, the latest value won't be replaced because
        //        the thread setting the first value is running slow.
        private bool TrySetValue(TValue valueToSet, [NotNull] IState<TValue> oldState)
        {
            bool valueWasSet = false;

            if (ValuesAreDifferent(oldState.Value, valueToSet))
            {
                IState<TValue> newState = new State<TValue>(valueToSet);

                do
                {
                    if (TryCompareExchangeOrSet(ref state, newState, ref oldState))
                    {
                        valueWasSet = true;
                    }
                } 
                while ((valueWasSet == false)  &&  ValuesAreDifferent(oldState.Value, valueToSet));
            }

            return valueWasSet;
        }

        //- TODO : This is mostly used by the regular Proactive not the collections, so we may want to move it to
        //         the Proactive<T> class.
        protected abstract bool ValuesAreDifferent(TValue firstValue, TValue secondValue);

        #endregion

        
        #region Constructors
        
        protected ProactiveValue(TValue initialValue, string name) : base(name)
        {
            state = new State<TValue>(initialValue);
        }

        #endregion
    }
}