namespace Core.Factors
{
    public interface IReactorCoreOwner: IFactor
    {
        bool CoreDestabilized();
        void CoreTriggered();
    }
    
    public interface ICollectionCoreOwner<TValue>: IFactor
    {
        void CollectionChanged();
    }
}