using Core.Factors;
using JetBrains.Annotations;

namespace Core.Causality
{
    public interface IOutcome : IState
    {
     // bool PotentiallyInvalid  { get; } //- TODO : Can we make this work and avoid recalculating a Reactor if the parent is still the same after they recalculate?
        bool HasCallback         { get; }
        bool IsBeingAffected     { get; }

        bool IsReflexive { get; set; }
        bool IsDirty { get; set; }

        void SetInfluences([NotNull] IState[] newInfluences);
        bool Invalidate(IState invalidParentState);
        void SetCallback(INotifiable objectToNotify);
        void DisableCallback();
        void Recalculate();
        bool MarkDirty(int currentDepth);

        bool MarkUnstable(int currentDepth);
        void NotifyParentChanged(IState outcome, long newVersionNumber, int currentDepth);
        bool NotifyParentUnstable(IState outcome, long unstableVersionNumber);
    }
    
    public interface IOutcome<T> : IOutcome
    {
        T Value { get; set; }

        T Peek();
    }
}