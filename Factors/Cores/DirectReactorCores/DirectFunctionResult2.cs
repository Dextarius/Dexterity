using System;
using System.Collections.Generic;
using Core.Factors;
using Core.Tools;
using JetBrains.Annotations;

namespace Factors.Cores.DirectReactorCores
{
        public class DirectFunctionResult<TInput1, TInput2, TOutput> : DirectResult<TOutput> 
    {
        #region Instance Fields

        [NotNull]
        private readonly Func<TInput1, TInput2, TOutput> valueFunction;
        private readonly IFactor<TInput1>                inputSource1;
        private readonly IFactor<TInput2>                inputSource2;
        private          int                             priority;

        #endregion


        #region Properties

        public override int NumberOfTriggers => 2;
        public override int Priority         => priority;

        protected override IEnumerable<IFactor> Triggers
        {
            get
            {
                yield return inputSource1;
                yield return inputSource2;
            }
        }

        #endregion
        
        
        #region Instance Methods

        protected override TOutput GenerateValue()
        {
            TOutput result = valueFunction(inputSource1.Value, inputSource2.Value);

            priority = Math.Max(inputSource1.Priority, inputSource2.Priority) + 1;
            //^ Set this after we generate the result, in case requesting the input values causes
            //  the input sources to update and change their priority.
            
            return result;
        }

        #endregion
        
        
        #region Constructors

        public DirectFunctionResult(Func<TInput1, TInput2, TOutput> functionThatDeterminesValue, 
                                    IFactor<TInput1>                firstInput, 
                                    IFactor<TInput2>                secondInput,
                                    IEqualityComparer<TOutput>      comparer = null,
                                    string                          name     = null)
            : base(name ?? Delegates.GetClassAndMethodName(functionThatDeterminesValue), comparer)
        {
            valueFunction = functionThatDeterminesValue??  
                            throw new ArgumentNullException(nameof(functionThatDeterminesValue));

            inputSource1 = firstInput;
            inputSource2 = secondInput;
        }
        
        public DirectFunctionResult(Func<TInput1, TInput2, TOutput> functionThatDeterminesValue, 
                                    IFactor<TInput1>                firstInput, 
                                    IFactor<TInput2>                secondInput,
                                    string                          name)
            : this(functionThatDeterminesValue,firstInput, secondInput, null, name)
        {
        }

        #endregion
    }
}