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
        public override int Priority         => inputSource.Priority + 1;

        protected override IEnumerable<IXXX> Triggers
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
        //- TODO : Decide if we want to make one of these for each of the Direct FunctionResult classes.

        #endregion
        
        
        #region Instance Methods

        protected override TOutput GenerateValue() => valueFunction(inputSource.Value);
        
        #endregion
        
        
        #region Constructors

        public DirectFunctionResult(Func<TInput, TOutput>      functionThatDeterminesValue, 
                                    IFactor<TInput>            factorToUseAsInput,
                                    string                     name     = null,
                                    IEqualityComparer<TOutput> comparer = null)
            : base(name ?? CreateNameFrom(functionThatDeterminesValue, factorToUseAsInput), comparer)
        {
            valueFunction = functionThatDeterminesValue?? throw new ArgumentNullException(nameof(functionThatDeterminesValue));
            inputSource   = factorToUseAsInput         ?? throw new ArgumentNullException(nameof(factorToUseAsInput));

        }
        
        public DirectFunctionResult(Func<TInput, TOutput>      functionThatDeterminesValue, 
                                    IFactor<TInput>            inputSource,
                                    IEqualityComparer<TOutput> comparer)
            : this(functionThatDeterminesValue,inputSource, null, comparer)
        {
        }

        #endregion
    }
    
    
    
    public class DirectModifiableFunctionResult<TInput, TOutput> : DirectResult<TOutput> 
    {
        #region Instance Fields

        [NotNull]
        private readonly Func<TInput, TOutput>          valueFunction;
        private readonly IFactor<TInput>                inputSource;
        private readonly List<IFactorModifier<TOutput>> modifiers;

        #endregion


        #region Properties

        public override int NumberOfTriggers => 1;
        public override int Priority         => inputSource.Priority + 1;

        protected override IEnumerable<IXXX> Triggers
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
        //- TODO : Decide if we want to make one of these for each of the Direct FunctionResult classes.

        #endregion
        
        
        #region Instance Methods

        protected override TOutput GenerateValue() => valueFunction(inputSource.Value);
        
        #endregion
        
        
        #region Constructors

        public DirectFunctionResult(Func<TInput, TOutput>      functionThatDeterminesValue, 
                                    IFactor<TInput>            factorToUseAsInput,
                                    string                     name     = null,
                                    IEqualityComparer<TOutput> comparer = null)
            : base(name ?? CreateNameFrom(functionThatDeterminesValue, factorToUseAsInput), comparer)
        {
            valueFunction = functionThatDeterminesValue?? throw new ArgumentNullException(nameof(functionThatDeterminesValue));
            inputSource   = factorToUseAsInput         ?? throw new ArgumentNullException(nameof(factorToUseAsInput));

        }
        
        public DirectFunctionResult(Func<TInput, TOutput>      functionThatDeterminesValue, 
                                    IFactor<TInput>            inputSource,
                                    IEqualityComparer<TOutput> comparer)
            : this(functionThatDeterminesValue,inputSource, null, comparer)
        {
        }

        #endregion
    }
}