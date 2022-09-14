using System;
using System.Collections.Generic;
using Core.Factors;
using Core.States;
using Core.Tools;
using JetBrains.Annotations;

namespace Factors.Cores.DirectReactorCores
{
    public class SetValueResponse<TValue> : DirectReactorCore
    {
        #region Instance Fields

        [NotNull]
        private readonly Action<TValue>     responseAction;
        private readonly IProactive<TValue> factorToSetValueOf;
        private readonly TValue             valueToSet;

        #endregion
        

        #region Properties

        protected override IEnumerable<IFactor> Triggers         { get { yield return factorToSetValueOf; } }
        public    override int                  NumberOfTriggers => 1;
        public    override int                  UpdatePriority   => factorToSetValueOf.UpdatePriority + 1;

        #endregion

        
        #region Instance Methods

        protected override long CreateOutcome()
        {
            responseAction(factorToSetValueOf.Value);
            SubscribeToInputs();

            return TriggerFlags.Default;
        }

        public override string ToString() => Delegates.GetClassAndMethodName(responseAction);

        #endregion


        #region Constructors

        public SetValueResponse(Action<TValue>     actionToTake, 
                                IProactive<TValue> inputArgSource, 
                                bool               useWeakSubscriber = true) : 
            base(useWeakSubscriber)
        {
            responseAction = actionToTake;
            factorToSetValueOf    = inputArgSource;
        }

        #endregion
    }
}