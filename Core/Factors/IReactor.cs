using Core.States;

namespace Core.Factors
{
    public interface IReactor : IFactor, IFactorSubscriber, ITriggeredState
    {
        bool IsReacting           { get; }
        bool IsStabilizing        { get; }
        bool HasTriggers          { get; }
        int  NumberOfTriggers     { get; }
        bool HasReacted           { get; }
        uint NumberOfTimesReacted { get; }
        bool IsReflexive          { get; set; }


        bool AttemptReaction();
        bool ForceReaction();
    }
}