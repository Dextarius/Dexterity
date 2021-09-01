using Core.Factors;
using JetBrains.Annotations;

namespace Core.Causality
{
    public interface IOutcome : IState
    {
     // bool PotentiallyInvalid  { get; } //- TODO : Can we make this work and avoid recalculating a Reactor if the parent is still the same after they recalculate?
        bool HasCallback         { get; }
        bool IsBeingAffected     { get; }

        void SetInfluences([NotNull] IState[] newInfluences);
        bool Invalidate(IState invalidParentState);
        void SetCallback(INotifiable objectToNotify);
        void DisableCallback();
    }
    
    public interface IOutcome<T> : IOutcome
    {
        T Value { get; set; }

        T Peek();
    }
}