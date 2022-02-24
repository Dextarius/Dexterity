using System;
using Core.Factors;

namespace Factors.Cores
{
    public class ArgumentEvaluator<TParam, TReturn> : IArgumentEvaluator<TParam, TReturn>
    {
        private readonly Func<TParam, TReturn> evaluationFunction;

        public TReturn GetResultFor(TParam argumentValue) => evaluationFunction.Invoke(argumentValue);

        public ArgumentEvaluator(Func<TParam, TReturn> functionToGetResultsFrom)
        {
            evaluationFunction = functionToGetResultsFrom ??
                             throw new ArgumentNullException(nameof(functionToGetResultsFrom), 
                                                     $"A {nameof(ArgumentEvaluator<TParam, TReturn>)} cannot be constructed " +
                                                             "with a null delegate, as it would would never produce any results. ");
        }
    }
}
