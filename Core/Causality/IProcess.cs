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
    
    public interface IProcess<out TResult, in TArg>
    {
        TResult Execute(TArg argument);
    }
}