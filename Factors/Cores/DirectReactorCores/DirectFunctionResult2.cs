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
        public override int UpdatePriority   => priority;

        protected override IEnumerable<IFactor> Triggers
        {
            get
            {
                yield return inputSource1;
                yield return inputSource2;
            }
        }

        #endregion

        
        #region Static Methods

        protected static string CreateNameFrom(Func<TInput1, TInput2, TOutput> valueDelegate, 
                                               IFactor<TInput1> input1, 
                                               IFactor<TInput2> input2) => 
                Delegates.CreateStringShowingArgumentBeingPassedToDelegate(input1, input2, valueDelegate);

        #endregion
        
        
        #region Instance Methods

        protected override TOutput GenerateValue()
        {
            TOutput result = valueFunction(inputSource1.Value, inputSource2.Value);

            priority = Math.Max(inputSource1.UpdatePriority, inputSource2.UpdatePriority) + 1;
            //^ Set this after we generate the result, in case requesting the input values causes
            //  the input sources to update and change their priority.
            
            return result;
        }
        
        public override string ToString() => CreateNameFrom(valueFunction, inputSource1, inputSource2);
        
        #endregion
        
        
        #region Constructors

        public DirectFunctionResult(Func<TInput1, TInput2, TOutput> functionThatDeterminesValue,
                                    IFactor<TInput1>                firstInput,
                                    IFactor<TInput2>                secondInput,
                                    IEqualityComparer<TOutput>      comparer = null)
            : base(comparer)
        {
            inputSource1  = firstInput;
            inputSource2  = secondInput;
            valueFunction = functionThatDeterminesValue ?? throw new ArgumentNullException(nameof(functionThatDeterminesValue));
        }

        #endregion
    }
}