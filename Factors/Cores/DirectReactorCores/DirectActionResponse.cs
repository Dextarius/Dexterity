using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Core.Factors;
using Core.Tools;

namespace Factors.Cores.DirectReactorCores
{
    public class DirectActionResponse<TArg> : DirectReactorCore
    {
        #region Instance Fields

        [NotNull]
        private readonly Action<TArg>  responseAction;
        private readonly IFactor<TArg> inputSource;

        #endregion
        

        #region Properties

        public override int NumberOfTriggers => 1;
        public override int Priority         => inputSource.Priority + 1;

        protected override IEnumerable<IFactor> Triggers
        {
            get
            {
                yield return inputSource;
            }
        }

        #endregion

        
        #region Instance Methods

        protected override bool GenerateOutcome()
        {
            responseAction(inputSource.Value);
            SubscribeToInputs();

            return true;
        }


        #endregion


        #region Constructors

        public DirectActionResponse(Action<TArg> actionToTake, IFactor<TArg> inputArgSource, string name = null) : 
            base(name ?? Delegates.GetClassAndMethodName(actionToTake))
        {
            responseAction = actionToTake;
            inputSource    = inputArgSource;
        }

        #endregion
    }
    
    
}