using System;
using Core.States;

namespace Core.Factors
{
    public interface IReactorCore : IFactorCore, ITriggeredState
    {
        bool IsNecessary { get; }
        bool HasReacted  { get; }

        bool GenerateOutcome();
        bool TryStabilizeOutcome();
        void OnNecessary();
        void OnNotNecessary();
        void SetOwner(IReactorCoreOwner owner);
    }
}