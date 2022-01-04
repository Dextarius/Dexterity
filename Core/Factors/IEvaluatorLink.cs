namespace Core.Factors
{
    public interface IEvaluatorLink<TParam, TReturn>
    {
        TReturn            GetResultFor(TParam argumentValue);
        void       RegisterForUpdatesTo(TParam argumentValue);
        void    UnregisterFromUpdatesOf(TParam argumentValue);
        
        
    }
}