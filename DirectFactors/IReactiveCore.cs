using Core.Factors;
using Core.States;

namespace DirectFactors
{
    public interface IReactiveCore
    {
        bool IsNecessary { get; }
        bool HasReacted  { get; }

        bool TryStabilizeOutcome();
        void OnNecessary();
        void OnNotNecessary();
    }
    
    public interface IReactiveCore<T> : IReactiveCore , IValue<T>
    {
        bool GenerateOutcome(out T oldValue, out T newValue);
        void SetOwner(IReactiveCoreOwner<T> owner);
    }

    public interface IReactiveCoreOwner<T>
    {
        bool CoreDestabilized(IReactiveCore<T> destabilizedCore);
        void CoreTriggered(IReactiveCore<T> triggeredCore);
        void CoreValueChanged(IReactiveCore<T> core, T oldValue, T newValue);

    }
}