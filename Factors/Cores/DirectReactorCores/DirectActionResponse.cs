using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Core.Factors;
using Core.States;
using Core.Tools;
using JetBrains.Annotations;

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

        protected override IEnumerable<IFactor> Triggers         { get { yield return inputSource; } }
        public    override int                  NumberOfTriggers => 1;
        public    override int                  UpdatePriority   => inputSource.UpdatePriority + 1;

        #endregion

        
        #region Instance Methods

        protected override long CreateOutcome()
        {
            responseAction(inputSource.Value);
            SubscribeToInputs();

            return TriggerFlags.Default;
        }

        public override string ToString() => Delegates.GetClassAndMethodName(responseAction);

        #endregion


        #region Constructors

        public DirectActionResponse(Action<TArg>  actionToTake, 
                                    IFactor<TArg> inputArgSource, 
                                    bool          useWeakSubscriber = true) : 
            base(useWeakSubscriber)
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
        private readonly IFactor trigger;

        #endregion
        

        #region Properties

        public override int NumberOfTriggers => 1;
        public override int UpdatePriority   => trigger.UpdatePriority + 1;

        protected override IEnumerable<IFactor> Triggers { get { yield return trigger; } }

        #endregion

        
        #region Instance Methods

        protected override long CreateOutcome()
        {
            responseAction();
            SubscribeToInputs();

            return TriggerFlags.Default;
        }

        public override string ToString() => Delegates.GetClassAndMethodName(responseAction);

        #endregion


        #region Constructors

        public DirectActionResponse(IFactor inputArgSource, 
                                    Action  actionToTake, 
                                    bool    useWeakSubscriber = true) : 
            base(useWeakSubscriber)
        {
            responseAction = actionToTake;
            trigger        = inputArgSource;
        }

        #endregion
    }
}