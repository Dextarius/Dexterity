using System;
using System.Collections.Generic;
using Core.Factors;
using Core.Tools;
using JetBrains.Annotations;

namespace Factors.Cores.DirectReactorCores
{
    public class HistoricDirectActionResponse<TInput> : HistoricDirectReactor<TInput>
    {
        #region Instance Fields

        [NotNull]
        private readonly Action<TInput, TInput> actionToTake;

        #endregion


        #region Properties
        
        public override int NumberOfTriggers => 1;
        public override int UpdatePriority   => inputSource.UpdatePriority + 1;
        
        protected override IEnumerable<IFactor> Triggers { get { yield return inputSource; } }

        #endregion
        
        
        #region Static Methods

        protected static string CreateNameFrom(Action<TInput, TInput> valueDelegate, IFactor<TInput> inputSource) => 
            Delegates.CreateStringShowingArgumentBeingPassedToDelegate(inputSource, valueDelegate);

        #endregion
        
        
        #region Instance Methods

        protected override long CreateOutcome()
        {
            var lastKnownValue = lastKnownValueOfInput;
            var newValue       = inputSource.Value;

            lastKnownValueOfInput = newValue;
            actionToTake(newValue, lastKnownValue);

            return TriggerFlags.Default;
            //- Make sure the parameter order of things using old and new values stays consistent.
        }

        public override string ToString() => CreateNameFrom(actionToTake, inputSource);
        
        #endregion
        
        
        #region Constructors

        public HistoricDirectActionResponse(IFactor<TInput>        factorToUseAsInput,
                                            Action<TInput, TInput> functionThatDeterminesValue)
            : base(factorToUseAsInput)
        {
            actionToTake = functionThatDeterminesValue?? throw new ArgumentNullException(nameof(functionThatDeterminesValue));
        }

        #endregion
        
        //- TODO : See what you can consolidate between this class and the regular DirectFunctionResult<T, T>
    }
}