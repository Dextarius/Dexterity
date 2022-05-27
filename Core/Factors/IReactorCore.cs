using System;

namespace Core.Factors
{
    public interface IReactorCore : IFactorCore
    {
        bool IsUnstable       { get; }
        bool HasBeenTriggered { get; }
        bool IsReacting       { get; }
        bool IsStabilizing    { get; }
        bool IsNecessary      { get; }
        bool HasReacted       { get; }
        bool HasTriggers      { get; }
        int  NumberOfTriggers { get; }

        bool Trigger();
        bool Trigger(IFactor triggeringFactor, out bool removeSubscription);
        bool GenerateOutcome();
        bool Destabilize(IFactor factor);
        bool TryStabilizeOutcome();
        void OnNecessary();
        void OnNotNecessary();
    }
}