using System;
using Core.Causality;
using Core.Factors;
using Core.States;
using Factors.Cores.DirectReactorCores;
using Factors.Cores.ObservedReactorCores;
using JetBrains.Annotations;
using static Core.Tools.Delegates;

namespace Factors
{
    public class Reaction : Reactor<IReactorCore>
    {
        public static Reaction Create<T>(Action<T> actionToTake, IFactor<T> inputSource) =>
            new Reaction(
                new DirectActionResponse<T>(actionToTake, inputSource));

        #region Constructors
        
        public Reaction(IReactorCore reactionCore, string name = null) : 
            base(reactionCore, name?? nameof(Reaction))
        {

        }
        
        // public Reaction(Action actionToExecute, string name = null) : 
        //     base(new ObservedActionResponse(actionToExecute), 
        //         name?? CreateDefaultName<Reaction>(actionToExecute))
        // {
        //     if(actionToExecute == null) { throw new ArgumentNullException(nameof(actionToExecute)); }
        // }

        #endregion
    }
}