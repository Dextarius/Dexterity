namespace Core.Factors
{
    public interface ICollectionCoreCallback<in TValue> : IReactorCoreCallback
    {
        bool ElementAdded(TValue valueAdded);
        bool ElementRemoved(TValue valueRemoved);
    }
}