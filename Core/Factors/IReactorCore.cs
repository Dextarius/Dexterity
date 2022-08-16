using System;
using Core.States;

namespace Core.Factors
{
    public interface IReactorCore : IFactorCore, ITriggeredState 
    {
        void SetCallback(IReactorCoreCallback callback);
    }
    
    public interface IReactorCore<T> : IReactorCore, IValue<T>, IValueEquatable<T>
    {
        
    }
}