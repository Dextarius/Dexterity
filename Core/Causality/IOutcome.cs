using Core.Factors;
using JetBrains.Annotations;

namespace Core.Causality
{
    public interface IOutcome : IState
    {
        bool HasCallback     { get; }
        bool IsBeingAffected { get; }

        void SetInfluences([NotNull] IState[] newInfluences);
        bool Invalidate(IState invalidParentState);
        void SetCallback(INotifiable objectToNotify);
        void DisableCallback();
    }
    
    public interface IOutcome<T> : IOutcome
    {
        T Value { get; set; }
    }
}