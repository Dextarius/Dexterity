namespace Subscriptions
{
    public interface ISubscriber
    {
        void Execute();
    }
    
    public interface ISubscriber<in T>
    {
        void Execute(T newValue);
    }
    
    public interface ISubscriber<in T1, T2>
    {
        void Execute(T1 newValue, T2 oldValue);
    }
}