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
        public override int UpdatePriority   => inputSource.UpdatePriority + 1;

        protected override IEnumerable<IFactor> Triggers
        {
            get
            {
                yield return inputSource;
            }
        }

        #endregion

        
        #region Instance Methods

        protected override bool CreateOutcome()
        {
            responseAction(inputSource.Value);
            SubscribeToInputs();

            return true;
        }

        public override string ToString() => Delegates.GetClassAndMethodName(responseAction);

        #endregion


        #region Constructors

        public DirectActionResponse(Action<TArg> actionToTake, IFactor<TArg> inputArgSource) 
        {
            responseAction = actionToTake;
            inputSource    = inputArgSource;
        }

        #endregion
    }
    
    
    public class DirectActionResponse : DirectReactorCore
    {
        #region Instance Fields

        [NotNull]
        private readonly Action  responseAction;
        private readonly IFactor inputSource;

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

        
        #region Instance Methods

        protected override bool CreateOutcome()
        {
            responseAction();
            SubscribeToInputs();

            return true;
        }

        public override string ToString() => Delegates.GetClassAndMethodName(responseAction);

        #endregion


        #region Constructors

        public DirectActionResponse(Action actionToTake, IFactor inputArgSource) 
        {
            responseAction = actionToTake;
            inputSource    = inputArgSource;
        }

        #endregion
    }
}