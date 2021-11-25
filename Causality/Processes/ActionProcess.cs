using System;
using Core.Causality;
using Core.Factors;
using JetBrains.Annotations;

namespace Causality.Processes
{
    public class ActionProcess : IProcess
    {
        #region Instance Fields

        [NotNull]
        private readonly Action actionToTake;

        #endregion
        
        
        #region Static Methods

        public static ActionProcess CreateFrom(Action action) => 
            new ActionProcess(action);

        #endregion

        
        #region Instance Methods

        public void Execute() => actionToTake();

        #endregion

        
        #region Constructors

        public ActionProcess(Action actionToExecute)
        {
            actionToTake = actionToExecute?? throw new ArgumentNullException(nameof(actionToExecute));
        }

        #endregion
    }
}