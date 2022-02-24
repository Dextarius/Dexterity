using System;
using Core.Tools;
using JetBrains.Annotations;

namespace Factors.Cores.ObservedReactorCores
{
    public class ObservedActionResponse : ObservedResponse
    {
        #region Instance Fields

        [NotNull]
        private readonly Action actionToTake;

        #endregion
        
        
        #region Static Methods

        public static ObservedActionResponse CreateFrom(Action action) => new ObservedActionResponse(action);

        #endregion

        
        #region Instance Methods

        protected override void ExecuteResponse() => actionToTake();

        #endregion

        
        #region Constructors

        public ObservedActionResponse(Action actionToExecute, string name = null) : 
            base(name ?? Delegates.GetClassAndMethodName(actionToExecute))
        {
            actionToTake = actionToExecute??  throw new ArgumentNullException(nameof(actionToExecute));
        }

        #endregion
    }
}