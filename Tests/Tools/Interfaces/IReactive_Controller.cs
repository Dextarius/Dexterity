using System;
using System.Reflection;
using Core.Factors;

namespace Tests.Tools.Interfaces
{
    
    public interface IReactive_Controller<out TReactive, TValue> : IFactor_T_Controller<TReactive, TValue>
        where TReactive : IReactive<TValue>
    {
        TValue ExpectedValue  { get; }
    }
    
    public interface IReaction_Controller : ITriggeredState_Controller
    {
        new IReactor ControlledInstance { get; }
    }
}