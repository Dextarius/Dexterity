namespace Core.States
{
    public interface IPrioritizedUpdate
    {
        int UpdatePriority { get; }
    }
}