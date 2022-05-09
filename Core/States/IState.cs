using Core.Factors;

namespace Core.States
{
    public interface IState<T> : IFactor<T>, IDeterminant
    {
        new T Value { get; set; }
    }
}