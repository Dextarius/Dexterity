namespace Core.States
{
    public interface ITriggeredState
    {
        bool HasBeenTriggered { get; }
        bool IsUnstable       { get; }
    }
}