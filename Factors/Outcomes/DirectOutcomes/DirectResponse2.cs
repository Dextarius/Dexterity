using System;
using System.Collections.Generic;
using Core.Factors;
using Core.States;
using Core.Tools;
using JetBrains.Annotations;

namespace Factors.Outcomes.DirectOutcomes
{
    public class DirectActionResponse<TArg1, TArg2> : DirectOutcome
    {
        #region Instance Fields

        [NotNull]
        private readonly Action<TArg1, TArg2> responseAction;
        private readonly IFactor<TArg1>       inputSource1;
        private readonly IFactor<TArg2>       inputSource2;
        private          int                  priority;

        #endregion
        

        #region Properties

        public override int NumberOfInfluences => 1;
        public override int Priority           => priority;
        
        public override IEnumerable<IFactor> Inputs
        {
            get
            {
                yield return inputSource1;
            }
        }

        #endregion

        
        #region Instance Methods

        protected override bool GenerateOutcome()
        {
            responseAction(inputSource1.Value, inputSource2.Value);
            AddSelfAsDependentToInputs();
            priority = Math.Max(inputSource1.Priority, inputSource2.Priority);

            return true;
        }

        #endregion


        #region Constructors

        public DirectActionResponse(Action<TArg1, TArg2> actionToTake, 
                                    IFactor<TArg1>       firstInput,
                                    IFactor<TArg2>       secondInput,
                                    string               name = null) : 
            base(name ?? Delegates.GetClassAndMethodName(actionToTake))
        {
            inputSource1   = firstInput;
            inputSource2   = secondInput;
            responseAction = actionToTake??  
                             throw new ArgumentNullException(nameof(actionToTake));
        }

        #endregion
    }
}