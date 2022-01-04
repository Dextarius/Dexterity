using System;
using Core.Causality;
using Core.Factors;
using Core.States;
using Factors.Outcomes.ObservedOutcomes;
using JetBrains.Annotations;
using static Core.Tools.Delegates;

namespace Factors
{
    public class Reaction : Reactor
    {
        #region Instance Fields

        [NotNull]
        protected readonly IResponse response;
        
        #endregion
        
        
        #region Properties

        protected override IOutcome Outcome => response;

        #endregion


        #region Constructors
        
        public Reaction(IResponse reactionResponse, string name = null) : 
            base(name?? nameof(Reactor))
        {
            response = reactionResponse;
        }
        
        public Reaction(Action actionToExecute, string name = null) : 
            base(name?? CreateDefaultName<Reaction>(actionToExecute))
        {
            if(actionToExecute == null) { throw new ArgumentNullException(nameof(actionToExecute)); }
            
            response = new ObservedActionResponse(actionToExecute);
        }

        #endregion
    }
}