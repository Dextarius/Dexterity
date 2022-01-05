using Core.Factors;

namespace Core.States
{
    public interface IResult<out T> : IReactor, IFactor<T>
    {

    }
}