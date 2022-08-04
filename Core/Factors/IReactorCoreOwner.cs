namespace Core.Factors
{
    public interface IReactorCoreOwner: IFactor
    {
        bool CoreDestabilized(IReactorCore destabilizedCore);
        void CoreTriggered(IReactorCore triggeredCore);
    }
}