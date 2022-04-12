using System;
using System.Collections.Generic;
using Core.Factors;
using Core.Tools;
using JetBrains.Annotations;

namespace Factors.Cores.DirectReactorCores
{
    public class DirectFunctionResult<TInput1, TInput2, TInput3, TOutput> : DirectResult<TOutput> 
    {
        #region Instance Fields

        [NotNull]
        private readonly Func<TInput1, TInput2, TInput3, TOutput> valueFunction;
        private readonly IFactor<TInput1> inputSource1;
        private readonly IFactor<TInput2> inputSource2;
        private readonly IFactor<TInput3> inputSource3;
        private          int              priority;

        #endregion


        #region Properties

        public override int NumberOfTriggers => 3;
        public override int Priority         => priority;

        protected override IEnumerable<IXXX> Triggers
        {
            get
            {
                yield return inputSource1;
                yield return inputSource2;
                yield return inputSource3;
            }
        }

        #endregion
        
        
        #region Instance Methods

        protected override TOutput GenerateValue()
        {
            TOutput result          = valueFunction(inputSource1.Value, inputSource2.Value, inputSource3.Value);
            int     highestPriority = Math.Max(inputSource1.Priority, inputSource2.Priority);

            highestPriority = Math.Max(highestPriority, inputSource3.Priority);
            priority        = highestPriority + 1;

            return result;
        }

        #endregion
        
        
        #region Constructors

        public DirectFunctionResult(Func<TInput1, TInput2, TInput3, TOutput> functionThatDeterminesValue, 
                                    IFactor<TInput1>                         firstInput, 
                                    IFactor<TInput2>                         secondInput, 
                                    IFactor<TInput3>                         thirdInput,
                                    IEqualityComparer<TOutput>               comparer = null,
                                    string                                   name     = null)
            : base(name ?? Delegates.GetClassAndMethodName(functionThatDeterminesValue), comparer)
        {
            valueFunction = functionThatDeterminesValue??  
                            throw new ArgumentNullException(nameof(functionThatDeterminesValue));

            inputSource1 = firstInput;
            inputSource2 = secondInput;
            inputSource3 = thirdInput;
        }
        
        public DirectFunctionResult(Func<TInput1, TInput2, TInput3, TOutput> functionThatDeterminesValue, 
                                    IFactor<TInput1>                         firstInput, 
                                    IFactor<TInput2>                         secondInput, 
                                    IFactor<TInput3>                         thirdInput,
                                    string                                   name)
            : this(functionThatDeterminesValue, firstInput, secondInput, thirdInput, null, name)
        {
        }

        #endregion
    }
}