using Core.Factors;

namespace Factors.Outcomes
{
    //- TODO : We could use this on collection Proactives/Reactives for methods which return a value that may or may not
    //         be used (such as the bool return value on the Add() method of a ProactiveSet).  We could make one of these
    //         the primary return value so that we'd be able to tell if the value was actually used/retrieved.  We would \
    //         likely need to explicitly implement all of the normal collection interfaces (IList, ISet) to redirect to 
    //         the new methods because I doubt they'll accept an implicitly convertible return value as an appropriate
    //         implementation of the interface's methods.

    public readonly struct PotentialInfluence<T>
    {
        #region Instance Fields

        private readonly T         value;
        private readonly IInvolved owner;

        #endregion


        #region Operators

        public static implicit operator T(PotentialInfluence<T> influence)
        {
            influence.owner.NotifyInvolved();
            return influence.value;
        }

        #endregion


        #region Constructors
        
        public PotentialInfluence(T value, IInvolved owner)
        {
            this.value = value;
            this.owner = owner;
        }

        #endregion
    }
}