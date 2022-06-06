using System;
using System.Collections.Generic;
using Core.Factors;
using Core.Tools;
using JetBrains.Annotations;

namespace Factors.Cores.DirectReactorCores
{
    public class DirectFunctionResult<TInput, TOutput> : DirectResult<TOutput> 
    {
        #region Instance Fields

        [NotNull]
        private readonly Func<TInput, TOutput> valueFunction;
        private readonly IFactor<TInput>       inputSource;

        #endregion


        #region Properties
        
        public override int NumberOfTriggers => 1;
        public override int UpdatePriority   => inputSource.UpdatePriority + 1;
        
        protected override IEnumerable<IFactor> Triggers
        {
            get
            {
                yield return inputSource;
            }
        }

        #endregion
        
        
        #region Static Methods

        protected static string CreateNameFrom(Func<TInput, TOutput> valueDelegate, IFactor<TInput> inputSource) => 
            Delegates.CreateStringShowingArgumentBeingPassedToDelegate(inputSource, valueDelegate);

        #endregion
        
        
        #region Instance Methods

        protected override TOutput GenerateValue() => valueFunction(inputSource.Value);
        public    override string  ToString()      => CreateNameFrom(valueFunction, inputSource);

        
        #endregion
        
        
        #region Constructors

        public DirectFunctionResult(Func<TInput, TOutput>      functionThatDeterminesValue,
                                    IFactor<TInput>            factorToUseAsInput,
                                    IEqualityComparer<TOutput> comparer = null)
            : base(comparer)
        {
            valueFunction = functionThatDeterminesValue?? throw new ArgumentNullException(nameof(functionThatDeterminesValue));
            inputSource   = factorToUseAsInput         ?? throw new ArgumentNullException(nameof(factorToUseAsInput));

        }

        #endregion
    }

}