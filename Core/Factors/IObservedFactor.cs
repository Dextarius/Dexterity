namespace Core.Factors
{
    public interface IObservedFactor<out T> : IFactor
    {
        T Peek();   
    }
}