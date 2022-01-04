namespace Core.Factors
{
    public interface IEvaluator<TParam, TReturn>
    {
        TReturn Evaluate(TParam argumentValue);
        IEvaluatorLink<TParam, TReturn> RequestLinkToEvaluator();
    }
}