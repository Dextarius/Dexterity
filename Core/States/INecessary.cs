namespace Core.States
{
    public interface INecessary
    {
        bool IsNecessary { get; }
        
        void NotifyNecessary();
        void NotifyNotNecessary();
    }
}