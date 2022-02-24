using System;
using System.Collections.Generic;
using Core.Factors;
using Core.States;
using Factors.Cores.DirectReactorCores;
using Factors.Cores.ObservedReactorCores.CollectionResults;

namespace Factors.Cores
{
    public static class Result
    {
        public static IResult<TOutput> CreateFrom<TInput, TOutput>(Func<TInput, TOutput> valueFunction, 
                                                                   IFactor<TInput>       input) => 
            new DirectFunctionResult<TInput,TOutput>(valueFunction, input);
        
        public static IResult<TOutput> CreateFrom<TInput1, TInput2, TOutput>(Func<TInput1, TInput2, TOutput> valueFunction, 
                                                                             IFactor<TInput1>                firstInput, 
                                                                             IFactor<TInput2>                secondInput) => 
            new DirectFunctionResult<TInput1, TInput2, TOutput>(valueFunction, firstInput, secondInput);
        
        public static IResult<TOutput> CreateFrom<TInput1, TInput2, TInput3, TOutput>(
            Func<TInput1, TInput2, TInput3, TOutput> valueFunction, 
            IFactor<TInput1>                         firstInput, 
            IFactor<TInput2>                         secondInput, 
            IFactor<TInput3>                         thirdInput) => 
                new DirectFunctionResult<TInput1, TInput2, TInput3, TOutput>(
                    valueFunction, firstInput, secondInput, thirdInput);
    }
    
    
    public static class ListResult 
    {
        public static ObservedListResult<T> CreateFrom<T>(Func<IEnumerable<T>> functionToGenerateElements,
                                                          IEqualityComparer<T> comparerForElements = null)
        {
            if (functionToGenerateElements is null) { throw new ArgumentNullException(nameof(functionToGenerateElements)); }

            return new ObservedListFunctionResult<T>(functionToGenerateElements, comparerForElements);
        }
    }
    
    
    public static class HashSetResult 
    {
        public static ObservedHashSetResult<T> CreateFrom<T>(Func<IEnumerable<T>> functionToGenerateElements,
                                                             IEqualityComparer<T> comparerForElements = null)
        {
            if (functionToGenerateElements is null) { throw new ArgumentNullException(nameof(functionToGenerateElements)); }

            return new ObservedHashSetFunctionResult<T>(functionToGenerateElements, comparerForElements);
        }
    }
}