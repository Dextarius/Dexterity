using Core.States;

namespace Core.Factors
{
    public interface IReactorCoreCallback : IFactorCoreCallback, INecessary
    {
        bool ReactorDestabilized(IReactorCore destabilizedCore);
        bool ReactorTriggered(IReactorCore triggeredCore);
    }
}