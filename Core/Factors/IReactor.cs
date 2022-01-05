using Core.States;

namespace Core.Factors
{
    public interface IReactor : IFactor, IFactorSubscriber
    {
        bool IsReflexive      { get; set; }
        bool HasTriggers      { get; }
        int  NumberOfTriggers { get; }
        bool HasBeenTriggered { get; }
        bool IsReacting       { get; }
        bool IsUnstable       { get; }
        bool IsStabilizing    { get; }


        bool AttemptReaction();
        bool ForceReaction();
    }
}