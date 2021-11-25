using System;
using System.Collections.Generic;
using Core.Causality;
using Core.Factors;
using Core.States;


namespace Causality.States
{
    public class Result<T> : Result, IResult<T>
    {
        #region Instance Fields

        protected readonly IEqualityComparer<T> valueComparer;
        protected readonly IProcess<T>          valueProcess;
        protected          T                    currentValue;

        #endregion


        #region Instance Properties

        public T Value
        {
            get
            {
                Reconcile();
                Observer.NotifyInvolved(this);
                return currentValue;
            }
        }

        #endregion


        #region Instance Methods

        public T Peek() => currentValue;

        protected override bool ExecuteProcess()
        {
            T oldValue = currentValue;
            T newValue = Observer.ObserveInteractions(valueProcess, this);

            using (Observer.PauseObservation()) //- To prevent us from adding dependencies to any other observations this one might be nested inside of. 
            {
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
        
        //- TODO : Watching the video on Incremental made me think about the idea of making a 'map' function
        //         for cases where the value of your Outcome is always determined using a specific State/Factor,
        //         so you don't need a complicated system to track its parents, just something where every time 
        //         that State changes values, the Outcome passes the new value to its Process and sets its Value to
        //         the result of that Process.  Or we could see about attaching it to the Process directly, but
        //         not every value that comes from a process is used (such as values that are equal to the old value).

        // public static Outcome<T2> Map<T1, T2>(State<T1> valueSource, Func<T2, T1> process)
        // {
        //
        // }

        // public static Outcome<T3> Map2<T1, T2, T3>(State<T1> valueSource1, State<T2> valueSource2, Func<T3, T1, T2> process)
        // {
        //     
        // }

        //- I'm trying to understand the relationship between my Reactives and the 'Bind' method in the video.
        //  It seems like the biggest difference is that they don't have 'discovery'.  With their system you
        //  have to know all of the States/Factors ahead of time and feed them into the constructors.
        // public static Outcome<Outcome<T2>> Bind<T1, T2>(State<T1> valueSource, Func<Outcome<T2>, T1> process)
        // {
        //     
        // }
        
        
        //- Could we support a set of events that notifies subscribers that the value is about to change,
        //  so they can change/stop it?
        //protected virtual void OnValueChanging(ValueChangedEventArgs e)
        

        #endregion


        #region Constructors
        
        public Result(object owner, IProcess<T> processToDetermineValue, IEqualityComparer<T> comparer = null): base(owner)
        {
            valueProcess  = processToDetermineValue ?? throw new ArgumentNullException(nameof(processToDetermineValue));
            valueComparer = comparer ?? EqualityComparer<T>.Default;
        }
        
        public Result(IProcess<T> processToDetermineValue, IEqualityComparer<T> comparer = null): 
            this(null, processToDetermineValue, comparer)
        {
        }

        #endregion


    }
}

