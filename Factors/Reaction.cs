using System;
using Core.Causality;
using Core.Factors;
using Core.States;
using Factors.Cores.ObservedReactorCores;
using JetBrains.Annotations;
using static Core.Tools.Delegates;

namespace Factors
{
    public class Reaction : Reactor<IReactor>
    {
        #region Constructors
        
        public Reaction(IReactor reactionCore, string name = null) : 
            base(reactionCore, name?? nameof(Reaction))
        {

        }
        
        public Reaction(Action actionToExecute, string name = null) : 
            base(new ObservedActionResponse(actionToExecute), 
                name?? CreateDefaultName<Reaction>(actionToExecute))
        {
            if(actionToExecute == null) { throw new ArgumentNullException(nameof(actionToExecute)); }
        }

        #endregion
    }
}