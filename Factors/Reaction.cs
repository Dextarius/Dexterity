using System;
using Causality;
using Causality.Processes;
using Causality.States;
using Core.Causality;
using Core.Factors;
using Core.States;
using JetBrains.Annotations;
using static Core.Tools.Delegates;

namespace Factors
{
    //- TODO : Should we make Reactions that take an Action<T>, and can be triggered by new values of a Reactive<T>?
    //         This could probably replace Subscriptions if it referenced the Reactives directly.
    //          R: You don't even need that.  If a regular Reaction uses a value from an Reactive it will
    //          re-run if that value changes. If you wanted something that used the old value as well though,
    //          that would have to be a new type of Reactor. 
    public class Reaction : Reactor
    {
        #region Instance Fields

        [NotNull]
        protected IResult result;
        
        #endregion
        
        #region Properties

        protected override IResult Result => result;
        #endregion


        #region Constructors

        public Reaction(IProcess processToExecute, string nameToGive = null) : base(nameToGive)
        {
            result = new Response(this, processToExecute);
        }
        
        public Reaction(Action actionToExecute, string name = null) : 
            this(new ActionProcess(actionToExecute), name?? CreateDefaultName<Reaction>(actionToExecute))
        {
            if(actionToExecute == null) { throw new ArgumentNullException(nameof(actionToExecute)); }
        }

        #endregion
    }
}