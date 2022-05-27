using System;
using Core.Factors;
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

        public override string ToString() => Delegates.GetClassAndMethodName(actionToTake);
        
        #endregion

        
        #region Constructors

        public ObservedActionResponse(Action actionToExecute) : base()
        {
            actionToTake = actionToExecute ??  throw new ArgumentNullException(nameof(actionToExecute));
        }

        #endregion
    }
}