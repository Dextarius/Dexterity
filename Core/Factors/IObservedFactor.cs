namespace Core.Factors
{
    public interface IObservedFactor<T> : IFactor<T>
    {
        T Peek();   
    }
}