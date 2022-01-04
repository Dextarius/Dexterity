namespace Core.Causality
{
    public interface IQueue
    {
        void StartQueuing();
        void StopQueuing();
    }
}