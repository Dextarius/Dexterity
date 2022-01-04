using Core.States;

namespace Core.Factors
{
    public interface IReactive<out T> : IReactor, IFactor<T>
    {
        
    }
}