using System;
using Causality;
using Causality.Processes;
using Causality.States;
using Core.Causality;
using Core.Factors;
using JetBrains.Annotations;
using static Core.Tools.Delegates;

namespace Factors
{
    public class Reaction : Reactor
    {
        #region Instance Fields

        [NotNull]
        protected          IOutcome outcome = InvalidOutcome.Default;
        protected readonly IProcess reactionProcess;

        #endregion
        
        #region Properties

        protected override IOutcome Outcome => outcome;

        #endregion

        
        #region Instance Methods


        protected override bool Act()
        {
            IOutcome newOutcome = new Outcome(this);
            
            Observer.ObserveInteractions(reactionProcess, newOutcome);

            using (Observer.PauseObservation()) //- Prevents anything we do here from adding dependencies to any other observations this one may be nested in. 
            {
                outcome = newOutcome;            
                return true;
            }
        }

        #endregion


        #region Constructors

        public Reaction(IProcess processToExecute, string nameToGive = null) : base(nameToGive)
        {
            reactionProcess = processToExecute;
        }
        
        public Reaction(Action actionToExecute, string name = null) : 
            this(new ActionProcess(actionToExecute), name?? GetClassAndMethodName(actionToExecute))
        {
            if(actionToExecute == null) { throw new ArgumentNullException(nameof(actionToExecute)); }
        }

        #endregion
    }
}

//- TODO : Do we want to allow people to Subscribe to these?  They don't have a value but we should reason about it. 

