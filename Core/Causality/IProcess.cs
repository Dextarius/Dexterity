namespace Core.Causality
{
    public interface IProcess
    {
        void Execute();
    }
    
    public interface IProcess<out T>
    {
         T Execute();
    }
}