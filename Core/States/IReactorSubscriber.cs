namespace Core.States
{
    public interface IReactorSubscriber : IFactorSubscriber
    {
        bool IsTriggered { get; set; }
        bool IsUnstable  { get; set; }
        bool IsNecessary { get; set; }
    }
}