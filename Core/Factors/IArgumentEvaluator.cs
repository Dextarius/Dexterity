namespace Core.Factors
{
    public interface IArgumentEvaluator<TParam, TReturn>
    {
        TReturn GetResultFor(TParam argumentValue);
    }
}