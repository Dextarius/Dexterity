namespace Core.Factors
{
    public interface ICollectionCoreOwner<TValue>: IFactor
    {
        void CollectionChanged();
    }
}