using Core.Factors;

namespace Core.States
{
    public interface IState<T> : IFactor<T>
    {
        new T Value { get; set; }
    }
}