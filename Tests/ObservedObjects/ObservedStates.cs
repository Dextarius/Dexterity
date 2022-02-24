﻿using Core.Factors;
using Core.States;
using Factors.Observer;
using NUnit.Framework;
using Tests.Tools.Interfaces;
using Tests.Tools.Mocks;
using static Tests.Tools.Tools;

namespace Tests.ObservedObjects
{
    public class ObservedStates<TState, TFactory, TValue>  
        where TState   : IState<TValue>, IInvolved
        where TFactory : IState_T_Factory<TState, TValue>, new()
    {
        #region Instance Fields

        private TFactory factory = new TFactory();

        #endregion


        #region Tests


        
        #endregion
    }
}