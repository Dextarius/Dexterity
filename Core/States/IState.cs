using Core.Factors;
using Core.Redirection;

namespace Core.States
{
    public interface IState<T> : IFactor<T>
    {
        new T Value { get; set; }
    }
}