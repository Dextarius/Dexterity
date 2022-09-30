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
        #region Instance Methods

        public override bool CoresAreNotEqual(IReactorCore oldCore, IReactorCore newCore) => oldCore != newCore;

        #endregion
        

        #region Constructors
        
        public Reaction(IReactorCore reactionCore, string name = null) : base(reactionCore, name?? nameof(Reaction))
        {
        }

        #endregion
        
    }
}