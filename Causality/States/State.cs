using System;
using System.Collections.Immutable;
using Core.Causality;
using Core.Factors;
using JetBrains.Annotations;
using static Core.InterlockedUtils;

namespace Causality.States
{
    //- TODO : We need to do some research and find out for sure if we need to be using Volatile.Read() or using 
    //         memory barriers to make sure each thread always reads the correct vale for all of these
    //         fields we're setting using Interlocked methods.  Joe Duffy is the expert in this area, so read his blog
    //         or we also have a copy of his book on concurrency.
    public class State : IState
    {
        #region Constants

        protected const int False   = 0b0000_0000;
        protected const int Invalid = 0b0000_0001;
        
        #endregion
        

        #region Instance Fields

        [NotNull, ItemNotNull]
        protected ImmutableHashSet<IOutcome> affectedOutcomes = ImmutableHashSet<IOutcome>.Empty;
        protected int status;

        #endregion


        #region Properties

        public bool IsInvalid       => (status & Invalid) == Invalid;
        public bool IsValid         => IsInvalid == false;
        public bool IsConsequential => (affectedOutcomes != ImmutableHashSet<IOutcome>.Empty) && IsValid;
        //^ Make sure to check the conditions in this order, if we check in the opposite order
        //  we might return the wrong result if a process marks this as invalid right after we check, but doesn't 
        //  clear affectedResults before we check it.

        #endregion


        #region Instance Methods

        public void NotifyInvolved() => Observer.NotifyInvolved(this);
        
        public virtual bool Invalidate()
        {
            int formerState = status;
            
            while ((formerState & Invalid)  ==  False)
            {
                if (TryCompareExchangeOrSet(ref status, (formerState | Invalid), ref formerState))
                {
                    InvalidateDependents();

                    return true;
                }
            }

            return false;
        }

        public void InvalidateDependents()
        {
            var formerDependents = affectedOutcomes;

            if (TryExchangeUntilSetIsEmpty(ref affectedOutcomes))
            {
                foreach (var factor in formerDependents)
                {
                    factor.Invalidate(this);
                }
            }
        }

        public bool AddDependent(IOutcome dependentToAdd)
        {
            if(dependentToAdd == null) { throw new ArgumentNullException(nameof(dependentToAdd)); }

            var oldFactorSet = affectedOutcomes;
            
            while (oldFactorSet.Contains(dependentToAdd) == false)
            {
                if (IsValid)
                {
                    var newFactorSet = oldFactorSet.Add(dependentToAdd);

                    //- TODO : Test this and make sure if this is invalidated the dependent still gets a notification.
                    if (TryCompareExchange(ref affectedOutcomes, newFactorSet, oldFactorSet, out oldFactorSet))
                    {
                        return true;
                    }
                }
                else
                {
                    //- TODO : This code is replicated in section where the CasualEvent tries to add dependencies.
                    dependentToAdd.Invalidate(this);  
                    return false;
                }
            }
            
            return false;
        }

        public void ReleaseDependent(IOutcome dependentToRelease)
        {
            if (dependentToRelease != null)
            {
                RemoveAndExchangeUntilSuccessful(ref affectedOutcomes, dependentToRelease);
            }
        }

        #endregion
    }

    public class State<T> : State, IState<T>
    {
        protected readonly T value;
        
        public virtual T Value 
        {
            get
            {
                Observer.NotifyInvolved(this);
                return value;
            }
        }

        public State(T value)
        {
            this.value = value;
        }
    }
}
