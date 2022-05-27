using System.Collections.Generic;
using Core.Causality;
using Core.Factors;
using Core.States;

namespace Factors.Cores.ObservedReactorCores
{
    public abstract class ObservedResult<T> : ObservedReactorCore, IResult<T>, IProcess<T>
    {
        #region Instance Fields

        protected readonly IEqualityComparer<T> valueComparer;
        protected          T                    currentValue;

        #endregion

        
        #region Properties

        public T Value
        {
            get
            {
                NotifyInvolved();
                return currentValue;
            }
        }

        #endregion

        
        #region Instance Methods

        protected override bool CreateOutcome()
        {
            T oldValue = currentValue;
            T newValue = Observer.ObserveInteractions<ObservedResult<T>, T>(this);
            
            using (Observer.PauseObservation()) //- Prevents us from adding dependencies to any other observations this
            {                                   //  one might be nested inside of.     
                RemoveUnusedTriggers();

                if (valueComparer.Equals(oldValue, newValue))
                {
                    return false;
                }
                else
                {
                    currentValue = newValue;
                    return true;
                }
            }
        }
        
        public bool ValueEquals(T valueToCompare) => valueComparer.Equals(currentValue, valueToCompare);

        public T Peek() => currentValue;

        protected abstract T GenerateValue();

        //- Should we support a set of events that notifies subscribers that the value is about to change,
        //  so they can alter/stop it?
        //protected virtual void OnValueChanging(ValueChangedEventArgs e)

        #endregion


        #region Constructors

        protected ObservedResult(IEqualityComparer<T> comparer = null) : base()
        {
            valueComparer = comparer ?? EqualityComparer<T>.Default;
        }

        #endregion

        
        #region Explicit Implementations

        T IProcess<T>.Execute() => GenerateValue();

        #endregion
    }
}