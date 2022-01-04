using Core.Factors;

namespace Core.States
{
    public interface IResult<out T> : IOutcome
    {
        T    Value { get; }
        T    Peek();
    }
}