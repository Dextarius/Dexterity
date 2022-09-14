using System;
using System.Collections.Generic;
using Core.Factors;
using Core.Tools;
using JetBrains.Annotations;

namespace Factors.Cores.DirectReactorCores
{
    public class HistoricDirectFunctionResult<TInput, TOutput> : DirectResult<TOutput> 
    {
        #region Instance Fields

        [NotNull]
        private readonly Func<TInput, TInput, TOutput> valueFunction;
        private readonly IFactor<TInput>               inputSource;
        private          TInput                        lastKnownValueOfInput;

        #endregion


        #region Properties
        
        protected override IEnumerable<IFactor> Triggers         { get { yield return inputSource; } }
        public override    int                  NumberOfTriggers => 1;
        public override    int                  UpdatePriority   => inputSource.UpdatePriority + 1;

        #endregion
        
        
        #region Static Methods

        protected static string CreateNameFrom(Func<TInput, TInput, TOutput> valueDelegate, IFactor<TInput> inputSource) => 
            Delegates.CreateStringShowingArgumentBeingPassedToDelegate(inputSource, valueDelegate);

        #endregion
        
        
        #region Instance Methods

        protected override TOutput GenerateValue()
        {
            var lastKnownValue = lastKnownValueOfInput;
            var newValue       = inputSource.Value;
            
            lastKnownValueOfInput = newValue;

            return valueFunction(newValue, lastKnownValue); 
            //- Make sure the parameter order of things using old and new values stays consistent.
        }

        public override string ToString() => CreateNameFrom(valueFunction, inputSource);
        
        #endregion
        
        
        #region Constructors

        public HistoricDirectFunctionResult(IFactor<TInput>               factorToUseAsInput,
                                            Func<TInput, TInput, TOutput> functionThatDeterminesValue,
                                            IEqualityComparer<TOutput>    comparer = null)
            : base(comparer)
        {
            valueFunction = functionThatDeterminesValue?? throw new ArgumentNullException(nameof(functionThatDeterminesValue));
            inputSource   = factorToUseAsInput         ?? throw new ArgumentNullException(nameof(factorToUseAsInput));
        }

        #endregion
        
        //- TODO : See what you can consolidate between this class and the regular DirectFunctionResult<T, T>
    }
}