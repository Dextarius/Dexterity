namespace Core.Factors
{
    public interface IReactorCoreOwner: IFactor
    {
        bool CoreDestabilized();
        void CoreTriggered();
    }
}