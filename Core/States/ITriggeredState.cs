namespace Core.States
{
    public interface ITriggeredState : IFactorSubscriber
    {
        
        bool IsUnstable          { get; }
        bool IsReacting          { get; }
        bool IsStabilizing       { get; }
        bool HasReacted          { get; }
        bool HasTriggers         { get; }
        bool IsTriggered         { get; }
        int  NumberOfTriggers    { get; }
        bool IsReflexive         { get; set; }
        bool AutomaticallyReacts { get; set; }
        
        bool AttemptReaction();
        bool ForceReaction();
    }
}