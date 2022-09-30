using System;
using System.Collections.Generic;
using Core.Factors;
using Core.Tools;
using JetBrains.Annotations;

namespace Factors.Cores.DirectReactorCores
{
    public class DirectActionResponse<TArg1, TArg2> : DirectReactorCore
    {
        #region Instance Fields

        [NotNull]
        private readonly Action<TArg1, TArg2> responseAction;
        private readonly IFactor<TArg1>       inputSource1;
        private readonly IFactor<TArg2>       inputSource2;
        private          int                  priority;

        #endregion
        

        #region Properties

        public override int NumberOfTriggers => 2;
        public override int UpdatePriority   => priority;

        protected override IEnumerable<IFactor> Triggers { get { yield return inputSource1; } }

        #endregion

        
        #region Instance Methods

        protected override long CreateOutcome()
        {
            responseAction(inputSource1.Value, inputSource2.Value);
            SubscribeToInputs();
            priority = Math.Max(inputSource1.UpdatePriority, inputSource2.UpdatePriority);

            return TriggerFlags.Default;
        }
        
        public override string ToString() => Delegates.GetClassAndMethodName(responseAction);

        #endregion


        #region Constructors

        public DirectActionResponse(Action<TArg1, TArg2> actionToTake,
                                    IFactor<TArg1>       firstInput,
                                    IFactor<TArg2>       secondInput)
        {
            inputSource1   = firstInput;
            inputSource2   = secondInput;
            responseAction = actionToTake ?? throw new ArgumentNullException(nameof(actionToTake));
        }

        #endregion
    }
}