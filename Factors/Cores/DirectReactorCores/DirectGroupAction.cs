using System;
using System.Collections.Generic;
using Core.Factors;
using Core.Tools;
using JetBrains.Annotations;

namespace Factors.Cores.DirectReactorCores
{
    public class DirectGroupAction<TArg> : DirectReactorCore
    {
        #region Instance Fields

        [NotNull]
        private readonly Action<TArg>           responseAction;
        private readonly HashSet<IFactor<TArg>> inputs = new HashSet<IFactor<TArg>>();

        #endregion
        

        #region Properties

        public    override    int               NumberOfTriggers => inputs.Count;
   //   public    override    int               UpdatePriority   => inputs.UpdatePriority + 1;
        protected override IEnumerable<IFactor> Triggers         => inputs;

        #endregion

        
        #region Instance Methods

        protected override long CreateOutcome()
        {
           // responseAction(inputs.Value);
            SubscribeToInputs();

            return TriggerFlags.Default;
        }

        public override string ToString() => Delegates.GetClassAndMethodName(responseAction);

        #endregion


        #region Constructors

        public DirectGroupAction(Action<TArg> actionToTake) 
        {
            responseAction = actionToTake;
        }

        #endregion
    }
}