namespace Core.States
{
    public interface ITriggeredState : IFactorSubscriber
    {
        bool IsUnstable       { get; }
        bool IsReacting       { get; }
        bool IsStabilizing    { get; }
        bool HasTriggers      { get; }
        bool HasBeenTriggered { get; }
        int  NumberOfTriggers { get; }
    }
}