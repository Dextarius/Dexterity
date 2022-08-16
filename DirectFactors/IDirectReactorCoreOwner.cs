using Core.Factors;

namespace DirectFactors
{
    public interface IDirectReactorCoreOwner<in TValue> : IFactor
    {
        bool CoreTriggered();
        bool CoreDestabilized();
    }
}